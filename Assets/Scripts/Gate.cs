using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Transform objectToOpen;
    public float openTime = 0.25f;
    public float leavePokingOut = 0.25f;
    public GameObject poof;

    [ContextMenu("Open")]
    public void Open()
    {
        StartCoroutine(OpenCoroutine());
        Instantiate(poof, transform.position, transform.rotation);
        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator OpenCoroutine()
    {
        float travelDistance = objectToOpen.GetComponent<SpriteRenderer>().sprite.bounds.size.y - leavePokingOut;
        Vector3 initialPosition = objectToOpen.localPosition;
        float startTime = Time.time;

        for (float t = 0f; t < openTime; t = Time.time - startTime)
        {
            yield return new WaitForSeconds(1f / 12f);
            objectToOpen.localPosition = initialPosition + Vector3.down * (t / openTime) * travelDistance;
        }

        objectToOpen.localPosition = initialPosition + Vector3.down * travelDistance;
    }
}
