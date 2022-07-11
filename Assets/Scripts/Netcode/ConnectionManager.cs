using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : NetworkBehaviour
{
    protected static ConnectionManager _instance;
    public static ConnectionManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance is null) _instance = this;
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    void OnClientConnected(ulong id)
    {
        if (IsClient)
        {
            Debug.Log($"Im a client: Connected as {id}");
        }
    }

    void OnClientDisconnected(ulong id)
    {
        if (IsClient)
        {
            Debug.Log($"Im a client: DisConnected as {id}");
        }
    }


}
