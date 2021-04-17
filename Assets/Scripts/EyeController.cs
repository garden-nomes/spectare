using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float closeDistance = 15f;
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private Collider2D collider;

    private EyeAnimator eyeAnimator;
    private Room room;
    private GameManager gameManager;
    private bool isBonked = false;

    void Start()
    {
        eyeAnimator = GetComponent<EyeAnimator>();
        room = GetComponentInParent<Room>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (room != null && gameManager != null && gameManager.CurrentRoom != room)
        {
            if (eyeAnimator.State == EyeAnimatorState.Open)
                eyeAnimator.Close();

            return;
        }

        collider.enabled = eyeAnimator.State == EyeAnimatorState.Open;

        var player = gameManager == null ? null : gameManager.Player;
        if (player == null)
            return;

        var toTarget = player.transform.position - transform.position;
        if (eyeAnimator.State == EyeAnimatorState.Closed)
        {
            if (isBonked)
                GameObject.Destroy(gameObject);
            else if (toTarget.sqrMagnitude < closeDistance * closeDistance)
                eyeAnimator.Open();
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
