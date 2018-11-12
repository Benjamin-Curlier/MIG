using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using System;

namespace MIG
{
    public class Lobby : SceneController
    {
        public enum ButtonType { PLAY, DISCONNECT, OPTIONS, EXIT, TUTO, TEAM }

        [Serializable]
        public class WrapperButton
        {
            public Button button;
            public ButtonType type;
        }

        public List<WrapperButton> Buttons;

        protected override void Start()
        {
            base.Start();

            AddButtonsListener();
        }

        private void OnPlay()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        private void OnOptions()
        {
            SceneManager.LoadScene("Options");
        }

        private void OnDisconnected()
        {
            PhotonNetwork.Disconnect();
        }

        private void OnExit()
        {
            Application.Quit();
        }

        private void OnTuto()
        {

        }

        private void OnTeam()
        {
            SceneManager.LoadScene("TeamsManagement");
        }

        private void AddButtonsListener()
        {
            foreach (WrapperButton w in Buttons)
            {
                switch (w.type)
                {
                    case ButtonType.PLAY:
                        w.button.onClick.AddListener(OnPlay);
                        break;
                    case ButtonType.OPTIONS:
                        w.button.onClick.AddListener(OnOptions);
                        break;
                    case ButtonType.DISCONNECT:
                        w.button.onClick.AddListener(OnDisconnected);
                        break;
                    case ButtonType.EXIT:
                        w.button.onClick.AddListener(OnExit);
                        break;
                    case ButtonType.TEAM:
                        w.button.onClick.AddListener(OnTeam);
                        break;
                    case ButtonType.TUTO:
                        w.button.onClick.AddListener(OnTuto);
                        break;
                }
            }
        }
    }
}