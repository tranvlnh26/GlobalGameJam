using System.Collections;
using UnityEngine;

public class RotateBlock : MonoBehaviour
{
    public Transform LevelBlock;
    public float angle = 90;
    public Vector3 rotationAxis = Vector3.forward;
    public float duration = 0.5f;
    bool rotated;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if ((rotated)) return;
        StartCoroutine(RotateLevel());
    }
    IEnumerator RotateLevel()
    {
        rotated = true;
        Quaternion start = LevelBlock.rotation;
        Quaternion end = LevelBlock.rotation * Quaternion.Euler(rotationAxis * angle);
        float time = 0f;
        while (time < duration)
        {
            LevelBlock.rotation = Quaternion.Slerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        LevelBlock.rotation = end;
    }
}
