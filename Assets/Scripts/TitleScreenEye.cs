using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenEye : MonoBehaviour
{
    public float movePupilTime = .5f;
    public float lookAroundFrequency = 3f;
    public float shiftPositionFrequency = 5f;
    public float minRadius = 4f;
    public Vector3 repositionOffset;
    public Vector2 repositionSize;
    public float repositionPadding = 2f;

    private EyeAnimator eyeAnimator;
    private PupilLookAt pupilLookAt;
    private Vector3 lookAt;
    private TitleScreenEye[] others;

    void Start()
    {
        eyeAnimator = GetComponent<EyeAnimator>();
        others = GameObject.FindObjectsOfType<TitleScreenEye>();
        pupilLookAt = GetComponentInChildren<PupilLookAt>();
        lookAt = transform.position + (Vector3) Random.insideUnitCircle.normalized * 5f;
        Reposition();
    }

    void Update()
    {
        if (eyeAnimator.State == EyeAnimatorState.Closed)
        {
            if (Random.value < Time.deltaTime / shiftPositionFrequency)
            {
                Reposition();
                eyeAnimator.Open();
            }
        }

        if (Random.value < Time.deltaTime / lookAroundFrequency)
            ChangeLookAt();

        if (Random.value < Time.deltaTime / shiftPositionFrequency)
            eyeAnimator.Close();

        pupilLookAt.target = lookAt;
    }

    void Reposition()
    {
        bool isValid = false;
        int tries = 0;

        while (!isValid)
        {
            var cameraPosition = Camera.main.transform.position;
            float x = cameraPosition.x + (Random.value - .5f) * (repositionSize.x - repositionPadding * 2f);
            float y = cameraPosition.y + (Random.value - .5f) * (repositionSize.y - repositionPadding * 2f);
            transform.position = new Vector3(x, y, 0f) + repositionOffset;

            tries++;
            isValid = true;

            if (tries < 20)
            {
                foreach (var other in others)
                {
                    if (this != other &&
                        (transform.position - other.transform.position).sqrMagnitude < minRadius * minRadius)
                    {
                        isValid = false;
                    }
                }
            }
        }
    }

    void ChangeLookAt()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeLookAtCoroutine());
    }

    IEnumerator ChangeLookAtCoroutine()
    {
        Vector3 previousLookAt = lookAt;
        Vector3 nextLookAt = transform.position + (Vector3) Random.insideUnitCircle * 5f;

        for (float t = 0f; t < 1f; t += Time.deltaTime / movePupilTime)
        {
            float smoothStep = t * t * (3f - 2f * t);
            lookAt = Vector3.Lerp(previousLookAt, nextLookAt, smoothStep);
            yield return null;
        }

        lookAt = nextLookAt;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(lookAt, .25f);
    }
#endif
}
