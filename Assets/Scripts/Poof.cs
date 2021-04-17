using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poof : MonoBehaviour
{
    public SpriteAnimation[] particles;
    public float width = 1f;
    public float height = 0.1f;
    public int particleCount = 3;

    private SpriteAnimator[] instantiatedParticles;

    void OnEnable()
    {
        instantiatedParticles = new SpriteAnimator[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            var position = transform.position + new Vector3(
                Random.Range(-width / 2, width / 2),
                Random.Range(-height / 2, height / 2),
                0f);

            var animation = particles[Random.Range(0, particles.Length)];

            var obj = new GameObject($"Particle {i}");
            obj.transform.position = position;
            float angle = Mathf.Floor(Random.value * 4f) * 90f;
            obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            obj.transform.SetParent(transform);
            obj.AddComponent<SpriteRenderer>().sortingOrder = 100;
            instantiatedParticles[i] = obj.AddComponent<SpriteAnimator>();
            instantiatedParticles[i].Animation = animation;
        }
    }

    void LateUpdate()
    {
        bool isDone = true;

        for (int i = 0; i < instantiatedParticles.Length; i++)
        {
            if (instantiatedParticles[i].IsCompletedThisFrame)
                instantiatedParticles[i].gameObject.SetActive(false);
            else if (instantiatedParticles[i].isActiveAndEnabled)
                isDone = false;
        }

        if (isDone)
            GameObject.Destroy(gameObject);
    }
}
