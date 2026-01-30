using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    
    private static readonly object _lock = new object();
    
    // Cờ kiểm tra game đang tắt
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' " +
                                 "đã bị hủy do ứng dụng đang tắt. Trả về null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // Tìm instance có sẵn trong scene
                    _instance = FindFirstObjectByType<T>(); // Unity 2023+ dùng FindFirst, cũ hơn dùng FindObjectOfType

                    if (_instance == null)
                    {
                        // Nếu chưa có thì tạo mới
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Đảm bảo nó là Root object để DontDestroyOnLoad hoạt động
                        if (singletonObject.transform.parent != null)
                        {
                            singletonObject.transform.SetParent(null);
                        }

                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            
            // Đảm bảo là root trước khi DontDestroyOnLoad
            if (transform.parent != null) transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // Nếu đã có instance rồi mà tạo thêm cái nữa -> Hủy cái mới đi
            Destroy(gameObject);
        }
    }

    // Ngăn chặn tạo lại Singleton khi game đang tắt
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        // Reset flag khi object bị hủy (quan trọng nếu load scene lại)
        if (_instance == this)
        {
             _applicationIsQuitting = true;
        }
    }
}