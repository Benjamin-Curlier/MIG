using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System;
using System.IO;
using MIG.Scripts.Character;
using MIG.Scripts.Common.Json;
using System.Linq;
using MIG.Scripts.Dtos;
using MIG.Scripts.Game.Interfaces;

namespace MIG
{
    public class RoomMIG : SceneController, IPlayerJoinRoom, IPlayerLeaveRoom, IPlayerReady
    {
        [Serializable]
        public class RoomPlayerData
        {
            public Text Name;
            public Toggle Toggle;
        }

        public RoomPlayerData LocalPlayer;
        public RoomPlayerData OtherPlayer;
        public Button LeaveButton;
        public Dropdown TeamsDropdown;

        List<Team> _teams;

        protected override void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull<RoomPlayerData>(LocalPlayer);
            Assert.IsNotNull<Toggle>(LocalPlayer.Toggle);
            Assert.IsNotNull<Text>(LocalPlayer.Name);
            Assert.IsNotNull<RoomPlayerData>(OtherPlayer);
            Assert.IsNotNull<Toggle>(OtherPlayer.Toggle);
            Assert.IsNotNull<Text>(OtherPlayer.Name);
            Assert.IsNotNull<Button>(LeaveButton);
            Assert.IsNotNull<Dropdown>(TeamsDropdown);

            base.Start();
            LeaveButton.onClick.AddListener(LeaveRoom);
            LocalPlayer.Toggle.onValueChanged.AddListener(OnToggle);
            OtherPlayer.Toggle.interactable = false;
            SetUpRoom();
        }

        private void OnDestroy()
        {
            if (ConnectorPhoton.Game != null)
                ConnectorPhoton.Game.RemoveFromListener(this);
        }

        private void SetUpRoom()
        {
            if (ConnectorPhoton.State != ConnectorPhoton.PhotonState.Room)
                return;

            ConnectorPhoton.Game.AddToListener(this);

            LocalPlayer.Name.text += PhotonNetwork.player.NickName;
            if (PhotonNetwork.otherPlayers.Length > 0)
            {
                OtherPlayer.Name.text += PhotonNetwork.otherPlayers[0].NickName;
                ConnectorPhoton.Game.OtherPlayer.PlayerPhoton = PhotonNetwork.otherPlayers[0];
            }

            var json = string.Empty;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/PlayerDatas/", "Teams.json");
            json = File.ReadAllText(filePath);
            _teams = JsonHelper.GetJsonArray<Team>(json).ToList();

            if (_teams == null)
            {
                PhotonNetwork.LeaveRoom();
                return;
            }

            TeamsDropdown.ClearOptions();
            TeamsDropdown.AddOptions(_teams.Select(x => x.Name).ToList());
        }

        public override void SetUpDone()
        {
            base.SetUpDone();
            SetUpRoom();
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void OnToggle(bool isToggle)
        {
            LocalPlayer.Toggle.interactable = false;

            if (ConnectorPhoton.Game.OtherPlayer.PlayerPhoton == null)
                return;

            SendEvReady();

            Team t = _teams[TeamsDropdown.value]; 

            ConnectorPhoton.Game.LocalPlayer.PlayerTeam = t;

            SendEvTeam(JsonUtility.ToJson(t));
        }

        public void SendEvReady()
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvReady;
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others });
        }

        public void SendEvTeam(string team)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvSendTeam;
            string content = team;
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others });
        }

        public void SendEvLaunchGame()
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvLaunchGame;
            bool reliable = true;
            string content;

           
            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    content = "Game";
                    break;

                case 1:
                    content = "Game2";
                    break;

                case 2:
                    content = "Game3";
                    break;

                default:
                    content = "Game"; // è.é fuck you compiler i know what i'm doing
                    break;
            }


            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
        }

        public void PlayerJoinRoom(PhotonPlayer other)
        {
            OtherPlayer.Name.text += other.NickName;

            if (LocalPlayer.Toggle.isOn)
                OnToggle(true);
        }

        public void PlayerLeaveRoom(PhotonPlayer other)
        {
            OtherPlayer.Name.text = "Player : ";
            OtherPlayer.Toggle.isOn = false;
        }

        public void PlayerReady()
        {
            if (ConnectorPhoton.Game.OtherPlayer.PlayerPhoton == null)
                return;

            OtherPlayer.Toggle.isOn = true;

            if (LocalPlayer.Toggle.isOn == true)
                SendEvLaunchGame();
        }
    }
}