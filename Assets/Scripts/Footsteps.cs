using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footsteps;
    public float rate = 0.25f;
    [Range(0f, 1f)] public float volume = 1f;

    private float timer = 0f;
    private PlayerController playerController;
    private AudioSource audioSource;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.IsGrounded && playerController.velocity.x != 0)
        {
            timer += Time.deltaTime;
            if (timer >= rate)
            {
                audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], volume);
                timer = 0f;
            }
        }
        else
            timer = 0f;
    }
}
