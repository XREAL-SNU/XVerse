using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerString : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    // flag, false if player name field has not yet been set.
    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // set the playerName.
            playerName.Value = $"Player {OwnerClientId}";
        }

    }
    private void SetOverlay()
    {
        Text localPlayerOverlay = GetComponentInChildren<Text>();
        localPlayerOverlay.text = playerName.Value;
    }

    private void Update()
    {
        // expensive to do in Update..
        if (!overlaySet && !string.IsNullOrEmpty(playerName.Value)){
            SetOverlay();
            overlaySet = true;
        }
    }
}

