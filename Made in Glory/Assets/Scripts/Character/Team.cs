using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIG.Scripts.Character
{
    [Serializable]
    public class Team
    {
        public string Name;
        public List<Player> Characters;

        public int Gold;

        public Team()
        {
            Gold = 500;
            Characters = new List<Player>();
        }
    }
}
