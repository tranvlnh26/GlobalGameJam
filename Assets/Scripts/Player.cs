using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public static float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;

    [Header("Slide Settings")]
    public static bool onSmoothBlock = false;
    [SerializeField] private float normalAcceleration = 60f;
    [SerializeField] private float slideAcceleration = 6f;

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
    private bool _isFacingRight = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

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
        
        Flip();
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
        {
            MaskManager.Instance.ApplyMask(MaskType.Blue);
        }
        else if (Input.GetKey(KeyCode.E) && MaskManager.Instance.currentMask != MaskType.Red)
        {
            MaskManager.Instance.ApplyMask(MaskType.Red);
        }
    }
    private void Flip()
    {
        if ((!_isFacingRight || !(_inputX < 0f)) && (_isFacingRight || !(_inputX > 0f)))
            return;

        _isFacingRight = !_isFacingRight;
        var localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    
}
