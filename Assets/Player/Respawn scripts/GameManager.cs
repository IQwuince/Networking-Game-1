using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    //other scripts can globally access via GameManager.Instance without needing a reference
    public static GameManager Instance { get; private set; }

    public GameObject Player;
    public Rigidbody PlayerRB;

    public Vector3 SavePosition;
    public float dashCount = 2;
    public float maxDashes = 99;
    private float dashCountStart;

    public TextMeshProUGUI textMeshProUGUI;

    private RespawnManager respawnManager;
    public PlayerMovementAdvanced pm;

    //UI management

    private ParticleSystem.MinMaxCurve speedStart;
    private ParticleSystem.MinMaxCurve spawnRate;
    private bool looping;

    public ParticleSystem sl;
    private ParticleSystem.EmissionModule emissionSl;

    private void Start()
    {
        respawnManager = FindObjectOfType<RespawnManager>();

        if (respawnManager == null)
        {
            Debug.LogError("RespawnManager not found in the scene!");
        }
        textMeshProUGUI.text = $"Dashes: {dashCount}";
        dashCountStart = dashCount;

        emissionSl = sl.emission;

        emissionSl.rate = 0f;
    }

    private void Awake()
    {
        //ensure there's only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UseDash()
    {
        if (dashCount <= -1)
        {
            dashCount = 0;
            textMeshProUGUI.text = $"Dashes: {dashCount}";
        }
        else
        {
            dashCount--;
            textMeshProUGUI.text = $"Dashes: {dashCount}";
        }
    }

    public void DashPickup()
    {
        if (dashCount < maxDashes)
        {
            dashCount++;
            textMeshProUGUI.text = $"Dashes: {dashCount}";
        }
    }

    public void Die()
    {
        Player.transform.position = SavePosition;
        pm.moveSpeed = pm.walkSpeed;
        PlayerRB.linearVelocity = new Vector3(0,0,0);
        respawnManager.RespawnAllPickups();
        dashCount = dashCountStart;
        textMeshProUGUI.text = $"Dashes: {dashCount}";
    }

    //UI functions
    public void StartingSpeed()
    {
        emissionSl.rate = 0f;
    }
    public void SlowSpeed()
    {
        emissionSl.rate = 10.0f;
    }

    public void MediumSpeed()
    {
        emissionSl.rate = 50.0f;
    }

    public void FastSpeed()
    {
        emissionSl.rate = 125.0f;
    }
}
