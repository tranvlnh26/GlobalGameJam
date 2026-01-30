using UnityEngine;
using System.Collections;

public class FadeBlock3D : MonoBehaviour
{
    public float fadeDelay = 2f;
    public float respawnTime = 3f;

    BoxCollider2D[] col;
    MeshRenderer sr;
    bool running;

    void Awake()
    {
        col = GetComponents<BoxCollider2D>();
        sr = GetComponent<MeshRenderer>();
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

        yield return new WaitForSeconds(fadeDelay);
        foreach (BoxCollider2D c in col)
            c.enabled = false;
        sr.enabled = false;

        yield return new WaitForSeconds(respawnTime);
        foreach (BoxCollider2D c in col)
            c.enabled = true;
        sr.enabled = true;

        running = false;
    }
}
