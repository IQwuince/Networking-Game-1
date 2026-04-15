using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPickup : MonoBehaviour
{
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    private bool shouldRespawn = false;
    //public PlayerAudio playerAudio;

    private void Start()
    {
        // Cache components
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Register with the RespawnManager
        FindAnyObjectByType<RespawnManager>().RegisterPickup(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.dashCount >= GameManager.Instance.maxDashes)
        {
            // Do nothing if dash count is maxed
        }
        else
        {
            GameManager.Instance.DashPickup();
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            Debug.Log($"{gameObject.name}: Disabled the collider and mesh renderer");

           // playerAudio.playDashPickUpAudio();
        }
    }

    public void TriggerRespawn()
    {
        // Mark this pickup for respawn
        shouldRespawn = true;
    }

    private void Update()
    {
        if (shouldRespawn)
        {
            DoRespawn();
        }
    }

    private void DoRespawn()
    {
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
        shouldRespawn = false;
        Debug.Log($"{gameObject.name}: Enabled the collider and mesh renderer");
    }
}
