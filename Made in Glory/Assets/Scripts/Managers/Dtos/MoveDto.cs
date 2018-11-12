using System;
using UnityEngine;

namespace MIG.Scripts.Dtos
{
    [Serializable]
    public struct MoveDto
    {
        public MoveDto(Vector3 pos, Vector2Int prevCell, Vector2Int nextCell)
        {
            newPos = pos;
            newCell = nextCell;
            lastCell = prevCell;
        }

        public Vector3 newPos;
        public Vector2Int lastCell;
        public Vector2Int newCell;
    }
}