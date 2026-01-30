using UnityEngine;

public class SmoothBlock : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        else
        {
            Player.moveSpeed = 10;
        }
    }
}
