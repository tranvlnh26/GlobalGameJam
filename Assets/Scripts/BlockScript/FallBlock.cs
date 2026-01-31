using UnityEngine;
using System.Threading;

[RequireComponent(typeof(Rigidbody2D))]
public class DelayedTrapBlock : MonoBehaviour
{
    [Header("Giai đoạn 1: Cảnh báo")]
    [SerializeField] private float delayBeforeFall = 1.0f; // Thời gian rung lắc trước khi sập
    [SerializeField] private float shakeAmount = 0.05f;    // Độ mạnh của rung lắc

    [Header("Giai đoạn 2: Sập xuống")]
    [SerializeField] private float fallDistance = 5f;      // Khoảng cách rơi
    [SerializeField] private float fallSpeed = 20f;        // Tốc độ rơi (Nhanh)

    [Header("Giai đoạn 3: Hồi phục")]
    [SerializeField] private float stayAtBottomTime = 2f;  // Thời gian nằm chờ dưới đáy
    [SerializeField] private float returnSpeed = 5f;       // Tốc độ bay về (Chậm)

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private Rigidbody2D _rb;
    private bool _isActivated = false; // Biến khóa: Đã kích hoạt thì phải chạy hết quy trình
    private CancellationTokenSource _cts;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // Cấu hình Physics chuẩn
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        _startPos = transform.position;
        _targetPos = new Vector3(_startPos.x, _startPos.y - fallDistance, _startPos.z);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Dính nhân vật vào Block
            other.transform.SetParent(this.transform);

            // Chỉ kích hoạt nếu block đang ở trạng thái nghỉ
            if (!_isActivated)
            {
                RunTrapSequence();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Chỉ gỡ nhân vật ra, KHÔNG hủy quy trình rơi
            other.transform.SetParent(null);
        }
    }

    private async void RunTrapSequence()
    {
        _isActivated = true; // Khóa lại
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            // --- GIAI ĐOẠN 1: RUNG LẮC CẢNH BÁO (SHAKE) ---
            float timer = 0f;
            while (timer < delayBeforeFall)
            {
                token.ThrowIfCancellationRequested();
                timer += Time.fixedDeltaTime;

                // Tạo hiệu ứng rung nhẹ xung quanh vị trí gốc
                Vector2 shakeOffset = Random.insideUnitCircle * shakeAmount;
                _rb.MovePosition(_startPos + (Vector3)shakeOffset);

                await Awaitable.FixedUpdateAsync(token);
            }
            // Reset về vị trí chuẩn trước khi rơi
            _rb.MovePosition(_startPos);


            // --- GIAI ĐOẠN 2: SẬP XUỐNG (FALL) ---
            while (Vector3.Distance(_rb.position, _targetPos) > 0.01f)
            {
                token.ThrowIfCancellationRequested();

                Vector2 newPos = Vector2.MoveTowards(_rb.position, _targetPos, fallSpeed * Time.fixedDeltaTime);
                _rb.MovePosition(newPos);

                await Awaitable.FixedUpdateAsync(token);
            }
            _rb.MovePosition(_targetPos);


            // --- GIAI ĐOẠN 3: NẰM CHỜ (STAY) ---
            timer = 0f;
            while (timer < stayAtBottomTime)
            {
                token.ThrowIfCancellationRequested();
                timer += Time.fixedDeltaTime;
                
                _rb.MovePosition(_targetPos); // Giữ vị trí cố định
                
                await Awaitable.FixedUpdateAsync(token);
            }


            // --- GIAI ĐOẠN 4: BAY VỀ (RETURN) ---
            while (Vector3.Distance(_rb.position, _startPos) > 0.01f)
            {
                token.ThrowIfCancellationRequested();

                Vector2 newPos = Vector2.MoveTowards(_rb.position, _startPos, returnSpeed * Time.fixedDeltaTime);
                _rb.MovePosition(newPos);

                await Awaitable.FixedUpdateAsync(token);
            }
            _rb.MovePosition(_startPos);

        }
        catch (System.OperationCanceledException)
        {
            // Xử lý hủy nếu cần
        }
        finally
        {
            // Mở khóa để chu kỳ sau có thể bắt đầu
            _isActivated = false;
            if (_cts != null) { _cts.Dispose(); _cts = null; }
        }
    }

    private void OnDestroy()
    {
        if (_cts != null) _cts.Cancel();
    }
}