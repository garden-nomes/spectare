using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform startPoint;
    public float respawnTime = 1f;
    public CameraController camera;

    private Room[] rooms;
    private Room currentRoom;
    public Room CurrentRoom => currentRoom;
    private Transform respawnPoint;
    private GameObject player;
    public GameObject Player => player;
    private float respawnTimer = 0f;

    void Start()
    {
        player = Instantiate(playerPrefab, startPoint.position, Quaternion.identity);
        camera.target = player.transform;
        respawnPoint = startPoint;
        rooms = GameObject.FindObjectsOfType<Room>();
    }

    void Update()
    {
        if (player == null && respawnTimer <= 0f)
            respawnTimer = respawnTime;

        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
            {
                player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
                camera.target = player.transform;
            }
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
