using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIG.Scripts.Commands;
using MIG.Scripts.Terrain;
using MIG.Scripts.Character;
using System;
using MIG.Scripts.Dtos;
using static MIG.Scripts.Terrain.GridGenerator;
using MIG.Scripts.UI;

namespace MIG.Scripts.Commands
{
    // The input class must fill and call the functions of this class
    public class MoveCommand : Command
    {
        protected GameObject TargetCell;
        private GridGenerator _grid;
        protected Player PlayerCharac;
        private Stack<Vector2Int> _path;
        private SpellBar _spellBar;
        private DisplayInformationsInGame _display;

        private Cell _cell;

        public MoveCommand(GameObject Player, GameObject TargetCell, Player player, SpellBar spellBar, DisplayInformationsInGame display) : base(Player)
        {
            this.TargetCell = TargetCell;
            _grid = GridGenerator.Instance;
            this.PlayerCharac = player;
            _spellBar = spellBar;
            _display = display;
        }

        public override bool EvaluateCommand()
        {
            _cell = _grid.GetCell(TargetCell);

            Debug.Log($"Cell : {_cell}");
            Debug.Log($"Cell : {_cell.State}");

            if (_cell.State == CellState.Occupied)
                return false;

            Debug.Log($"Player position : {PlayerCharac.PositionOnGrid}");
            Debug.Log($"Cell position : {_cell.Position}");

            _path = _grid.ComputeAStar(PlayerCharac.PositionOnGrid, _cell.Position);

            Debug.Log($"Path count : {_path.Count}");

            //// No path found
            if (_path == null || _path.Count == 0)
                return false;

            Debug.Log($"AP : {PlayerCharac.Attributes.ActionPoints.FinalValue}");

            if ((int)PlayerCharac.Attributes.ActionPoints.FinalValue < _path.Count - 1)
                return false;

            return true;
        }

        public override void ExecuteCommand()
        {
            Debug.Log($"Cell script : {_cell}");

            var oldPosition = PlayerCharac.PositionOnGrid;

            _grid.ChangeStateCell(PlayerCharac.PositionOnGrid, CellState.Empty);

            PlayerCharac.SetPositionOnGrid(_cell.Position);
            _cell.State = CellState.Occupied;

            var newPosition = _grid.MovePlayerOnGrid(PlayerCharac.PositionOnGrid, _cell.Position, Player, _path);

            PlayerCharac.Attributes.ActionPoints.AddFlatModifier((_path.Count - 1) * -1);

            _spellBar.UpdateDisplay();
            _display.UpdateDisplay();

            RaiseEventPhoton(newPosition, _cell.Position, oldPosition);
        }

        private void RaiseEventPhoton(Vector3 newPos, Vector2Int newPosGrid, Vector2Int oldPosGrid)
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvMove;
            var moveData = new MoveDto(newPos, oldPosGrid, newPosGrid);

            string content = JsonUtility.ToJson(moveData);
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others });
        }
    }
}
