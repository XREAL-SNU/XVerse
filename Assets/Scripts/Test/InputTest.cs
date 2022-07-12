using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XVerse.Player.Input;

public class InputTest : MonoBehaviour
{
    private void Awake()
    {
        XInput.Instance.SetInputSetting("XVerse Default");
    }
    private void Update()
    {
        if (XInput.Instance.KeyInput("hi")) { Debug.Log("Front"); }
    }
}
