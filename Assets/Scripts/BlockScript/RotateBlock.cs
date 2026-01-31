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

        Quaternion start = levelBlock.rotation;
        Quaternion end = start * Quaternion.AngleAxis(angle, rotationAxis);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            levelBlock.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        levelBlock.rotation = end;
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
