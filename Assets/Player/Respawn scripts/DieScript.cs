using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    //public PlayerAudio playerAudio;

    private void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.Die();
        //playerAudio.playRespawnAudio();
    }
}
