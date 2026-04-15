using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private List<DashPickup> pickups = new List<DashPickup>();

    public void RegisterPickup(DashPickup pickup)
    {
        pickups.Add(pickup);
    }

    public void RespawnAllPickups()
    {
        foreach (DashPickup pickup in pickups)
        {
            pickup.TriggerRespawn();
        }
    }
}
