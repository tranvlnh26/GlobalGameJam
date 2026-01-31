using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Settings")] [SerializeField] private Volume _globalVolume;
    [SerializeField] private float _glitchDuration = 0.2f;

    [Header("Intensity Settings")] [SerializeField]
    private float _maxAberration = 1.0f; // Tách màu tối đa

    [SerializeField] private float _maxDistortion = -0.6f; // Bẻ cong màn hình (Số âm để phình ra)
    [SerializeField] private float _maxGrain = 1.0f; // Độ nhiễu hạt

    // Cache các hiệu ứng để điều khiển
    private ChromaticAberration _ca;
    private LensDistortion _ld;
    private FilmGrain _fg;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Lấy tham chiếu tới các hiệu ứng trong Volume
        var profile = _globalVolume.profile;
        profile.TryGet(out _ca);
        profile.TryGet(out _ld);
        profile.TryGet(out _fg);
    }

    public void TriggerGlitch()
    {
        StopAllCoroutines();
        StartCoroutine(GlitchCoroutine());
    }

    private IEnumerator GlitchCoroutine()
    {
        float timer = 0;

        // 1. TĂNG VỌT (BÙM!)
        SetValues(_maxAberration, _maxDistortion, _maxGrain);

        // Giữ nguyên trạng thái cực đại trong 1 khoảng cực ngắn (ví dụ 0.05s) để mắt kịp thấy
        yield return new WaitForSeconds(0.05f);

        // 2. GIẢM DẦN (Fade out)
        while (timer < _glitchDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _glitchDuration;

            // Dùng Lerp để giảm từ Max về 0 (hoặc về giá trị mặc định)
            float currentAberration = Mathf.Lerp(_maxAberration, 0f, progress);
            float currentDistortion = Mathf.Lerp(_maxDistortion, 0f, progress);
            float currentGrain = Mathf.Lerp(_maxGrain, 0f, progress);

            SetValues(currentAberration, currentDistortion, currentGrain);

            yield return null;
        }

        // 3. CLEAN UP (Về 0 tuyệt đối)
        SetValues(0f, 0f, 0f);
    }

    private void SetValues(float ab, float dist, float grain)
    {
        if (_ca != null) _ca.intensity.value = ab;
        if (_ld != null) _ld.intensity.value = dist;
        if (_fg != null) _fg.intensity.value = grain;
    }
}