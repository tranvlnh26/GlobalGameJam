using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    // Kiểm tra va chạm bắt đầu
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem cái đụng vào có phải là Player không?
        // (Nhớ đặt Tag cho nhân vật là "Player" nhé)
        if (collision.gameObject.CompareTag("Player"))
        {
            // BƯỚC QUAN TRỌNG: Gán nhân vật làm con của Platform này
            collision.transform.SetParent(this.transform);
        }
    }

    // Kiểm tra khi rời va chạm (Nhảy hoặc đi ra ngoài)
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gỡ nhân vật ra, trả về làm con của "null" (tức là con của Scene)
            collision.transform.SetParent(null);
        }
    }
}