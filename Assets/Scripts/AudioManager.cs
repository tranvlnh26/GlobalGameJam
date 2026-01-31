using UnityEngine;

/// <summary>
/// Quản lý âm thanh cho game True Sight.
/// Nhạc nền chạy xuyên suốt game và footstep SFX.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic; // Nhạc nền xuyên suốt

    [Header("SFX Clips")]
    [SerializeField] private AudioClip[] footstepSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)] private float musicVolume = 1f;
    [Range(0f, 1f)] private float sfxVolume = 1f;

    private bool _musicStarted = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
        LoadVolumeSettings();
    }

    private void Start()
    {
        // Phát nhạc nền ngay khi game bắt đầu (chỉ phát một lần)
        if (!_musicStarted)
        {
            PlayBackgroundMusic();
            _musicStarted = true;
        }
    }

    private void InitializeAudioSources()
    {
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

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolume();
    }

    #region Music Methods

    /// <summary>
    /// Phát nhạc nền xuyên suốt game
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;
        if (musicSource.isPlaying && musicSource.clip == backgroundMusic) return; // Đã đang phát
        
        musicSource.clip = backgroundMusic;
        musicSource.Play();
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

    public void PlayFootstep()
    {
        if (footstepSFX == null || sfxSource == null) return;
        var index = Random.Range(0, footstepSFX.Length);
        sfxSource.PlayOneShot(footstepSFX[index], sfxVolume * 0.2f);
    }

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
