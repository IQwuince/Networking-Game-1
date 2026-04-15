using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{

    [SerializeField] private Button _quitButton = null;
    [SerializeField] private TextMeshProUGUI _logText = null;
    [SerializeField] private int _maxLogEntries = 20;
    private List<string> _logEntries = new List<string>();

    private void Start()
    {
        _logText.text = "";
        _quitButton.onClick.AddListener(ReturnToMainMenu);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetConnectionData(ConnectionManager.singleton.connectionIP, ConnectionManager.singleton.connectionPort);
        if (ConnectionManager.singleton.connectionType == ConnectionManager.Type.Server)
        {
            NetworkManager.Singleton.StartServer();
        }
        else if (ConnectionManager.singleton.connectionType == ConnectionManager.Type.Client)
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        NetworkManager.Singleton.Shutdown();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Log($"[Server]: Client connected. ID:{clientId}", Color.green);
        }
        if (NetworkManager.Singleton.IsClient)
        {
            Log($"[Client]: Connected to server. ID: {clientId}", Color.green);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Log($"[Server]: Client disconnected. ID:{clientId}", Color.red);
        }
        if (NetworkManager.Singleton.IsClient)
        {
            Log($"[Client]: Disconnected from server.", Color.red);
        }
    }

    private void OnServerStarted()
    {
        Log("[Server]: Server started.", Color.cyan);
    }

    private void Log(string message, Color color)
    {

        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{timestamp}] {message}</color>";
        _logEntries.Insert(0, coloredMessage);
        if (_logEntries.Count > _maxLogEntries)
        {
            _logEntries.RemoveAt(_logEntries.Count - 1);
        }
        _logText.text = string.Join("\n", _logEntries);
    }

}
