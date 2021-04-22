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

    public float endingSequenceBeat = 3f;
    public float endingSequenceTime = 3f;
    public Vector3 endingSequencePan = new Vector3(0f, 30f, 0f);

    private Room[] rooms;
    private GameObject[] powerups;
    private Room currentRoom;
    public Room CurrentRoom => currentRoom;
    private Vector3 respawnPoint;
    private GameObject player;
    public GameObject Player => player;
    private float respawnTimer = 0f;
    private int keys = 0;
    private bool isEndingSequenceStarted;

    void Start()
    {
        respawnPoint = startPoint.position;
        rooms = GameObject.FindObjectsOfType<Room>();
        powerups = GameObject.FindGameObjectsWithTag(powerupTag);
        isEndingSequenceStarted = false;

        foreach (var trigger in GameObject.FindObjectsOfType<EndingSequenceTrigger>())
            trigger.onPlayerEnter += OnEndingSequenceTriggered;

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
        player = Instantiate(playerPrefab, respawnPoint, Quaternion.identity);

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
                    var respawnPoint_ = room.GetComponentInChildren<RespawnPoint>();
                    if (respawnPoint_ != null)
                        respawnPoint = respawnPoint_.transform.position;

                    currentRoom = room;
                    camera.bounds = room.Bounds;
                }
            }
        }
    }

    void OnEndingSequenceTriggered()
    {
        if (!isEndingSequenceStarted)
            StartCoroutine(EndingSequenceCoroutine());
    }

    IEnumerator EndingSequenceCoroutine()
    {
        isEndingSequenceStarted = true;
        camera.enabled = false;
        Vector3 initialPosition = camera.transform.position;

        yield return new WaitForSeconds(endingSequenceBeat);

        for (float t = 0f; t < 1f; t += Time.deltaTime / endingSequenceTime)
        {
            float smoothStep = t * t * (3f - 2f * t);
            camera.transform.position = initialPosition + endingSequencePan * smoothStep;
            yield return null;
        }
    }
}
