using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilLookAt : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bounds;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager != null && gameManager.Player != null)
        {
            Vector3 toTarget = (gameManager.Player.transform.position - transform.parent.position).normalized;
            toTarget.x *= bounds.bounds.size.x * 0.5f;
            toTarget.y *= bounds.bounds.size.y * 0.5f;
            toTarget.x = Mathf.Round(toTarget.x * 8f) / 8f;
            toTarget.y = Mathf.Round(toTarget.y * 8f) / 8f;
            transform.localPosition = toTarget;
        }
    }
}
