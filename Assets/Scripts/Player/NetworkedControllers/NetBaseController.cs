
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class NetBaseController : NetworkBehaviour
{
    public Camera ControllerCamera;

    protected Rigidbody rigidBody;
    protected Animator animator;
    protected Transform camTrans;             // A reference to transform of the third person camera

    private float h, v;

    [SerializeField]
    protected float speed = 5f;

    [SerializeField]
    private float cameraDistance = 0f;

    protected virtual void OnEnable()
    {
        ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
    }

    protected virtual void OnDisable()
    {
        ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
    }

    protected virtual void ChangePOV_CameraChanged(Camera camera)
    {
        if (camera != this.ControllerCamera)
        {
            this.enabled = false;
            this.HideCamera(this.ControllerCamera);
        }
        else
        {
            this.ShowCamera(this.ControllerCamera);
        }
    }

    protected virtual void Start()
    {
        this.Init();
        this.SetCamera();
    }

    protected virtual void Init()
    {
        this.rigidBody = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponent<Animator>();
    }

    protected virtual void SetCamera()
    {
        if (!ControllerCamera) return;
        this.camTrans = this.ControllerCamera.transform;
        this.camTrans.position += this.cameraDistance * this.transform.forward;
    }

    protected virtual void UpdateAnimator(float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool walking = h != 0 || v != 0;
        // Tell the animator whether or not the player is walking.
        if(animator) animator.SetBool("IsWalking", walking);
    }

    // DO NOT OVERRIDE THIS LOOP
    private void FixedUpdate()
    {
        if(IsClient && IsOwner)
        {
            // Store the input axes.
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            // send input to the animator
            UpdateAnimator(h, v);
            // process client side logic to request move to the server
            MoveClient(h, v);
        }
        if (IsServer)
        {
            MoveServer();
        }
        
    }

    protected virtual void ShowCamera(Camera camera)
    {
        if (camera != null) { camera.gameObject.SetActive(true); }
    }

    protected virtual void HideCamera(Camera camera)
    {
        if (camera != null) { camera.gameObject.SetActive(false); }
    }

    // client side processing: Inputs -> Network Variables
    protected abstract void MoveClient(float h, float v);
    // server side processing: Network Variables -> Transforms and Rigidbodies
    protected abstract void MoveServer();
    // the network variables themselves are implementation details -> set in child classes
}
