/*using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("FMOD Events Movement")]
    [EventRef] public string dashEvent;
    [EventRef] public string walkEvent;
    [EventRef] public string wallRunEvent;
    [EventRef] public string slidingEvent;
    [EventRef] public string landEvent;
    [EventRef] public string jumpEvent;
    [EventRef] public string sweetSpotEvent;

    [Header("FMOD Events Game")]
    [EventRef] public string dashPickUpEvent;
    [EventRef] public string checkpointEvent;
    [EventRef] public string respawnEvent;

    private float dashSoundCooldown = 0.5f; // Adjust this value for desired cooldown duration
    private float dashSoundCooldownTimer = 0f;

    private FMOD.Studio.EventInstance walkInstance;
    private FMOD.Studio.EventInstance wallRunInstance;
    private FMOD.Studio.EventInstance slidingInstance;

    private bool isWalking = false;
    private bool isWallRunning = false;
    private bool isSliding = false;

    private void Start()
    {
        walkInstance = RuntimeManager.CreateInstance(walkEvent);
        wallRunInstance = RuntimeManager.CreateInstance(wallRunEvent);
        slidingInstance = RuntimeManager.CreateInstance(slidingEvent);

        walkInstance.setPaused(true);
        wallRunInstance.setPaused(true);
        slidingInstance.setPaused(true);
    }
    private void Update()
    {
        if (dashSoundCooldownTimer > 0)
        {
            dashSoundCooldownTimer -= Time.deltaTime;
        }

        PlayDashAudio();

    }

    public void PlayDashAudio()
    {
        if (Input.GetKeyDown(KeyCode.E) && dashSoundCooldownTimer <= 0)
        {
            RuntimeManager.PlayOneShot(dashEvent, transform.position);
            dashSoundCooldownTimer = dashSoundCooldown; // Reset cooldown timer
        }
    }

    public void playWalkAudio()
    {
        if (!isWalking)
        {
            walkInstance.start();
            walkInstance.setPaused(false);
            isWalking = true;

        }

    }
    public void StopWalkAudio()
    {
        if (isWalking)
        {
            walkInstance.setPaused(true);
            isWalking = false;

        }
    }

    public void playWallRunAudio()
    {
        if (!isWallRunning)
        {
            wallRunInstance.start();
            wallRunInstance.setPaused(false);
            isWallRunning = true;
        }
    }

    public void StopWallRunAudio()
    {
        if (isWallRunning)
        {
            wallRunInstance.setPaused(true);
            isWallRunning = false;
        }
    }

    public void playSlidingAudio()
    {
        if (!isSliding)
        {
            slidingInstance.start();
            slidingInstance.setPaused(false);
            isSliding = true;

        }
    }

    public void stopSlidingAudio()
    {
        if (isSliding)
        {
            slidingInstance.setPaused(true);
            isSliding = false;
        }
    }

    public void playLandAudio()
    {
        RuntimeManager.PlayOneShot(landEvent, transform.position);
    }

    public void playJumpAudio()
    {
        RuntimeManager.PlayOneShot(jumpEvent, transform.position);
    }

    public void PlaySweetSpotAudio()
    {
        RuntimeManager.PlayOneShot(sweetSpotEvent, transform.position);
    }

    public void playDashPickUpAudio()
    {
        RuntimeManager.PlayOneShot(dashPickUpEvent, transform.position);
    }
    public void playCheckpointAudio()
    {
        RuntimeManager.PlayOneShot(checkpointEvent, transform.position);
    }

    public void playRespawnAudio()
    {
        RuntimeManager.PlayOneShot(respawnEvent, transform.position);
    }
}
*/