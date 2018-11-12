using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class CreationTeamButton : MonoBehaviour
    {
        public InputField Input;
        public Button Button;
        public TeamScrollList teamScrollList;

        void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(Input, "Input need to be set");
            Assert.IsNotNull(Button, "Button need to be set");

            Button.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            if (!string.IsNullOrEmpty(Input.text))
                teamScrollList.AddTeam(Input.text);
        }
    }
}
