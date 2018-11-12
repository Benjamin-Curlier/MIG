using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIG.Scripts.Character;
using System;
using MIG.Scripts.Common.Json;
using System.Linq;
using MIG.Scripts.Terrain;
using MIG.Scripts.Dtos;
using MIG.Scripts.Game.Interfaces;

namespace MIG
{
    public class Game
    {
        [Serializable]
        public class PlayerState
        {
            public PlayerState(string c, bool master)
            {
                character = c;
                Master = master;
                Alive = true;
            }

            public string character;
            public bool Alive;
            public bool Master;
        }

        public PlayerDto OtherPlayer;
        public PlayerDto LocalPlayer;

        public List<IEndOfGame> ListenersEndOfGame;
        public List<IEndOfTurn> ListenersEndOfTurn;
        public List<INewCharacterTurn> ListenersNewCharacterTurn;
        public List<IPlayerAttack> ListenersPlayerAttack;
        public List<IPlayerJoinRoom> ListenersPlayerJoinRoom;
        public List<IPlayerLeaveRoom> ListenersPlayerLeaveRoom;
        public List<IPlayerMovement> ListenersPlayerMovement;
        public List<IPlayerReady> ListenersPlayerReady;
        public List<IStartGame> ListenersStartGame;

        public Player CurrentPlayer;
        private Player Player;

        private ConnectorPhoton parent;
        private bool EndTurn = false;
        private bool EndGame = false;
        private bool Start = false;
        private GameObject[] _availableModels = Resources.LoadAll<GameObject>("Characters/Prefabs");

        List<PlayerState> pawns;

        public Game(ConnectorPhoton p)
        {
            parent = p;
            OtherPlayer = new PlayerDto();
            LocalPlayer = new PlayerDto();
            LocalPlayer.PlayerPhoton = PhotonNetwork.player;

            ListenersEndOfGame = new List<IEndOfGame>();
            ListenersEndOfTurn = new List<IEndOfTurn>();
            ListenersNewCharacterTurn = new List<INewCharacterTurn>();
            ListenersPlayerAttack = new List<IPlayerAttack>();
            ListenersPlayerJoinRoom = new List<IPlayerJoinRoom>();
            ListenersPlayerLeaveRoom = new List<IPlayerLeaveRoom>();
            ListenersPlayerMovement = new List<IPlayerMovement>();
            ListenersPlayerReady = new List<IPlayerReady>();
            ListenersStartGame = new List<IStartGame>();
        }

        public void AddToListener(object listener)
        {
            if (listener is IEndOfGame)
            {
                var listen = listener as IEndOfGame;
                ListenersEndOfGame.Add(listen);
            }
            if (listener is IEndOfTurn)
            {
                var listen = listener as IEndOfTurn;
                ListenersEndOfTurn.Add(listen);
            }
            if (listener is INewCharacterTurn)
            {
                var listen = listener as INewCharacterTurn;
                ListenersNewCharacterTurn.Add(listen);
            }
            if (listener is IPlayerAttack)
            {
                var listen = listener as IPlayerAttack;
                ListenersPlayerAttack.Add(listen);
            }
            if (listener is IPlayerJoinRoom)
            {
                var listen = listener as IPlayerJoinRoom;
                ListenersPlayerJoinRoom.Add(listen);
            }
            if (listener is IPlayerLeaveRoom)
            {
                var listen = listener as IPlayerLeaveRoom;
                ListenersPlayerLeaveRoom.Add(listen);
            }
            if (listener is IPlayerMovement)
            {
                var listen = listener as IPlayerMovement;
                ListenersPlayerMovement.Add(listen);
            }
            if (listener is IPlayerReady)
            {
                var listen = listener as IPlayerReady;
                ListenersPlayerReady.Add(listen);
            }
            if (listener is IStartGame)
            {
                var listen = listener as IStartGame;
                ListenersStartGame.Add(listen);
            }
        }

        public void RemoveFromListener(object listener)
        {
            if (listener is IEndOfGame)
            {
                var listen = listener as IEndOfGame;
                ListenersEndOfGame.Remove(listen);
            }
            if (listener is IEndOfTurn)
            {
                var listen = listener as IEndOfTurn;
                ListenersEndOfTurn.Remove(listen);
            }
            if (listener is INewCharacterTurn)
            {
                var listen = listener as INewCharacterTurn;
                ListenersNewCharacterTurn.Remove(listen);
            }
            if (listener is IPlayerAttack)
            {
                var listen = listener as IPlayerAttack;
                ListenersPlayerAttack.Remove(listen);
            }
            if (listener is IPlayerJoinRoom)
            {
                var listen = listener as IPlayerJoinRoom;
                ListenersPlayerJoinRoom.Remove(listen);
            }
            if (listener is IPlayerLeaveRoom)
            {
                var listen = listener as IPlayerLeaveRoom;
                ListenersPlayerLeaveRoom.Remove(listen);
            }
            if (listener is IPlayerMovement)
            {
                var listen = listener as IPlayerMovement;
                ListenersPlayerMovement.Remove(listen);
            }
            if (listener is IPlayerReady)
            {
                var listen = listener as IPlayerReady;
                ListenersPlayerReady.Remove(listen);
            }
            if (listener is IStartGame)
            {
                var listen = listener as IStartGame;
                ListenersStartGame.Remove(listen);
            }
        }

