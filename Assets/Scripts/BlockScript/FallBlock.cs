using UnityEngine;

public class FallBlock : MonoBehaviour
{
    public float fallspeed = 0.5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = fallspeed;
        }
    }

}
