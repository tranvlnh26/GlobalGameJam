using UnityEngine;
using System.Collections;

public class FadeBlock3D : MonoBehaviour
{
    [Header("Timing")]
    public float fadeDelay = 2f;      // Thời gian chờ trước khi bắt đầu shrink
    public float shrinkDuration = 0.5f; // Thời gian thu nhỏ
    public float respawnTime = 3f;    // Thời gian chờ trước khi respawn

    [Header("Animation")]
    public AnimationCurve shrinkCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // Curve cho shrink animation

    BoxCollider2D[] col;
    MeshRenderer sr;
    bool running;
    Vector3 originalScale;

    void Awake()
    {
        col = GetComponents<BoxCollider2D>();
        sr = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (running) return;

        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        running = true;

        // Chờ trước khi bắt đầu shrink
        yield return new WaitForSeconds(fadeDelay);

        // Animation thu nhỏ dần (vẫn giữ collision để player đứng được)
        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            float scale = shrinkCurve.Evaluate(t);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        // Ẩn hoàn toàn và tắt collision SAU KHI shrink xong
        transform.localScale = Vector3.zero;
        sr.enabled = false;
        foreach (BoxCollider2D c in col)
            c.enabled = false;

        // Chờ respawn
        yield return new WaitForSeconds(respawnTime);

        // Respawn: bật lại collider và renderer, reset scale
        transform.localScale = originalScale;
        sr.enabled = true;
        foreach (BoxCollider2D c in col)
            c.enabled = true;

        running = false;
    }
}
