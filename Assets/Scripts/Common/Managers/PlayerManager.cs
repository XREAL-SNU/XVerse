//using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    protected static PlayerManager _instance;
    public static PlayerManager Instance{
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance is null) _instance = this;
    }

    

    private NetworkVariable<int> playersInGame = new NetworkVariable<int>(readPerm: NetworkVariableReadPermission.Everyone);
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log($"{id} connected");
                playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log($"{id} disconnected");
                playersInGame.Value--;
            }
        };


    }
}
