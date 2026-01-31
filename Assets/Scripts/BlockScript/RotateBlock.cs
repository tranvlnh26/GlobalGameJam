using UnityEngine;
using System.Collections;

public class RotateBlock : MonoBehaviour
{
    public Transform levelBlock;
    public float angle = -90f;
    public Vector3 rotationAxis = Vector3.forward;
    public float duration = 2f;
    public KeyCode rotateKey = KeyCode.R;
    public GameObject button;

    bool canInteract;
    bool rotating;
    bool rotated;

    void Update()
    {
        if (!canInteract) return;
        if (rotating || rotated) return;

        if (Input.GetKeyDown(rotateKey))
        {
            StartCoroutine(RotateLevel());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = false;
    }

    IEnumerator RotateLevel()
    {
        rotating = true;
        rotated = true;

        Quaternion start = levelBlock.rotation;
        Quaternion end = start * Quaternion.AngleAxis(angle, rotationAxis);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            levelBlock.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        levelBlock.rotation = end;
        rotating = false;
    }
    
    // IEnumerator Animate()
    // {
    //     var target = 10;
    //     while (button.transform.localScale.z != target)
    //     {
    //         
    //     }
    // }
}
