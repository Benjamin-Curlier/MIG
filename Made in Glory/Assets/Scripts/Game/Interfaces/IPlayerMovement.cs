using MIG.Scripts.Character;
using MIG.Scripts.Dtos;
using System;

namespace MIG.Scripts.Game.Interfaces
{
    public interface IPlayerMovement
    {
        void PlayerMove(Player player, MoveDto data);
    }
}