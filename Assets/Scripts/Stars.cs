using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public Sprite[] starSprites;
    public int starCount = 100;
    public float width = 20f;
    public float height = 20f;
    public float parallax = 0.5f;
    public Transform cameraTransform;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;

        var obj = new GameObject();
        var star = obj.AddComponent<SpriteRenderer>();
        star.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        star.sortingOrder = -100;

        for (int i = 0; i < starCount; i++)
        {
            Vector3 position = new Vector3(
                Mathf.Round(Random.Range(-width / 2f, width / 2f) * 8f) / 8f,
                Mathf.Round(Random.Range(-height / 2f, height / 2f) * 8f) / 8f,
                0f);
            var sprite = starSprites[Random.Range(0, starSprites.Length)];
            var instance = Instantiate(star, transform.position + position, Quaternion.identity, transform);
            instance.sprite = sprite;
        }
    }

    void LateUpdate()
    {
        Vector3 toCamera = cameraTransform.position - initialPosition;
        transform.position = initialPosition + toCamera * parallax;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 a = transform.position + new Vector3(-width / 2f, -height / 2f);
        Vector3 b = transform.position + new Vector3(width / 2f, -height / 2f);
        Vector3 c = transform.position + new Vector3(width / 2f, height / 2f);
        Vector3 d = transform.position + new Vector3(-width / 2f, height / 2f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
#endif
}
