using MIG.Scripts.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Managers.Dtos
{
    public struct NewStatePlayerDto
    {
        public PlayerState state;
        public int indexSpell;

        public NewStatePlayerDto(int indexSpell, PlayerState state)
        {
            this.state = state;
            this.indexSpell = indexSpell;
        }
    }
}
