using MIG.Scripts.Dtos;
using System;

namespace MIG.Scripts.Game.Interfaces
{
    public interface INewCharacterTurn
    {
        void NewCharacterTurn(NextPlayerDto character);
    }
}