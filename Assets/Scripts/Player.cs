using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 14f;

    Rigidbody2D rb;
    bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }
}
