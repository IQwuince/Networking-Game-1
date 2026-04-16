using Unity.Netcode;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [Header("Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerShot = 25;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float shootDistance = 100f;
    [SerializeField] private LayerMask hitMask = ~0;
    public int shots;

    [Header("References")]
    [SerializeField] private Camera ownerCamera; // local camera only
    [SerializeField] private CharacterController characterController;

    public NetworkVariable<int> Health = new NetworkVariable<int>(
        100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> Kills = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float _nextFireTime;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Health.Value = maxHealth;
            Kills.Value = 0;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + fireRate;

            if (ownerCamera == null) return;

            Vector3 origin = ownerCamera.transform.position;
            Vector3 direction = ownerCamera.transform.forward;

            shots++; 

            ShootServerRpc(origin, direction);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 origin, Vector3 direction, ServerRpcParams rpcParams = default)
    {
        // Optional: validate shooter is alive
        if (Health.Value <= 0) return;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootDistance, hitMask))
        {
            PlayerCombat target = hit.collider.GetComponentInParent<PlayerCombat>();
            if (target != null && target != this)
            {
                target.ApplyDamage(damagePerShot, OwnerClientId);
            }
        }
    }

    public void ApplyDamage(int amount, ulong attackerClientId)
    {
        if (Health.Value <= 0) return;

        Health.Value -= amount;

        if (Health.Value <= 0)
        {
            Health.Value = 0;

            // Give kill to attacker
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(attackerClientId, out var attackerClient))
            {
                var attackerCombat = attackerClient.PlayerObject.GetComponent<PlayerCombat>();
                if (attackerCombat != null)
                    attackerCombat.Kills.Value += 1;
            }

            // Respawn this player
            RespawnServer();
        }
    }

    private void RespawnServer()
    {
        Vector3 spawnPos = SpawnManager.Instance.GetRandomSpawn();
        transform.SetPositionAndRotation(spawnPos, Quaternion.identity);

        if (characterController != null)
        {
            // Small trick to avoid CC internal overlap issues on teleport
            characterController.enabled = false;
            characterController.enabled = true;
        }

        Health.Value = maxHealth;
    }
}