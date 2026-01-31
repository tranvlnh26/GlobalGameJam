using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SpringBlock : MonoBehaviour
{
    [Header("Bouncing Settings")]
    [SerializeField] private float minForce = 10f;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float bounceMultiplier = 1.4f;

    [Header("Visual Elements")]
    [SerializeField] private GameObject spring; // Lò xo
    [SerializeField] private GameObject cube;   // Khối đế hoặc nắp trên

    [Header("Animation Settings")]
    [SerializeField] private float squashScaleY = 0.025f; // Thu nhỏ còn 50%
    [SerializeField] private float cubeDownOffset = 0.2f; // Cube hạ xuống bao nhiêu
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 _originalSpringScale;
    private Vector3 _originalCubePos;
    private bool _isAnimating;

    void Start()
    {
        if (spring) _originalSpringScale = spring.transform.localScale;
        if (cube) _originalCubePos = cube.transform.localPosition;
    }

    async void OnTriggerEnter2D(Collider2D other)
    {
        
        if (!other.CompareTag("Player")) return;

        var rb = other.GetComponent<Rigidbody2D>();
        if (rb == null || rb.linearVelocity.y > 0.1f) return;

        var force = Mathf.Clamp(Mathf.Abs(rb.linearVelocity.y) * bounceMultiplier, minForce, maxForce);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // Chạy hiệu ứng
        if (!_isAnimating)
        {
            AudioManager.Instance.PlaySpring();
            await PlaySpringEffectAsync();
        }
    }

    async Task PlaySpringEffectAsync()
    {
        _isAnimating = true;

        var targetSpringScale = new Vector3(_originalSpringScale.x, squashScaleY, _originalSpringScale.z);
        var targetCubePos = _originalCubePos + Vector3.down * cubeDownOffset;

        // 1. Nén
        await LerpTransform(1f); 

        // 2. Bung
        await LerpTransform(0f); // Hàm bổ trợ để tái sử dụng logic

        _isAnimating = false;

        // Hàm local để xử lý Lerp
        async Task LerpTransform(float targetState)
        {
            float t = 0;
            var startScale = spring.transform.localScale;
            var endScale = (targetState > 0.5f) ? targetSpringScale : _originalSpringScale;
            
            var startPos = cube.transform.localPosition;
            var endPos = (targetState > 0.5f) ? targetCubePos : _originalCubePos;

            while (t < 1)
            {
                t += Time.deltaTime * animationSpeed;
                if (spring) spring.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                if (cube) cube.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                
                await Task.Yield();
            }
        }
    }
}