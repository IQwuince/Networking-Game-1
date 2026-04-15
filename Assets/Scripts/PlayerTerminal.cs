using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTerminal : NetworkBehaviour
{

    private static PlayerTerminal _singleton = null; public static PlayerTerminal singleton { get { return _singleton; } }

    protected override void OnNetworkPostSpawn()
    {
        _singleton = this;
        base.OnNetworkPostSpawn();
        if (IsClient)
        {
            SpawnPlayerRpc();
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnPlayerRpc(RpcParams rpcParams = default)
    {
        ulong id = rpcParams.Receive.SenderClientId;
        var prefab = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[0].PrefabList[0].Prefab;
        Transform spawnPoint = SpawnPoints.singleton.GetPointInOrder();
        var player = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
    }

}