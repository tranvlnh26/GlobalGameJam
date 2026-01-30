using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] public static float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;

    [Header("Detection Settings")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask baseGroundMask;

    [Header("Game Feel (Timers)")] 
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private Rigidbody2D _rb;
    private float _inputX;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _isGrounded;

    void Start() => _rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        
        HandleGroundCheck();
        HandleTimers();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferCounter = jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && _rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * jumpCutMultiplier);
            _coyoteTimeCounter = 0f; // Prevent double jumps
        }

        // 3. Execute Jump
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            PerformJump();
        }
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_inputX * moveSpeed, _rb.linearVelocity.y);
    }

    private void HandleGroundCheck()
    {
        LayerMask currentEffectiveLayer = baseGroundMask;

        if (MaskManager.Instance != null && MaskManager.Instance.currentMask != MaskType.None)
        {
            string layerName = MaskManager.Instance.currentMask.ToString() + "World";
            int worldLayer = LayerMask.NameToLayer(layerName);
            if (worldLayer != -1) currentEffectiveLayer |= (1 << worldLayer);
        }

        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, currentEffectiveLayer);
    }

    private void HandleTimers()
    {
        _coyoteTimeCounter = _isGrounded ? coyoteTime : _coyoteTimeCounter - Time.deltaTime;
        _jumpBufferCounter -= Time.deltaTime;
    }

    private void PerformJump()
    {
        var finalJumpForce = jumpForce;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); // Clear velocity for consistent jump height
        _rb.AddForce(Vector2.up * finalJumpForce, ForceMode2D.Impulse);

        _jumpBufferCounter = 0f;
        _coyoteTimeCounter = 0f;
    }
}