
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class BaseController : MonoBehaviour
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
        this.camTrans = this.ControllerCamera.transform;
        this.camTrans.position += this.cameraDistance * this.transform.forward;
    }

    protected virtual void UpdateAnimator(float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool walking = h != 0 || v != 0;
        // Tell the animator whether or not the player is walking.
        this.animator.SetBool("IsWalking", walking);
    }

    protected virtual void FixedUpdate()
    {
        // Store the input axes.
        this.h = Input.GetAxisRaw("Horizontal");
        this.v = Input.GetAxisRaw("Vertical");

        // send input to the animator
        this.UpdateAnimator(this.h, this.v);
        // Move the player around the scene.
        this.Move(this.h, this.v);
    }

    protected virtual void ShowCamera(Camera camera)
    {
        if (camera != null) { camera.gameObject.SetActive(true); }
    }

    protected virtual void HideCamera(Camera camera)
    {
        if (camera != null) { camera.gameObject.SetActive(false); }
    }

    protected abstract void Move(float h, float v);
}
