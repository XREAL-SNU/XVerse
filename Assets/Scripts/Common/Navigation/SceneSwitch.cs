using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : NetworkBehaviour
{
    [SerializeField]
    private string Current = "NetLobby";
    [SerializeField]
    public string Target = "NetOpenSpace";

    void Start()
    {
        Current = SceneManager.GetActiveScene().name;
    }


    public void Switch()
    {
        DetermineTargetScene();
        if (IsClient)
        {
            ClientSwitchSceneServerRpc(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    void DetermineTargetScene()
    {
        NetworkEndPoint ep;
        if (Current.Equals("NetLobby"))
        {
            Target = "NetOpenSpace";
            NetworkEndPoint.TryParse("127.0.0.1", 7778, out ep);
        }
        else if (Current.Equals("NetOpenSpace"))
        {
            Target = "NetCorpScene";
            NetworkEndPoint.TryParse("127.0.0.1", 7777, out ep);
        }
        else if (Current.Equals("NetCorpScene"))
        {
            Target = "NetOpenSpace";
            NetworkEndPoint.TryParse("127.0.0.1", 7778, out ep);
        }
        else
        {
            ep = NetworkEndPoint.LoopbackIpv4;

        }
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ep);

    }

    [ServerRpc(RequireOwnership = false)]
    void ClientSwitchSceneServerRpc(ulong clientId)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
    }
    
    void OnClientDisconnected(ulong clientId)
    {
        if (IsClient)
        {
#if UNITY_EDITOR
            EditorSceneManager.LoadScene(Target);
#else
            SceneManager.LoadScene(Target);

#endif
            SceneManager.sceneLoaded += AfterSceneLoaded;
        }
    }

    public void AfterSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Current = scene.name;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        SceneManager.sceneLoaded -= AfterSceneLoaded;

        // connect to service again!
        Debug.Log("Connecting to server again!");
        NetworkManager.Singleton.StartClient();
    }



}