        public Player GetActualPlayer() => this.Player;

        private List<PlayerState> SetPawns()
        {
            int y = 0;
            int z = 0;

            List<PlayerState> pawns = new List<PlayerState>(LocalPlayer.PlayerTeam.Characters.Count + OtherPlayer.PlayerTeam.Characters.Count);

            while (true)
            {
                bool b = false;

                if (y < LocalPlayer.PlayerTeam.Characters.Count)
                {
                    pawns.Add(new PlayerState(LocalPlayer.PlayerTeam.Characters[y].Name, true));
                    y += 1;
                    b = true;
                }
                if (z < OtherPlayer.PlayerTeam.Characters.Count)
                {
                    pawns.Add(new PlayerState(OtherPlayer.PlayerTeam.Characters[z].Name, false));
                    z += 1;
                    b = true;
                }

                if (b == false)
                    break;
                b = false;
            }

            return pawns;
        }


        public IEnumerator GameUpdate()
        {
            yield return new WaitUntil(() => Start);
            Start = false;

            int i = 0;
            foreach (Player character in LocalPlayer.PlayerTeam.Characters)
            {
                SendSpawnPlayer(new SpawnDto(i, GridGenerator.Instance.GetFreeCellIndex(1), true, false));
                i += 1;
                yield return new WaitForEndOfFrame();
            }

            i = 0;
            foreach (Player character in OtherPlayer.PlayerTeam.Characters)
            {
                if (i == OtherPlayer.PlayerTeam.Characters.Count - 1)
                    SendSpawnPlayer(new SpawnDto(i, GridGenerator.Instance.GetFreeCellIndex(2), false, true));
                else
                    SendSpawnPlayer(new SpawnDto(i, GridGenerator.Instance.GetFreeCellIndex(2), false, false));
                i += 1;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => Start);

            pawns = SetPawns();

            int turn = 0;
            float timeAtStart;
            float turnDuration = 20f;

            while (EndGame == false)
            {
                foreach (PlayerState character in pawns)
                {
                    if (character.Alive && EndGame == false)
                    {
                        SendEvNextCharacter(new NextPlayerDto(character, PhotonNetwork.time + turnDuration));
                        timeAtStart = Time.time;
                        while (Time.time < timeAtStart + turnDuration)
                        {
                            if (EndTurn == true)
                            {
                                EndTurn = false;
                                break;
                            }

                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                if (EndGame == false)
                    SendEndTurn(turn);
                turn++;
            }

            Debug.Log("Fin de partie");
        }

        public void OnEvent(byte eventCode, object content, int senderId)
        {
            switch ((ConnectorPhoton.EventCode)eventCode)
            {
                case ConnectorPhoton.EventCode.EvReady:
                    PlayerReady();
                    break;
                case ConnectorPhoton.EventCode.EvLaunchGame:
                    string map = content as string;
                    parent.Load(map);
                    break;
                case ConnectorPhoton.EventCode.EvSendTeam:
                    string team = content as string;
                    OtherPlayer.PlayerTeam = JsonUtility.FromJson<Team>(team);
                    break;
                case ConnectorPhoton.EventCode.EvReadyForGame:
                    ReadyTolaunchGameUpdate(senderId);
                    break;
                case ConnectorPhoton.EventCode.EvNewCharacterTurn:
                    string character = content as string;
                    NewCharacterTurn(JsonUtility.FromJson<NextPlayerDto>(character));
                    break;
                case ConnectorPhoton.EventCode.EvPassCharacterTurn:
                    EndTurn = true;
                    break;
                case ConnectorPhoton.EventCode.EvMove:
                    string move = content as string;
                    PlayerMove(Player, JsonUtility.FromJson<MoveDto>(move));
                    break;
                case ConnectorPhoton.EventCode.EvSpell:
                    string attack = content as string;
                    var attackData = JsonUtility.FromJson<AttackDto>(attack);
                    var playerAttacked = GetPlayerFromAttackData(attackData);
                    PlayerSpell(Player, attackData.indexSpell, playerAttacked);
                    break;
                case ConnectorPhoton.EventCode.EvEndTurn:
                    EndOfTurn();
                    break;
                case ConnectorPhoton.EventCode.EvEndGame:
                    EndOfGame();
                    break;
                case ConnectorPhoton.EventCode.EvSpawnPlayer:
                    string data = content as string;
                    SpawnPlayer(JsonUtility.FromJson<SpawnDto>(data));
                    break;
                case ConnectorPhoton.EventCode.EvEndSpawnPlayer:
                    EndSpawnPlayer(senderId);
                    break;
                case ConnectorPhoton.EventCode.EvPlayerDeath:
                    string death = content as string;
                    KillPlayer(death);
                    break;
            }
        }

        void KillPlayer(string name)
        {
            var isMaster = CurrentPlayer == null;

            pawns.First(x => x.Master == isMaster && x.character == name).Alive = false;

            if (pawns.Where(x => x.Master == isMaster).All(x => !x.Alive))
            {
                EndGame = true;

                byte evCode = (byte)ConnectorPhoton.EventCode.EvEndGame;
                bool reliable = true;

                PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
            }
        }

        private void SendEvNextCharacter(NextPlayerDto data)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvNewCharacterTurn;

            string content = JsonUtility.ToJson(data);
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
        }

        private void SendEndTurn(int turn)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvEndTurn;

            bool reliable = true;
            PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
        }

        private void SendSpawnPlayer(SpawnDto data)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvSpawnPlayer;

            string content = JsonUtility.ToJson(data);
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
        }

