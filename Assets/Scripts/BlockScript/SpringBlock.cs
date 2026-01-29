using UnityEngine;

public class SpringBlock : MonoBehaviour
{
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 10f;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("IsPlayer")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        float bounceMultiplier = 1.4f;
        float force = Mathf.Abs(rb.linearVelocity.y) * bounceMultiplier;
        force = Mathf.Clamp(force, minForce, maxForce);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }
}
