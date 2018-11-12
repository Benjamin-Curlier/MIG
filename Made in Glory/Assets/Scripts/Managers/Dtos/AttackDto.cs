using System;
using UnityEngine;

namespace MIG.Scripts.Dtos
{
    [Serializable]
    public struct AttackDto
    {
        public AttackDto(Vector2Int gridPos, int indexSpell)
        {
            this.gridPos = gridPos;
            this.indexSpell = indexSpell;
        }

        public Vector2Int gridPos;
        public int indexSpell;
    }
}