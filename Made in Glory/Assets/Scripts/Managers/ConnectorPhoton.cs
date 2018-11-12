using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

namespace MIG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Photon Connector")]
    public class ConnectorPhoton : ScriptableObject, IPunCallbacks
    {
        public enum PhotonState { Disconnected, Lobby, Room, Game, NONE }
        public enum EventCode
        {
            EvReady = 0x0,
            EvLaunchGame = 0x1,
            EvCancelGame = 0x2,
            EvSendTeam = 0x3,
            EvReadyForGame = 0x4,
            EvMove = 0x5,
            EvSpell = 0x6,
            EvNewCharacterTurn = 0x8,
            EvPassCharacterTurn = 0x9,
            EvEndTurn = 0x10,
            EvEndGame = 0x11,
            EvSpawnPlayer = 0x12,
            EvEndSpawnPlayer = 0x13,
            EvPlayerDeath = 0x14,
        }

        [SerializeField] string GameVersion;
        [SerializeField] TypedLobby Lobby;
        [NonSerialized] PhotonState state = PhotonState.NONE;
        [NonSerialized] bool isLoading = false;
        [NonSerialized] bool isSetting = false;
        [NonSerialized] Game game;

        private bool Setting
        {
            get { return isSetting; }
            set { isSetting = value; }
        }

        public Game Game
        {
            get { return game; }
            private set
            {
                game = value;
                if (value != null)
                    PhotonNetwork.OnEventCall += game.OnEvent;
                else
                    PhotonNetwork.OnEventCall -= null;
            }
        }

        public bool Loading
        {
            get { return isLoading; }
            private set { isLoading = value; }
        }

        public PhotonState State
        {
            get { return state; }
            private set { state = value; }
        }

        public IEnumerator SetUp(SceneController sceneController)
        {
            PhotonNetwork.CallbacksTarget = this;
            SceneManager.sceneLoaded += Onload;

            switch (sceneController.PhotonState)
            {
                case PhotonState.Room:
                    Setting = true;
                    Connection_Photon(Environment.UserName);
                    yield return new WaitUntil(() => !Setting);
                    Setting = true;
                    PhotonNetwork.JoinRandomRoom();
                    yield return new WaitUntil(() => !Setting);
                    break;
                case PhotonState.Lobby:
                    Setting = true;
                    Connection_Photon(Environment.UserName);
                    yield return new WaitUntil(() => !Setting);
                    break;
                case PhotonState.Disconnected:
                    break;
            }

            sceneController.SetUpDone();
        }

        public void Load(string sceneName)
        {
            switch (sceneName)
            {
                case "Launcher":
                    State = PhotonState.Disconnected;
                    break;
                case "Room":
                    State = PhotonState.Room;
                    Game = new Game(this);
                    break;
                case "Lobby":
                    State = PhotonState.Lobby;
                    break;
                case "Game":
                    State = PhotonState.Game;
                    break;
            }

            if (Setting)
            {
                Setting = false;
                return;
            }

            Loading = true;
            SceneManager.LoadScene(sceneName);
        }

        private void Onload(Scene arg0, LoadSceneMode arg1)
        {
            Loading = false;
        }

        public void Connection_Photon(string name)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.ConnectUsingSettings(GameVersion);
            PhotonNetwork.playerName = name;
        }

        #region Callbacks

        public void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(Lobby);
        }

        public void OnConnectedToPhoton()
        {

        }

        public void OnConnectionFail(DisconnectCause cause)
        {
            throw new NotImplementedException();
        }

        public void OnCreatedRoom()
        {
            PhotonNetwork.room.MaxPlayers = 2;
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            throw new NotImplementedException();
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromPhoton()
        {
            Load("Launcher");
        }

        public void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            switch (cause)
            {
                case DisconnectCause.DisconnectByClientTimeout:
                    Debug.LogWarning("Could not reach photon server");
                    break;
                default:
                    Debug.LogWarning("A problem occured with photon server");
                    break;
            }
        }

        public void OnJoinedLobby()
        {
            Load("Lobby");
        }

        public void OnJoinedRoom()
        {
            Load("Room");
        }

        public void OnLeftLobby()
        {

        }

        public void OnLeftRoom()
        {
            Game = null;
            Load("Lobby");
        }

        public void OnLobbyStatisticsUpdate()
        {

        }

        public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {

        }

        public void OnOwnershipRequest(object[] viewAndPlayer)
        {
            throw new NotImplementedException();
        }

        public void OnOwnershipTransfered(object[] viewAndPlayers)
        {
            throw new NotImplementedException();
        }

        public void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {

        }

        public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {

        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            throw new NotImplementedException();
        }

        public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {

        }

        public void OnPhotonMaxCccuReached()
        {
            throw new NotImplementedException();
        }

        public void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
        {

        }

        public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Game.PlayerJoinRoom(newPlayer);
        }

        public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            if (State == PhotonState.Game)
            {
                PhotonNetwork.LeaveRoom();
                return;
            }

            Game.PlayerLeaveRoom(otherPlayer);
        }

        public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {

        }

        public void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName + "'s room");
        }

        public void OnReceivedRoomListUpdate()
        {

        }

        public void OnUpdatedFriendList()
        {
            throw new NotImplementedException();
        }

        public void OnWebRpcResponse(OperationResponse response)
        {
            throw new NotImplementedException();
        }

        #endregion

        public GameObject My_Instantiate(GameObject prefab, Vector3 v, Quaternion r)
        {
            return Instantiate(prefab, v, r);
        }
    }
}