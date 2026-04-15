/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private PlayerMovementAdvanced pm;

    private void Update()
    {
        StateHandler();
    }
    private void StateHandler()
    {
        // Idle
        if (pm.idle)
        {

            playerAudio.StopWallRunAudio();
            playerAudio.StopWalkAudio();
            playerAudio.stopSlidingAudio();
        }

        // sliding
        else if (pm.sliding)
        {
            playerAudio.StopWalkAudio();
            playerAudio.StopWallRunAudio();

            playerAudio.playSlidingAudio();
        }
        // dashing
        else if (pm.dashing)
        {
            playerAudio.StopWalkAudio();
            playerAudio.StopWallRunAudio();
            playerAudio.stopSlidingAudio();
        }
        // wall running
        else if (pm.wallrunning)
        {
            playerAudio.stopSlidingAudio();
            playerAudio.StopWalkAudio();

            playerAudio.playWallRunAudio();
        }

        // walking
        else if (pm.grounded && (pm.horizontalInput != 0 || pm.verticalInput != 0))
        {
            playerAudio.StopWallRunAudio();
            playerAudio.stopSlidingAudio();

            playerAudio.playWalkAudio();
        }

        // air
        else
        {
            playerAudio.StopWalkAudio();
            playerAudio.stopSlidingAudio();
            playerAudio.StopWallRunAudio();
        }

    }
}
*/