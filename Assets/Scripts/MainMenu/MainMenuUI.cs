using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XVerse.MainMenuUI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private InputField nicknameInputField;

        public void OnClickLoginButton()
        {
            Debug.Log("Click Login");
            if(nicknameInputField.text != "")
            {
                PlayerSettings.PlayerSetting.nickname = nicknameInputField.text;
            }
        }

        public void OnClickQuitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
