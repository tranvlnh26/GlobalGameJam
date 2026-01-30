using UnityEngine;

public class ElevatorNgangBlock : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minX = 0f;
    public float maxX = 10f;

    // 1 = trái, -1 = phải
    private int direction = 1;
    private bool playerOn = false;

    void Update()
    {
        if (playerOn)
        {
            elevatormove();
        }
    }

    void elevatormove()
    {
        // Đi sang trái trước
        transform.position += Vector3.left * direction * moveSpeed * Time.deltaTime;

        // Chạm biên trái -> quay sang phải
        if (transform.position.x <= minX)
        {
            direction = -1;
        }
        // Chạm biên phải -> quay sang trái
        else if (transform.position.x >= maxX)
        {
            direction = 1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOn = true;
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOn = false;
            collision.transform.SetParent(null);
        }
    }
}