        private void SendLastSpawn()
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvEndSpawnPlayer;
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.MasterClient });
        }

        public void PlayerJoinRoom(PhotonPlayer other)
        {
            OtherPlayer.PlayerPhoton = other;

            foreach (IPlayerJoinRoom igcb in ListenersPlayerJoinRoom)
            {
                igcb.PlayerJoinRoom(other);
            }
        }

        public void PlayerLeaveRoom(PhotonPlayer other)
        {
            OtherPlayer.PlayerPhoton = null;

            foreach (IPlayerLeaveRoom igcb in ListenersPlayerLeaveRoom)
            {
                igcb.PlayerLeaveRoom(other);
            }
        }

        public void EndOfTurn()
        {
            foreach (var character in LocalPlayer.PlayerTeam.Characters)
            {
                character.Attributes.ActionPoints.ResetFlatModifiers();
                character.Attributes.ManaPoints.AddModifier(10f);

                if (character.Attributes.ManaPoints.FinalValue > character.Attributes.ManaPoints.BaseValue)
                {
                    character.Attributes.ManaPoints.ResetFlatModifiers();
                    character.Attributes.ManaPoints.ResetModifiers();
                }

                foreach (var spell in character.Spells)
                {
                    if (spell.Attributes.Cooldown.BaseValue != spell.Attributes.Cooldown.FinalValue)
                    {
                        spell.Attributes.Cooldown.AddFlatModifier(1);
                        if (spell.Attributes.Cooldown.BaseValue == spell.Attributes.Cooldown.FinalValue)
                            spell.Attributes.Cooldown.ResetFlatModifiers();
                    }
                }
            }

            foreach (var character in OtherPlayer.PlayerTeam.Characters)
            {
                character.Attributes.ActionPoints.ResetFlatModifiers();
                character.Attributes.ManaPoints.AddModifier(10f);

                if (character.Attributes.ManaPoints.FinalValue > character.Attributes.ManaPoints.BaseValue)
                {
                    character.Attributes.ManaPoints.ResetFlatModifiers();
                    character.Attributes.ManaPoints.ResetModifiers();
                }

                foreach (var spell in character.Spells)
                {
                    if (spell.Attributes.Cooldown.BaseValue != spell.Attributes.Cooldown.FinalValue)
                    {
                        spell.Attributes.Cooldown.AddFlatModifier(1);
                        if (spell.Attributes.Cooldown.BaseValue == spell.Attributes.Cooldown.FinalValue)
                            spell.Attributes.Cooldown.ResetFlatModifiers();
                    }
                }
            }

            foreach (IEndOfTurn igcb in ListenersEndOfTurn)
            {
                igcb.EndOfTurn();
            }
        }

        public void EndOfGame()
        {
            foreach (IEndOfGame igcb in ListenersEndOfGame)
            {
                igcb.EndOfGame();
            }
        }

        public void StartOfGame()
        {
            foreach (IStartGame igcb in ListenersStartGame)
            {
                igcb.StartOfGame();
            }
        }

        public void PlayerReady()
        {
            foreach (IPlayerReady igcb in ListenersPlayerReady)
            {
                igcb.PlayerReady();
            }
        }

        public void NewCharacterTurn(NextPlayerDto data)
        {
            if (!PhotonNetwork.isMasterClient)
                data.player.Master = !data.player.Master;

            if (data.player.Master)
            {
                foreach (Player p in LocalPlayer.PlayerTeam.Characters)
                {
                    if (p.Name == data.player.character)
                    {
                        CurrentPlayer = p;
                        Player = p;
                        break;
                    }
                }
            }
            else
            {
                CurrentPlayer = null;
                foreach (Player p in OtherPlayer.PlayerTeam.Characters)
                {
                    if (p.Name == data.player.character)
                    {
                        Player = p;
                        break;
                    }
                }
            }

            CursorManager.Instance.MoveCursor(Player.Visual);

            foreach (INewCharacterTurn igcb in ListenersNewCharacterTurn)
            {
                igcb.NewCharacterTurn(data);
            }
        }

        public void ReadyTolaunchGameUpdate(int idPlayer)
        {
            if (idPlayer == LocalPlayer.PlayerPhoton.ID)
                LocalPlayer.Ready = true;

            if (idPlayer == OtherPlayer.PlayerPhoton.ID)
                OtherPlayer.Ready = true;

            if (LocalPlayer.Ready && OtherPlayer.Ready == true)
            {
                Start = true;
                LocalPlayer.Ready = false;
                OtherPlayer.Ready = false;
            }
        }

        public void SpawnPlayer(SpawnDto data)
        {
            if (!PhotonNetwork.isMasterClient)
                data.local = !data.local;

            GridGenerator grid = GridGenerator.Instance;
            Vector3 v = grid.GetCellFomIndex(data.indexCell);
            Quaternion r = Quaternion.identity;

            if (data.local)
            {
                LocalPlayer.PlayerTeam.Characters[data.pos].Visual = parent.My_Instantiate(_availableModels.First(x => x.name == LocalPlayer.PlayerTeam.Characters[data.pos].PrefabName), v, r);
                LocalPlayer.PlayerTeam.Characters[data.pos].SetPositionOnGrid(grid.GetGridPositionFromIndex(data.indexCell));
            }
            else
            {
                OtherPlayer.PlayerTeam.Characters[data.pos].Visual = parent.My_Instantiate(_availableModels.First(x => x.name == OtherPlayer.PlayerTeam.Characters[data.pos].PrefabName), v, r);
                OtherPlayer.PlayerTeam.Characters[data.pos].SetPositionOnGrid(grid.GetGridPositionFromIndex(data.indexCell));
            }

            if (data.last)
                SendLastSpawn();
        }

        public void EndSpawnPlayer(int idPlayer)
        {
            if (idPlayer == LocalPlayer.PlayerPhoton.ID)
                LocalPlayer.Ready = true;

            if (idPlayer == OtherPlayer.PlayerPhoton.ID)
                OtherPlayer.Ready = true;

            if (LocalPlayer.Ready && OtherPlayer.Ready == true)
            {
                Start = true;
                LocalPlayer.Ready = false;
                OtherPlayer.Ready = false;
            }
        }

        public void PlayerMove(Player player, MoveDto data)
        {
            foreach (IPlayerMovement ipa in ListenersPlayerMovement)
            {
                ipa.PlayerMove(player, data);
            }
        }

        public void PlayerSpell(Player player, int indexSpell, IEnumerable<Player> playerAttacked)
        {
            foreach (IPlayerAttack ipa in ListenersPlayerAttack)
            {
                ipa.PlayerSpell(player, indexSpell, playerAttacked);
            }
        }

        public IEnumerable<Player> GetPlayerFromAttackData(AttackDto data)
        {
            List<Player> attackedPlayers = new List<Player>();

            var targetGameObjet = GridGenerator.Instance.GetCell(data.gridPos).cellGameObject;

            var colliders = Physics.OverlapSphere(targetGameObjet.transform.position, Player.Spells[data.indexSpell].Attributes.Radius.FinalValue, LayerMask.GetMask("GameplayElement"))
                                    .Where(x => x.name.Contains("Grid Cell"));

            foreach (var collider in colliders)
            {
                var target = GridGenerator.Instance.GetCell(collider.gameObject);

                var attackedPlayer = LocalPlayer.PlayerTeam.Characters.FirstOrDefault(x => x.PositionOnGrid == target.Position);
                if (attackedPlayer == null)
                    attackedPlayer = OtherPlayer.PlayerTeam.Characters.FirstOrDefault(x => x.PositionOnGrid == target.Position);

                if (attackedPlayer != null)
                    attackedPlayers.Add(attackedPlayer);
            }

            return attackedPlayers;
        }
    }
}