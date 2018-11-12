using MIG.Scripts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MIG.Scripts.Game.Interfaces;
using MIG.Scripts.Character;

namespace MIG.Scripts.UI
{
    public class SpellBar : MonoBehaviour, INewCharacterTurn
    {
        public List<Button> SpellsButton;
        public Text ActionPointsDisplay;
        public ConnectorPhoton connector;

        private Player _player;

        public void NewCharacterTurn(NextPlayerDto character)
        {
            _player = connector.Game.CurrentPlayer;

            foreach (var btn in SpellsButton)
            {
                var img = btn.GetComponent<Image>();
                img.type = Image.Type.Filled;
                img.fillAmount = 0.0f;
                btn.interactable = false;
            }

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (_player != null)
            {
                ActionPointsDisplay.text = $"AP : {_player.Attributes.ActionPoints.FinalValue} / {_player.Attributes.ActionPoints.BaseValue}";

                var activeButtons = SpellsButton.GetRange(0, _player.Spells.Count);
                for (int i = 0; i < activeButtons.Count; i++)
                {
                    var btn = activeButtons[i];
                    btn.interactable = true;
                    var sprite = Resources.Load<Sprite>($"Sprites/SpellIcons/{_player.Spells[i].Icon}");
                    var img = btn.GetComponent<Image>();
                    img.sprite = sprite;
                    img.type = Image.Type.Filled;
                    img.fillAmount = _player.Spells[i].Attributes.Cooldown.FinalValue / _player.Spells[i].Attributes.Cooldown.BaseValue;

                    if (img.fillAmount <= 0.0f)
                        img.fillAmount = 0.15f;
                }
            }
            else
            {
                ActionPointsDisplay.text = "Opponent turn";
            }
        }

        public void Start()
        {
            connector.Game.AddToListener(this);
        }

        public void OnDestroy()
        {
            connector.Game.RemoveFromListener(this);
        }
    }
}
