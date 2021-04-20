using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform startPoint;
    public float respawnTime = 1f;
    public CameraController camera;
    public KeyDisplay keyDisplay;
    public string powerupTag = "Powerup";

    private Room[] rooms;
    private GameObject[] powerups;
    private Room currentRoom;
    public Room CurrentRoom => currentRoom;
    private Transform respawnPoint;
    private GameObject player;
    public GameObject Player => player;
    private float respawnTimer = 0f;
    private int keys = 0;

    void Start()
    {
        respawnPoint = startPoint;
        rooms = GameObject.FindObjectsOfType<Room>();
        powerups = GameObject.FindGameObjectsWithTag(powerupTag);
        RespawnPlayer();
    }

    void Update()
    {
        if (player == null && respawnTimer <= 0f)
            respawnTimer = respawnTime;

        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
                RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        var playerController = player.GetComponent<PlayerController>();
        playerController.onKeyPickup += OnKeyPickup;
        playerController.onUnlock += OnUnlock;

        foreach (var powerup in powerups)
            powerup.gameObject.SetActive(true);

        camera.target = player.transform;
    }

    void OnKeyPickup(Collider2D keyCollider)
    {
        keyDisplay.KeyCount++;
        GameObject.Destroy(keyCollider.gameObject);
    }

    void OnUnlock(Collider2D lockCollider)
    {
        if (keyDisplay.KeyCount > 0)
        {
            lockCollider.GetComponent<Lock>().Unlock();
            keyDisplay.KeyCount--;
        }
    }

    void LateUpdate()
    {
        if (player != null &&
            (currentRoom == null || !currentRoom.Bounds.Contains(player.transform.position)))
        {
            foreach (var room in rooms)
            {
                if (room == currentRoom)
                    continue;

                if (room.Bounds.Contains(player.transform.position))
                {
                    foreach (var respawnPoint in room.RespawnPoints)
                    {
                        if (respawnPoint.previousRoom == currentRoom)
                            this.respawnPoint = respawnPoint.transform;
                    }

                    currentRoom = room;
                    camera.bounds = room.Bounds;
                }
            }
        }
    }
}
