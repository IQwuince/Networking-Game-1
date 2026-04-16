using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHudBillboard : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI shootCount; 
    [SerializeField] private PlayerCombat combat;
    public GameObject playerUI;

    private void Update()
    {
        if (IsOwner)
        {
            playerUI.SetActive(true);
            if (label == null || combat == null) return;
            label.text = $"HP: {combat.Health.Value}\nK: {combat.Kills.Value}";
            shootCount.text = $"Shots: {combat.shots}";

        }


    }
}