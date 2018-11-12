using Assets.Scripts.Managers.Dtos;
using MIG.Scripts.Character;
using MIG.Scripts.Commands;
using MIG.Scripts.Common.Events;
using MIG.Scripts.Common.Input;
using MIG.Scripts.Dtos;
using MIG.Scripts.Game.Interfaces;
using MIG.Scripts.PlayerManager.Commands;
using MIG.Scripts.Terrain;
using MIG.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MIG.Scripts.PlayerManager
{
    public class PlayerManager : MonoBehaviour, IPlayerAttack, IPlayerMovement, INewCharacterTurn
    {
        public static event EventHandler<InfoEventArgs<NewStatePlayerDto>> PlayerStateChanged;

        public PlayerState state;
        public int spellToAttack = -1;
        public ConnectorPhoton connector;
        public GameObject FireBall;
        private Camera _camera;

        public SpellBar SpellBar;
        public DisplayInformationsInGame DisplayInfos;

        public void Start()
        {
            state = PlayerState.Idle;
            InputController.ClickEvent += InputController_ClickEvent;
            InputController.EscapeEvent += InputController_EscapeEvent;
            InputController.MoveEvent += Inputcontroller_MoveEvent;
            InputController.ShortcutEvent += InputController_ShortcutEvent;
            InputController.ScrollEvent += InputController_ScrollEvent;

            connector.Game.AddToListener(this);
        }

        private void InputController_ScrollEvent(object sender, InfoEventArgs<float> e)
        {
            _camera.orthographicSize += e.Info;
        }

        public void Awake()
        {
            _camera = Camera.main;
        }

        private void InputController_ShortcutEvent(object sender, InfoEventArgs<int> e)
        {
            var player = connector.Game.CurrentPlayer;

            if (player != null)
            {
                var gameobjectevent = e.Info;

                ICommand command = default(ICommand);

                command = new SelectSpellCommand(player.Visual, gameobjectevent, this, player);

                if (command != null && command.EvaluateCommand())
                    command.ExecuteCommand();
            }
        }

        internal void UpdateState(int indexSpell, PlayerState newState)
        {
            state = newState;
            spellToAttack = indexSpell;

            PlayerStateChanged?.Invoke(this, new InfoEventArgs<NewStatePlayerDto>(new NewStatePlayerDto(spellToAttack, state)));
        }

        public void OnDestroy()
        {
            connector.Game.RemoveFromListener(this);
        }

        private void Inputcontroller_MoveEvent(object sender, InfoEventArgs<Vector2> e)
        {
            Vector3 pos = _camera.transform.position;

            pos.x += e.Info.x * 2;
            pos.z += e.Info.y * 2;

            _camera.transform.position = pos;
        }

        private void InputController_EscapeEvent(object sender, InfoEventArgs<object> e)
        {
            if (state == PlayerState.IsAttacking)
                UpdateState(-1, PlayerState.Idle);
        }

        private void InputController_ClickEvent(object sender, InfoEventArgs<GameObject> e)
        {
            var player = connector.Game.CurrentPlayer;

            if (player != null)
            {
                var gameobjectevent = e.Info;

                Debug.Log($"Clicked object : {gameobjectevent}");

                ICommand command = default(ICommand);

                if (gameobjectevent.name.Contains("Grid") && state == PlayerState.Idle)
                    command = new MoveCommand(player.Visual, gameobjectevent, player, SpellBar, DisplayInfos);
                else if (gameobjectevent.name.Contains("Grid") && state == PlayerState.IsAttacking)
                    command = new AttackCommand(player.Visual, gameobjectevent, this, player, SpellBar, DisplayInfos);
                else if (gameobjectevent.name.Contains("Spell"))
                {
                    int spellIndex = 0;
                    int.TryParse(gameobjectevent.name.Replace("Spell", string.Empty), out spellIndex);

                    command = new SelectSpellCommand(player.Visual, spellIndex, this, player);
                }

                if (command != null && command.EvaluateCommand())
                    command.ExecuteCommand();
            }
        }

        public void PlayerMove(Player player, MoveDto data)
        {
            GridGenerator terrainGrid = GridGenerator.Instance;

            terrainGrid.ChangeStateCell(data.lastCell, CellState.Empty);
            terrainGrid.ChangeStateCell(data.newCell, CellState.Occupied);

            player.SetPositionOnGrid(data.newCell);
            
            terrainGrid.MovePlayerOnGrid(data.lastCell, data.newCell, player.Visual);
        }

        public void PlayerSpell(Player player, int indexSpell, IEnumerable<Player> playersAttacked)
        {
            foreach (var playerAttacked in playersAttacked)
            {
                var animAttacked = playerAttacked.Visual.GetComponent<CharacterAnimationManager>();

                var startPosition = playerAttacked.Visual.transform.position + new Vector3(0, 20, 0);
                var fireball = Instantiate(FireBall, startPosition, player.Visual.transform.rotation);
                var target = Instantiate(new GameObject(), playerAttacked.Visual.transform.position, playerAttacked.Visual.transform.rotation);

                fireball.GetComponent<RFX1_Target>().Target = target;

                var multiplier = player.Spells[indexSpell].Attributes.Effect == EffectType.Damage ? -1 : 1;

                if (player.Spells[indexSpell].Attributes.Effect == EffectType.Damage)
                {
                    animAttacked.TakingDamage();
                }

                var anim = player.Visual.GetComponent<CharacterAnimationManager>();

                if (player.Spells[indexSpell].Attributes.Effect == EffectType.Damage)
                    anim.CastingSpell();
                else if (player.Spells[indexSpell].Attributes.Effect == EffectType.Heal)
                    anim.CastingHeal();

                playerAttacked.Attributes.Health.AddFlatModifier((int)player.Spells[indexSpell].Attributes.Damage.FinalValue * multiplier);

                if (player.Spells[indexSpell].Attributes.Effect == EffectType.Heal && playerAttacked.Attributes.Health.FinalValue > playerAttacked.Attributes.Health.BaseValue)
                {
                    playerAttacked.Attributes.Health.ResetFlatModifiers();
                    playerAttacked.Attributes.Health.ResetModifiers();
                }

                Debug.Log($"PlayerAttacked {playerAttacked.Name} HP : {playerAttacked.Attributes.Health.FinalValue}/{playerAttacked.Attributes.Health.BaseValue}");

                animAttacked.IsDead(playerAttacked.Attributes.Health.FinalValue <= 0);

                if (playerAttacked.Attributes.Health.FinalValue <= 0)
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        byte evCode = (byte)ConnectorPhoton.EventCode.EvPlayerDeath;

                        bool reliable = true;
                        PhotonNetwork.RaiseEvent(evCode, playerAttacked.Name, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.MasterClient });
                    }
                }
            }

            UpdateState(-1, PlayerState.Idle);
        }

        public void NewCharacterTurn(NextPlayerDto character)
        {
            UpdateState(-1, PlayerState.Idle);
        }
    }
}