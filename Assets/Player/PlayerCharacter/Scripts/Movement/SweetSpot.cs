using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetSpot : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;
    public Transform orientation;
    public Transform playerCam;

    [Header("Boosting")]
    public float boostForce;
    public float boostAirMultiplier;

    private float oldJumpForce;
    private float oldAirMultiplier;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();

        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is inside the sweet spot
        if (other.CompareTag("Sweet Spot"))
        {
                BoostingPlayer();    
        }
    }

    private void OnTriggerExit(Collider other)
    { 
        // Check if the player is outside the sweet spot
        if (other.CompareTag("Sweet Spot"))
        {
            StopBoost();
        }
    }

    private void BoostingPlayer()
    {
        pm.boosting = true;
        
        oldJumpForce = pm.jumpForce;
        oldAirMultiplier = pm.airMultiplier;

        pm.jumpForce = boostForce;
        pm.airMultiplier = boostAirMultiplier;  


        //Debug.Log("Boosting");
    }

    private void StopBoost()
    {
        pm.boosting = false;
        pm.jumpForce = oldJumpForce;
        pm.airMultiplier = oldAirMultiplier;
    }

}
