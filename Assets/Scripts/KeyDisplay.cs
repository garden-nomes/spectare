using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private Sprite keySprite;
    [SerializeField] private float offset = 8f;

    public int KeyCount
    {
        get => keyImageInstances.Count;
        set => UpdateInstances(value);
    }

    List<GameObject> keyImageInstances;

    void Start()
    {
        keyImageInstances = new List<GameObject>();
    }

    void UpdateInstances(int count)
    {
        while (count > keyImageInstances.Count)
        {
            var instance = new GameObject($"Key Image {keyImageInstances.Count}");
            instance.transform.SetParent(transform);

            var image = instance.AddComponent<Image>();
            image.sprite = keySprite;

            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.pivot = new Vector2(0f, 0f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);
            rectTransform.anchoredPosition = new Vector2(8f, 8f + keyImageInstances.Count * offset);

            rectTransform.sizeDelta = image.sprite.rect.size;

            keyImageInstances.Add(instance);
        }

        while (count < keyImageInstances.Count)
        {
            var top = keyImageInstances[keyImageInstances.Count - 1];
            keyImageInstances.Remove(top);
            GameObject.Destroy(top);
        }
    }
}
