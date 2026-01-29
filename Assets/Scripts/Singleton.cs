using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton _instance;

    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<Singleton>();
                if (_instance == null)
                {
                    var singletonObject = new GameObject(nameof(Singleton));
                    _instance = singletonObject.AddComponent<Singleton>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as Singleton;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}