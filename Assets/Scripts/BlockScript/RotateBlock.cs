using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class RotateBlock : MonoBehaviour
{
    public Transform levelBlock;
    public float angle = -90f;
    public Vector3 rotationAxis = Vector3.forward;
    public float duration = 2f;
    public KeyCode rotateKey = KeyCode.R;
    public GameObject button;
    
    public float De = 0.1f; // Ví dụ: Đè nút xuống còn 0.1 (hoặc 10 như ý bạn)
    public float animSpeed = 5f; // Tốc độ co giãn

    bool canInteract;
    bool rotating;
    bool rotated;

    async void Update()
    {
        if (!canInteract) return;
        if (rotating || rotated) return;

        if (Input.GetKeyDown(rotateKey))
        {
            AudioManager.Instance.PlaySFX("RotateBlock");
            StartCoroutine(RotateLevel());
            await AnimateButton();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = false;
    }

    IEnumerator RotateLevel()
    {
        rotating = true;
        rotated = true;

        // === TẠM DỪNG PHYSICS CỦA PLAYER ===
        GameObject playerObj = GameObject.FindWithTag("Player");
        Rigidbody2D playerRb = null;

        if (playerObj != null)
        {
            playerRb = playerObj.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // Freeze velocity và tắt physics simulation
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.simulated = false;
            }
        }

        // === LƯU VỊ TRÍ BAN ĐẦU CỦA PLAYER SO VỚI PIVOT ===
        Vector3 pivot = levelBlock.position;
        Vector3 playerStartOffset = playerObj != null ? playerObj.transform.position - pivot : Vector3.zero;

        // === XOAY LEVEL BLOCK VÀ PLAYER CÙNG LÚC ===
        Quaternion start = levelBlock.rotation;
        Quaternion end = start * Quaternion.AngleAxis(angle, rotationAxis);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            
            // Xoay levelBlock
            Quaternion currentRotation = Quaternion.Slerp(start, end, t);
            levelBlock.rotation = currentRotation;

            // Tính toán vị trí mới của Player (xoay quanh pivot)
            if (playerObj != null)
            {
                // Tính góc đã xoay so với ban đầu
                Quaternion deltaRotation = currentRotation * Quaternion.Inverse(start);
                
                // Xoay offset của player quanh pivot
                Vector3 rotatedOffset = deltaRotation * playerStartOffset;
                
                // Cập nhật vị trí player
                playerObj.transform.position = pivot + rotatedOffset;
                
                // QUAN TRỌNG: Giữ player luôn đứng thẳng
                playerObj.transform.rotation = Quaternion.identity;
            }

            yield return null;
        }

        // Đảm bảo rotation cuối cùng chính xác
        levelBlock.rotation = end;

        if (playerObj != null)
        {
            // Vị trí cuối cùng
            Quaternion finalDelta = end * Quaternion.Inverse(start);
            playerObj.transform.position = pivot + (finalDelta * playerStartOffset);
            playerObj.transform.rotation = Quaternion.identity;

            // === KHÔI PHỤC PHYSICS ===
            if (playerRb != null)
            {
                playerRb.simulated = true;
            }
        }

        rotating = false;
    }
    
    // Đổi Task thành Awaitable (Chuẩn Unity 6)
    async Awaitable AnimateButton()
    {

        // Lấy scale hiện tại để giữ nguyên X và Y, chỉ thay đổi Z
        Vector3 startScale = button.transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x, startScale.y, De);

        // Vòng lặp: Chạy cho đến khi Z gần bằng target (sai số 0.01)
        // LƯU Ý: Không dùng dấu != với số thực (float) vì rất dễ bị lỗi lặp vô tận
        while (Mathf.Abs(button.transform.localScale.z - De) > 0.01f)
        {
            // Dùng MoveTowards để thay đổi đều đặn theo thời gian
            button.transform.localScale = Vector3.MoveTowards(
                button.transform.localScale, 
                targetScale, 
                animSpeed * Time.deltaTime
            );

            // Chờ frame tiếp theo (Unity 6)
            await Awaitable.NextFrameAsync();
        }

        // Chốt số đẹp khi kết thúc
        button.transform.localScale = targetScale;
    }
}
