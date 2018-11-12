using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace MIG
{
    public class Connector : SceneController
    {
        public Button Connection_Button;
        public InputField Connection_InputField;

        protected override void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(Connection_Button, "Button connection not set");
            Assert.IsTrue(Connection_Button.IsActive(), "Button connection not active");
            Assert.IsNotNull(Connection_InputField, "Inputfield connection not set");
            Assert.IsTrue(Connection_InputField.IsActive(), "Inputfield connection not active");
            Assert.IsNotNull(ConnectorPhoton, "Connector Photon not set");

            base.Start();
            Connection_Button.onClick.AddListener(OnTryConnection);
        }

        private void OnTryConnection()
        {
            if (PhotonNetwork.connectionState != ConnectionState.Disconnected)
                return;

            if (Connection_InputField.text.Length > 3)
                ConnectorPhoton.Connection_Photon(Connection_InputField.text);
            else
                Connection_InputField.Select();
        }
    }
}