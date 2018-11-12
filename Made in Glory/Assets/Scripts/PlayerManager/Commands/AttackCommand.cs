using MIG.Scripts.Character;
using MIG.Scripts.Commands;
using MIG.Scripts.Dtos;
using MIG.Scripts.Terrain;
using MIG.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MIG.Scripts.Terrain.GridGenerator;

namespace MIG.Scripts.PlayerManager.Commands
{
    public class AttackCommand : Command
    {
        private GameObject GridCell;
        private PlayerManager PlayerManager;
        private Player PlayerCharac;
        private SpellBar _spellBar;
        private DisplayInformationsInGame _display;

        private Cell _cell;

        public AttackCommand(GameObject player, GameObject gridCell, PlayerManager playerManager, Player playerCharac, SpellBar spellBar, DisplayInformationsInGame display) : base(player)
        {
            GridCell = gridCell;
            PlayerManager = playerManager;
            PlayerCharac = playerCharac;
            _spellBar = spellBar;
            _display = display;
        }

        public override bool EvaluateCommand()
        {
            _cell = GridGenerator.Instance.GetCell(GridCell);

            if (_cell == null)
                return false;

            var distance = Vector2Int.Distance(PlayerCharac.PositionOnGrid, _cell.Position);

            Debug.Log($"Distance : {distance}");
            Debug.Log($"Range spell : {PlayerCharac.Spells[PlayerManager.spellToAttack].Attributes.Range.FinalValue}");

            if (distance > PlayerCharac.Spells[PlayerManager.spellToAttack].Attributes.Range.FinalValue)
                return false;

            return true;
        }

        public override void ExecuteCommand()
        {
            PlayerCharac.Attributes.ActionPoints.AddFlatModifier(-1);
            PlayerCharac.Spells[PlayerManager.spellToAttack].Attributes.Cooldown.AddFlatModifier(((int)PlayerCharac.Spells[PlayerManager.spellToAttack].Attributes.Cooldown.BaseValue + 1) * -1);
            PlayerCharac.Attributes.ManaPoints.AddFlatModifier((int)PlayerCharac.Spells[PlayerManager.spellToAttack].Attributes.ManaCost.FinalValue * -1);

            

            _spellBar.UpdateDisplay();
            _display.UpdateDisplay();

            RaiseEventPhoton(PlayerManager.spellToAttack, _cell.Position);
        }

        private void RaiseEventPhoton(int indexSpell, Vector2Int posGridToAttack)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvSpell;
            var attackData = new AttackDto(posGridToAttack, indexSpell);

            string content = JsonUtility.ToJson(attackData);
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.All });
        }
    }
}
