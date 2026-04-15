using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //public PlayerAudio PlayerAudio;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SavePosition = other.transform.position;
       // PlayerAudio.playCheckpointAudio();
    }
}
