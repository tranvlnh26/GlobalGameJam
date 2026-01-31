using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshPro))]
public class HintTrigger : MonoBehaviour
{
    private TextMeshPro tm;
    private Coroutine fadeRoutine;

    [SerializeField] private float fadeSpeed = 3f;

    private void Awake()
    {
        tm = GetComponent<TextMeshPro>();
        SetAlpha(0f); // ẩn ban đầu
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f); // fade in
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(0f); // fade out
        }
    }

    void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(targetAlpha));
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float alpha = tm.color.a;

        while (!Mathf.Approximately(alpha, targetAlpha))
        {
            alpha = Mathf.MoveTowards(alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            SetAlpha(alpha);
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        Color c = tm.color;
        c.a = a;
        tm.color = c;
    }
}
