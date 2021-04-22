using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float closeDistance = 15f;
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private Collider2D collider;

    private EyeAnimator eyeAnimator;
    private PupilLookAt pupilLookAt;
    private Room room;
    private GameManager gameManager;
    private bool isBonked = false;
    private Vector3 initialPosition;
    private AudioSource audioSource;

    void Start()
    {
        eyeAnimator = GetComponent<EyeAnimator>();
        pupilLookAt = GetComponentInChildren<PupilLookAt>();
        room = GetComponentInParent<Room>();
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (room != null && gameManager != null && gameManager.CurrentRoom != room)
        {
            transform.position = initialPosition;

            if (eyeAnimator.State == EyeAnimatorState.Open)
                eyeAnimator.Close();

            return;
        }

        collider.enabled = eyeAnimator.State == EyeAnimatorState.Open;

        var player = gameManager == null ? null : gameManager.Player;

        if (player == null)
            return;

        pupilLookAt.target = player.transform.position;

        var toTarget = player.transform.position - transform.position;

        if (eyeAnimator.State == EyeAnimatorState.Closed)
        {
            if (isBonked)
                GameObject.Destroy(gameObject);
            else if (toTarget.sqrMagnitude < closeDistance * closeDistance)
            {
                eyeAnimator.Open();
                audioSource.PlayOneShot(openSound);
            }
        }
        else if (eyeAnimator.State == EyeAnimatorState.Open)
        {
            if (toTarget.sqrMagnitude > closeDistance * closeDistance)
                eyeAnimator.Close();

            float step = speed * Time.deltaTime;

            if (toTarget.sqrMagnitude < step * step)
                transform.position = player.transform.position;
            else
                transform.position += toTarget.normalized * step;
        }
    }

    public void Bonk()
    {
        eyeAnimator.Close();
        isBonked = true;
    }
}
