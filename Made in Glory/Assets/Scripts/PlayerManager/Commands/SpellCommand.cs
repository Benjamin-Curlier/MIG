using MIG.Scripts.Character;
using MIG.Scripts.Commands;
using MIG.Scripts.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MIG.Scripts.PlayerManager.Commands
{
    public class SelectSpellCommand : Command
    {
        private GameObject ButtonSpell;
        private int _spellIndex;
        private GridGenerator _grid;
        private PlayerManager PlayerManager;
        private Player PlayerCharac;

        public SelectSpellCommand(GameObject player, int spellIndex, PlayerManager playerManager, Player playerCharac) : base(player)
        {
            _spellIndex = spellIndex;
            _grid = GridGenerator.Instance;
            PlayerManager = playerManager;
            PlayerCharac = playerCharac;
        }

        // idk
        public override bool EvaluateCommand()
        {
            if (_spellIndex == 0)
                _spellIndex = 9;
            else
                _spellIndex -= 1;

            if (_spellIndex >= PlayerCharac.Spells.Count)
            {
                Debug.LogError($"Spell index  {_spellIndex} and count {PlayerCharac.Spells.Count}");
                return false;
            }

            if (PlayerCharac.Attributes.ManaPoints.FinalValue < PlayerCharac.Spells[_spellIndex].Attributes.ManaCost.FinalValue)
                return false;

            if (PlayerCharac.Attributes.ActionPoints.FinalValue < 1)
                return false;

            if (PlayerCharac.Spells[_spellIndex].Attributes.Cooldown.BaseValue != PlayerCharac.Spells[_spellIndex].Attributes.Cooldown.FinalValue)
                return false;

            return true;
        }

        public override void ExecuteCommand()
        {
            PlayerManager.UpdateState(_spellIndex, PlayerState.IsAttacking);
        }
    }
}
