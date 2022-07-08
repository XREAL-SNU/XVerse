// ----------------------------------------------------------------------------
// <copyright file="ChangePOV.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// "Camera manager" class that handles the switch between the three different cameras.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value

namespace ExitGames.Demos.DemoPunVoice {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;


    public class ChangePOV : MonoBehaviour {
        private FirstPersonController firstPersonController;
        private OrthographicController orthographicController;

        private Vector3 initialCameraPosition;
        private Quaternion initialCameraRotation;
        private Camera defaultCamera;

        [SerializeField]
        private GameObject ButtonsHolder;

        [SerializeField]
        private Button FirstPersonCamActivator;

        [SerializeField]
        private Button OrthographicCamActivator;

        public delegate void OnCameraChanged(Camera newCamera);

        public static event OnCameraChanged CameraChanged;



        private void Start() {
            this.defaultCamera = Camera.main;
            this.initialCameraPosition = new Vector3(this.defaultCamera.transform.position.x,
                this.defaultCamera.transform.position.y, this.defaultCamera.transform.position.z);
            this.initialCameraRotation = new Quaternion(this.defaultCamera.transform.rotation.x,
                this.defaultCamera.transform.rotation.y, this.defaultCamera.transform.rotation.z,
                this.defaultCamera.transform.rotation.w);
            //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
            FirstPersonCamActivator = GameObject.Find("FirstPersonBtn").GetComponent<Button>();
            this.FirstPersonCamActivator.onClick.AddListener(this.FirstPersonMode);
#else
            this.FirstPersonCamActivator.gameObject.SetActive(false);
#endif
            OrthographicCamActivator = GameObject.Find("OrthoBtn").GetComponent<Button>();
            this.OrthographicCamActivator.onClick.AddListener(this.OrthographicMode);
        }

        private void OnCharacterInstantiated(GameObject character)
        {
            this.firstPersonController = character.GetComponent<FirstPersonController>();
            this.firstPersonController.enabled = false;
            this.orthographicController = character.GetComponent<OrthographicController>();
            this.ButtonsHolder.SetActive(true);
        }

        private void FirstPersonMode() {
            this.ToggleMode(this.firstPersonController);
        }


        private void OrthographicMode() {
            this.ToggleMode(this.orthographicController);
        }

        private void ToggleMode(BaseController controller) {
            if (controller == null) { return; } // this should not happen, throw error
            if (controller.ControllerCamera == null) { return; } // probably game is closing 
            controller.ControllerCamera.gameObject.SetActive(true);
            controller.enabled = true;
            this.FirstPersonCamActivator.interactable = !(controller == this.firstPersonController);
            this.OrthographicCamActivator.interactable = !(controller == this.orthographicController);
            this.BroadcastChange(controller.ControllerCamera); // BroadcastChange(Camera.main);
        }

        private void BroadcastChange(Camera camera) {
            if (camera == null) { return; } // should not happen, throw error
            if (CameraChanged != null) {
                CameraChanged(camera);
            }
        }

      
    }
}