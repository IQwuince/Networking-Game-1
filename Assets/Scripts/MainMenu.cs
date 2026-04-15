using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button serverButton = null;
    [SerializeField] private Button clientButton = null;

    private void Start()
    {
        #if !UNITY_EDITOR && UNITY_SERVER
        OnServerClicked();
        return;
        #endif
        serverButton.onClick.AddListener(OnServerClicked);
        clientButton.onClick.AddListener(OnClientClicked);
    }

    public void OnServerClicked()
    {
        ConnectionManager.singleton.InitializeAsServer(5678);
        SceneManager.LoadScene(1);
    }

    public void OnClientClicked()
    {
        ConnectionManager.singleton.InitializeAsClient("127.0.0.1", 5678);
        SceneManager.LoadScene(1);
    }

}