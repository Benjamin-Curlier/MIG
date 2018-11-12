using System;
using static MIG.Game;

namespace MIG.Scripts.Dtos
{
    [Serializable]
    public struct NextPlayerDto
    {
        public NextPlayerDto(PlayerState c, double t)
        {
            player = c;
            timeEndTurn = t;
        }

        public PlayerState player;
        public Double timeEndTurn;
    }
}