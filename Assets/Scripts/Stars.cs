using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    public Sprite[] starSprites;
    public float density = 0.01f;
    public float width = 20f;
    public float height = 20f;
    public float parallax = 0.5f;
    public float twinkleRate = 1f;

    private Vector3 initialPosition;
    private SpriteRenderer[] stars;
    private float[] starParallaxes;
    private Vector3[] starPositions;

    void Start()
    {
        initialPosition = transform.position;

        int starCount = Mathf.FloorToInt(width * height * density);
        stars = new SpriteRenderer[starCount];
        starParallaxes = new float[starCount];
        starPositions = new Vector3[starCount];

        for (int i = 0; i < starCount; i++)
        {
            starPositions[i] = new Vector3(
                Mathf.Round(Random.Range(-width / 2f, width / 2f) * 8f) / 8f,
                Mathf.Round(Random.Range(-height / 2f, height / 2f) * 8f) / 8f,
                0f);
        }

        var obj = new GameObject();
        obj.transform.SetParent(transform);
        obj.transform.position = transform.position + starPositions[0];

        stars[0] = obj.AddComponent<SpriteRenderer>();
        stars[0].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        stars[0].sprite = starSprites[Random.Range(0, starSprites.Length)];
        stars[0].sortingOrder = -100;
        starParallaxes[0] = Random.value;

        for (int i = 1; i < starCount; i++)
        {
            stars[i] = Instantiate(stars[0], transform.position + starPositions[i], Quaternion.identity, transform);
            stars[i].sprite = starSprites[Random.Range(0, starSprites.Length)];
            starParallaxes[i] = Random.value;
        }
    }

    void LateUpdate()
    {
        Vector3 toCamera = Camera.main.transform.position - initialPosition;

        for (int i = 0; i < stars.Length; i++)
        {
            if (Random.value < twinkleRate * Time.deltaTime)
                stars[i].sprite = starSprites[Random.Range(0, starSprites.Length)];

            stars[i].transform.position =
                initialPosition +
                starPositions[i] +
                toCamera * parallax * starParallaxes[i];
        }
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
