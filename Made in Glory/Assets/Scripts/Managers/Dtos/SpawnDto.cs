using System;

namespace MIG.Scripts.Dtos
{
    [Serializable]
    public struct SpawnDto
    {
        public SpawnDto(int p, int i, bool b, bool b2)
        {
            pos = p;
            indexCell = i;
            local = b;
            last = b2;
        }

        public int pos;
        public int indexCell;
        public bool local;
        public bool last;
    }
}