using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPlayer.Input.InputManager;
using System;

public class InputTest : MonoBehaviour
{
    private void Start()
    {
        XInput.Instance.SetInputSetting("");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log(XInput.Instance.PlayerInputSetting.InputSettingName);
        }
        if (XInput.Instance.KeyInput("Front"))
        {
            Debug.Log("go front");
            XInput.Instance.InputUnLockAll();
        }
        
        if (XInput.Instance.KeyInput("Jump"))
        {
            Debug.Log("Jump!");
        }

        if (XInput.Instance.KeyInput("Back"))
        {
            Debug.Log("go back");
            XInput.Instance.KeyInputLockOnly("Jump", false);
        }
    }
}
