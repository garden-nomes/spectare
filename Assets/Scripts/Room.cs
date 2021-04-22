using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        Room room = (Room) target;
        Bounds bounds = room.Bounds;
        Vector3 leftHandle = room.Bounds.center + Vector3.left * room.Bounds.size.x * 0.5f;
        Vector3 rightHandle = room.Bounds.center + Vector3.right * room.Bounds.size.x * 0.5f;
        Vector3 topHandle = room.Bounds.center + Vector3.up * room.Bounds.size.y * 0.5f;
        Vector3 bottomHandle = room.Bounds.center + Vector3.down * room.Bounds.size.y * 0.5f;

        EditorGUI.BeginChangeCheck();

        leftHandle = Handles.FreeMoveHandle(leftHandle, Quaternion.identity, 0.5f, Vector3.one, Handles.RectangleHandleCap);
        rightHandle = Handles.FreeMoveHandle(rightHandle, Quaternion.identity, 0.5f, Vector3.one, Handles.RectangleHandleCap);
        topHandle = Handles.FreeMoveHandle(topHandle, Quaternion.identity, 0.5f, Vector3.one, Handles.RectangleHandleCap);
        bottomHandle = Handles.FreeMoveHandle(bottomHandle, Quaternion.identity, 0.5f, Vector3.one, Handles.RectangleHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Change Room Bounds");

            leftHandle.x = Mathf.Round(leftHandle.x);
            rightHandle.x = Mathf.Round(rightHandle.x);
            topHandle.y = Mathf.Round(topHandle.y);
            bottomHandle.y = Mathf.Round(bottomHandle.y);

            Vector3 position = room.transform.position;
            position.x = (leftHandle.x + rightHandle.x) * 0.5f;
            position.y = (topHandle.y + bottomHandle.y) * 0.5f;
            Vector3 delta = room.transform.position - position;
            room.transform.position = position;

            foreach (Transform child in room.transform)
            {
                child.position += delta;
            }

            room.width = Mathf.Abs(leftHandle.x - rightHandle.x);
            room.height = Mathf.Abs(topHandle.y - bottomHandle.y);
        }
    }
}
#endif

public class Room : MonoBehaviour
{
    public float width;
    public float height;
    public bool isFinalRoom = false;

    public Bounds Bounds =>
        new Bounds(transform.position, new Vector3(width, height, 0f));

    private RespawnPoint[] respawnPoints;
    public RespawnPoint[] RespawnPoints => respawnPoints;

    void Start()
    {
        respawnPoints = GetComponentsInChildren<RespawnPoint>();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Bounds.min, Bounds.min + Vector3.right * Bounds.size.x);
        Gizmos.DrawLine(Bounds.min + Vector3.right * Bounds.size.x, Bounds.max);
        Gizmos.DrawLine(Bounds.max, Bounds.max - Vector3.right * Bounds.size.x);
        Gizmos.DrawLine(Bounds.max - Vector3.right * Bounds.size.x, Bounds.min);
    }
#endif
}
