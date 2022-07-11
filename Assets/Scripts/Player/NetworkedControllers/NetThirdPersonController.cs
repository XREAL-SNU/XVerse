

using Unity.Netcode;
using UnityEngine;

public class NetThirdPersonController : NetBaseController {

    [SerializeField]
    protected float movingTurnSpeed = 360;

    [SerializeField]
    protected NetworkVariable<Vector3> targetVelocity = new NetworkVariable<Vector3>();
    
    [SerializeField]
    protected NetworkVariable<Quaternion> rotationDelta = new NetworkVariable<Quaternion>();


    protected override void MoveClient(float h, float v) {

        // do the heavy calculations here
        Vector3 velocity = v * speed * transform.forward;
        Quaternion rotationDelta = Quaternion.AngleAxis(movingTurnSpeed * h * Time.deltaTime, Vector3.up);

        UpdateClientTransformServerRpc(velocity, rotationDelta);
    }

    // this runs every fixed update in the server!
    // based on the network variables that have been set by the RPC.
    protected override void MoveServer()
    {
        // do smallest calculation here.
        rigidBody.velocity = targetVelocity.Value;
        transform.rotation *= rotationDelta.Value;
    }

    [ServerRpc]
    protected void UpdateClientTransformServerRpc(Vector3 vel, Quaternion rotDel)
    {

        // this runs inside server
        targetVelocity.Value = vel;
        rotationDelta.Value = rotDel;
    }
}

