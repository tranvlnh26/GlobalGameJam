using System;
using UnityEngine;
public enum MaskType { None, Red, Blue }
public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance { get; private set; }
    
    // Cache LayerMasks để tối ưu hiệu năng
    private int _maskDefault;
    private int _maskRedWorld;
    private int _maskBlueWorld;
    
    // Cache Layer ID cho Physics
    private int _layerPlayer;
    private int _layerRed;
    private int _layerBlue;
    public MaskType currentMask;

    public event Action<MaskType> onMaskChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeLayers();
    }

    private void InitializeLayers()
    {
        // Setup Bitmask cho Camera Culling
        _maskDefault = LayerMask.GetMask("Default", "BaseWorld", "Player"); // Luôn thấy Player và Base
        _maskRedWorld = LayerMask.GetMask("RedWorld");
        _maskBlueWorld = LayerMask.GetMask("BlueWorld");

        // Setup Layer Index cho Physics Collision
        _layerPlayer = LayerMask.NameToLayer("Player");
        _layerRed = LayerMask.NameToLayer("RedWorld");
        _layerBlue = LayerMask.NameToLayer("BlueWorld");
        ApplyMask(MaskType.None);
    }

    public void ApplyMask(MaskType type)
    {
        var mainCam = Camera.main;
        if (mainCam == null) return;
        currentMask = type;
        // 1. VISUAL: Camera Culling Mask
        AudioManager.Instance.PlayMask();
        switch (type)
        {

            case MaskType.None:
                // Chỉ thấy Base, không thấy Red/Blue
                mainCam.cullingMask = _maskDefault;
                break;
            case MaskType.Red:
                // Thấy Base + Red
                mainCam.cullingMask = _maskDefault | _maskRedWorld;
                break;
            case MaskType.Blue:
                // Thấy Base + Blue
                mainCam.cullingMask = _maskDefault | _maskBlueWorld;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        // 2. PHYSICS: Ignore Collision
        UpdatePhysicsCollision(type);
        onMaskChanged?.Invoke(type);
        // 3. TRIGGER VFX (Glitch)
        if (VFXManager.Instance != null) 
        {
            VFXManager.Instance.TriggerGlitch();
        }
    }

    private void UpdatePhysicsCollision(MaskType type)
    {
        // Reset collision logic
        Physics2D.IgnoreLayerCollision(_layerPlayer, _layerRed, type != MaskType.Red);
        Physics2D.IgnoreLayerCollision(_layerPlayer, _layerBlue, type != MaskType.Blue);
        Debug.Log($"[Mask] Switched to: {type}");
    }
}
