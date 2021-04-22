using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSequenceTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public System.Action onPlayerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("hey");
            onPlayerEnter?.Invoke();
        }
    }
}
