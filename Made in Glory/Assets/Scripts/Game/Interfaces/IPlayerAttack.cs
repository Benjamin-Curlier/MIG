using MIG.Scripts.Character;
using System;
using System.Collections.Generic;

namespace MIG.Scripts.Game.Interfaces
{
    public interface IPlayerAttack
    {
        void PlayerSpell(Player player, int indexSpell, IEnumerable<Player> playerAttacked);
    }
}