using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XPlayer.Input
{
    [Serializable]
    public class PlayerInput
    {
        public string InputName;
        protected bool isActiveInput = true;
        public Action InputAction;
        public void InputLock() { isActiveInput = false; }
        public void InputUnLock() { isActiveInput = true; }

        public PlayerInput() { }
    }

    [Serializable]
    public class PlayerInputGroup<T> where T : PlayerInput
    {
        public string InputGroupName;
        public List<T> Inputs = new List<T>();

        public T GetInput(string inputName)
        {
            foreach (T input in Inputs)
            {
                if (input.InputName.Equals(inputName)) return input;
            }
            return null;
        }

        public void InputUnLockOnly(string inputName)
        {
            foreach (T input in Inputs)
            {
                if (input.InputName.Equals(inputName)) input.InputUnLock();
                else input.InputLock();
            }
        }
        public void InputLockOnly(string inputName)
        {
            foreach (T input in Inputs)
            {
                if (input.InputName.Equals(inputName)) input.InputLock();
                else input.InputUnLock();
            }
        }
        public void InputLockAll()
        {
            foreach (T input in Inputs)
            {
                input.InputLock();
            }
        }
        public void InputUnLockAll()
        {
            foreach (T input in Inputs)
            {
                input.InputUnLock();
            }
        }

    }
}
