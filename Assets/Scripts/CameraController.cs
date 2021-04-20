using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Bounds bounds;
    public Sprite squareSprite;

    private Camera camera;

    private SpriteRenderer[] letterboxes;

    void Start()
    {
        camera = GetComponent<Camera>();

        var obj = new GameObject("Letterbox");
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = Color.black;
        sr.sortingOrder = 1000;

        letterboxes = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            letterboxes[i] = Instantiate(sr, transform.position, Quaternion.identity, transform);
            letterboxes[i].enabled = false;
        }
    }

    void LateUpdate()
    {
        Vector3 position = transform.position;

        // lock to target
        if (target != null)
        {
            position.x = target.position.x;
            position.y = target.position.y;
        }

        if (bounds != null)
        {
            float vsize = camera.orthographicSize;
            float hsize = camera.aspect * vsize;

            // confine within bounds
            if (bounds.size.x > hsize * 2f)
            {
                position.x = Mathf.Max(bounds.min.x, position.x - hsize) + hsize;
                position.x = Mathf.Min(bounds.max.x, position.x + hsize) - hsize;
            }
            else
                position.x = bounds.center.x;

            if (bounds.size.y > vsize * 2f)
            {
                position.y = Mathf.Max(bounds.min.y, position.y - vsize) + vsize;
                position.y = Mathf.Min(bounds.max.y, position.y + vsize) - vsize;
            }
            else
                position.y = bounds.center.y;

            // hide anything outside bounds with letterboxes
            // |------|-------|-----|
            // |######|#######|#####|
            // |######|-------|#####|
            // |######|       |#####|
            // |######|       |#####|
            // |######|-------|#####|
            // |######|#######|#####|
            // |------|-------|-----|

            letterboxes[0].enabled = true;
            letterboxes[1].enabled = true;
            letterboxes[2].enabled = true;
            letterboxes[3].enabled = true;

            letterboxes[0].transform.position = bounds.center + // left
                new Vector3(-bounds.size.x * 0.5f - 50f, 0f, -transform.position.z);
            letterboxes[0].transform.localScale = new Vector3(100f, 200f + bounds.size.y, 1f);

            letterboxes[1].transform.position = bounds.center + // right
                new Vector3(bounds.size.x * 0.5f + 50f, 0f, -transform.position.z);
            letterboxes[1].transform.localScale = new Vector3(100f, 200f + bounds.size.y, 1f);

            letterboxes[2].transform.position = bounds.center + // top
                new Vector3(0f, bounds.size.y * 0.5f + 50f, -transform.position.z);
            letterboxes[2].transform.localScale = new Vector3(bounds.size.x, 100f, 1f);

            letterboxes[3].transform.position = bounds.center + // bottom
                new Vector3(0f, -bounds.size.y * 0.5f - 50f, -transform.position.z);
            letterboxes[3].transform.localScale = new Vector3(bounds.size.x, 100f, 1f);
        }

        transform.position = position;
    }
}
