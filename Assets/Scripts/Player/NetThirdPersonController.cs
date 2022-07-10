

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
        Debug.Log($"run MoveClient (h{h} v{v})");

        // do the heavy calculations here
        Vector3 velocity = v * speed * transform.forward;
        Quaternion rotationDelta = Quaternion.AngleAxis(movingTurnSpeed * h * Time.deltaTime, Vector3.up);

        UpdateClientTransformServerRpc(velocity, rotationDelta);
    }

    // this runs every fixed update in the server!
    // based on the network variables that have been set by the RPC.
    protected override void MoveServer()
    {
        Debug.Log($"run MoveServer(targetVel = {targetVelocity.Value})");
        // do smallest calculation here.
        rigidBody.velocity = targetVelocity.Value;
        transform.rotation *= rotationDelta.Value;
    }

    [ServerRpc]
    protected void UpdateClientTransformServerRpc(Vector3 vel, Quaternion rotDel)
    {
        Debug.Log($"run ServerRpc ()");

        // this runs inside server
        targetVelocity.Value = vel;
        rotationDelta.Value = rotDel;
    }
}

