// ----------------------------------------------------------------------------
// <copyright file="ThirdPersonController.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Third person character controller class.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------



using UnityEngine;

public class ThirdPersonController : BaseController {

    [SerializeField]
    protected float movingTurnSpeed = 360;

    protected override void Move(float h, float v) {
        this.rigidBody.velocity = v * this.speed * this.transform.forward;
        this.transform.rotation *= Quaternion.AngleAxis(this.movingTurnSpeed * h * Time.deltaTime, Vector3.up);
    }
}

