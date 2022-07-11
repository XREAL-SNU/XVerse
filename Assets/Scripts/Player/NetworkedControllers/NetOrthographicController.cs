
using Unity.Netcode;
using UnityEngine;

public class NetOrthographicController : NetThirdPersonController
{
    public float smoothing = 5f;        // The speed with which the camera will be following.
    private Vector3 offset;

    protected override void Init()
    {
        base.Init();
        GameObject ControllerCameraGo = GameObject.Find("OrthographicCamera");
        if(ControllerCameraGo) ControllerCameraGo.TryGetComponent<Camera>(out ControllerCamera);
    }

    protected override void SetCamera()
    {
        base.SetCamera();
        // Calculate the initial offset.
        offset = camTrans.position - transform.position;
    }

    // client side processing: Input -> Network variables.
    protected override void MoveClient(float h, float v)
    {
        // third person controller move
        base.MoveClient(h, v);
        // this is supposed to run for the client only, anyways.
        this.CameraFollow();
    }


    private void CameraFollow()
    {
        if (!ControllerCamera) return;
        // Create a postion the camera is aiming for based on the offset from the target.
        Vector3 targetCamPos = this.transform.position + this.offset;

        // Smoothly interpolate between the camera's current position and it's target position.
        camTrans.position = Vector3.Lerp(this.camTrans.position, targetCamPos, this.smoothing * Time.deltaTime);
    }

}

