using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public static float MoveSpeed = 8f;
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;

    [Header("Slide Settings")]
    public static bool onSmoothBlock = false;
    [SerializeField] private float normalAcceleration = 60f;
    [SerializeField] private float slideAcceleration = 6f;

    [Header("Detection Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask baseGroundMask;

    [Header("Void Detection")]
    [SerializeField] private float voidYThreshold = -5f;

    [Header("Game Feel (Timers)")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.43f; // Thời gian giữa các bước chân

    [Header("References")]
    [SerializeField] private GameplayUI gameplayUI;

    private Rigidbody2D _rb;
    private float _inputX;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _isGrounded;
    private bool _isDead = false;
    private float _footstepTimer = 0f;

    private Animator animation;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animator>();
        MoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (_isDead) return;
            // Kiểm tra rơi vào void
            if (transform.position.y < voidYThreshold)
        {
            Die();
            return;
        }

        _inputX = Input.GetAxisRaw("Horizontal");

        HandleGroundCheck();
        HandleTimers();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferCounter = jumpBufferTime;
        }
        if (Input.GetKeyUp(KeyCode.Space) && _rb.linearVelocity.y > 0)
        {
   
            _rb.linearVelocity = new Vector2(
                _rb.linearVelocity.x,
                _rb.linearVelocity.y * jumpCutMultiplier
            );
            _coyoteTimeCounter = 0f;
        }
        

        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            PerformJump();
        }

        maskchange();
    }

    void FixedUpdate()
    {
        if (_isDead) return;

        float targetX = _inputX * moveSpeed;
        float accel = onSmoothBlock ? slideAcceleration : normalAcceleration;

        _rb.linearVelocity = new Vector2(
            Mathf.MoveTowards(
                _rb.linearVelocity.x,
                targetX,
                accel * Time.fixedDeltaTime
            ),
            _rb.linearVelocity.y
        );
        // ===== ANIMATION =====
        animation.SetFloat("horizontal", _inputX);
        animation.SetFloat("speed", targetX);
        animation.SetBool("jump", _isGrounded);

        // ===== FOOTSTEP SOUND =====
        HandleFootstep();
    }

    private void HandleFootstep()
    {
        // Chỉ phát footstep khi: đang trên mặt đất VÀ đang di chuyển
        bool isMoving = Mathf.Abs(_inputX) > 0.1f;
        
        if (_isGrounded && isMoving)
        {
            _footstepTimer -= Time.fixedDeltaTime;
            
            if (_footstepTimer <= 0f)
            {
                AudioManager.Instance?.PlayFootstep();
                _footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Reset timer khi không di chuyển hoặc không trên mặt đất
            _footstepTimer = 0f;
        }
    }

    private void HandleGroundCheck()
    {
        LayerMask currentEffectiveLayer = baseGroundMask;

        if (MaskManager.Instance != null && MaskManager.Instance.currentMask != MaskType.None)
        {
            string layerName = MaskManager.Instance.currentMask.ToString() + "World";
            int worldLayer = LayerMask.NameToLayer(layerName);
            if (worldLayer != -1)
                currentEffectiveLayer |= (1 << worldLayer);
        }

        _isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            currentEffectiveLayer
        );
    }

    private void HandleTimers()
    {
        _coyoteTimeCounter = _isGrounded ? coyoteTime : _coyoteTimeCounter - Time.deltaTime;
        _jumpBufferCounter -= Time.deltaTime;
    }

    private void PerformJump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        _jumpBufferCounter = 0f;
        _coyoteTimeCounter = 0f;
        
        
    }

    private void maskchange()
    {
        if (Input.GetKey(KeyCode.Q) && MaskManager.Instance.currentMask != MaskType.Blue)
            MaskManager.Instance.ApplyMask(MaskType.Blue);
        else if (Input.GetKey(KeyCode.E) && MaskManager.Instance.currentMask != MaskType.Red)
            MaskManager.Instance.ApplyMask(MaskType.Red);
    }

    /// <summary>
    /// Xử lý khi player chết (rơi vào void).
    /// </summary>
    public async void Die()
    {
        if (_isDead) return;

        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;

        // Phát SFX chết
        AudioManager.Instance?.PlayDeath();
        animation.SetBool("death", true);
        // Hiển thị Lose screen
        await Awaitable.WaitForSecondsAsync(1.385f);
        if (gameplayUI != null)
        {
            gameplayUI.ShowLoseScreen();
        }
        
        else
        {
            // Fallback: tìm GameplayUI trong scene
            
            FindFirstObjectByType<GameplayUI>()?.ShowLoseScreen();

        }
        animation.SetBool("death", false); 
    }
}
