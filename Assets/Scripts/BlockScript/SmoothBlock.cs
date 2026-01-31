using UnityEngine;

public class SmoothBlock : MonoBehaviour
{
    public float slideSpeed = 9f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player.onSmoothBlock = true;
        Player.MoveSpeed = slideSpeed;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player.onSmoothBlock = false;
        Player.MoveSpeed = 8f;
    }
}
