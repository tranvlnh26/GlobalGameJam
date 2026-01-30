using UnityEngine;

public class SmoothBlock : MonoBehaviour
{
    public float slideSpeed = 9f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player.onSmoothBlock = true;
        Player.moveSpeed = slideSpeed;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player.onSmoothBlock = false;
        Player.moveSpeed = 4f;
    }
}
