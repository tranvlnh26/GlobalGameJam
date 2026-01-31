using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý âm thanh cho game True Sight.
/// Hỗ trợ phát nhạc nền và hiệu ứng âm thanh.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip maskRedSFX;      // "Gầm" trầm khi bật Mask Đỏ
    [SerializeField] private AudioClip maskBlueSFX;     // "Ping" cao khi bật Mask Xanh
    //[SerializeField] private AudioClip maskOffSFX;      // Tắt mask
    [SerializeField] private AudioClip footstepSFX;     // Tiếng bước chân kim loại
    [SerializeField] private AudioClip portalSFX;       // Hoàn thành màn
    [SerializeField] private AudioClip RBinteractSFX; // tương tác với khối RB4
    [SerializeField] private AudioClip FBinteractSFX; // tương tác với khối FB

    [Header("Volume Settings")]
    [Range(0f, 1f)] private float musicVolume = 1f;
    [Range(0f, 1f)] private float sfxVolume = 1f;

    private Dictionary<string, AudioClip> sfxDictionary;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
        InitializeSFXDictionary();
        LoadVolumeSettings();
    }

    private void InitializeAudioSources()
    {
        // Tự động tạo AudioSource nếu chưa có
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>
        {
            { "Jump", jumpSFX },
            { "Death", deathSFX },
            { "MaskRed", maskRedSFX },
            { "MaskBlue", maskBlueSFX },
            { "RotateBlock", RBinteractSFX },
            { "FallBlock", FBinteractSFX },
            { "Footstep", footstepSFX },
            { "Portal", portalSFX }
        };
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolume();
    }

    #region Music Methods

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    #endregion

    #region SFX Methods

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning($"AudioManager: SFX '{sfxName}' không tồn tại!");
        }
    }

    // Convenience methods cho các SFX thường dùng
    public void PlayJump() => PlaySFX(jumpSFX);
    public void PlayDeath() => PlaySFX(deathSFX);
    public void PlayMaskRed() => PlaySFX(maskRedSFX);
    public void PlayMaskBlue() => PlaySFX(maskBlueSFX);
    public void PLayRotateBlock() => PlaySFX(RBinteractSFX);
    public void PlayFallBlock() => PlaySFX(FBinteractSFX);
    public void PlayFootstep() => PlaySFX(footstepSFX);
    public void PlayPortal() => PlaySFX(portalSFX);

    #endregion

    #region Volume Control

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    #endregion
}
