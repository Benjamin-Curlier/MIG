using MIG.Scripts.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MIG.Scripts.Character
{
    public enum PlayerState
    {
        Idle = 0,
        IsAttacking
    }

    [Serializable]
    public class PlayerAttributes
    {
        public Characteristic Health;
        public Characteristic Armor;
        public Characteristic ManaPoints;
        public Characteristic ActionPoints;

        public PlayerAttributes()
        {
            Health = new Characteristic(50, 150);
            Armor = new Characteristic(10, 200);
            ManaPoints = new Characteristic(50, 150);
            ActionPoints = new Characteristic(1, 5);
        }
    }

    [Serializable]
    public class Player
    {
        public string Name;
        public GameObject Visual = null;
        public List<Spell> Spells;
        public PlayerAttributes Attributes;
        private Vector2Int _positionOnGrid;

        public Vector2Int PositionOnGrid { get { return _positionOnGrid; } }

        public string PrefabName;

        public Player()
        {
            Spells = new List<Spell>();
            Attributes = new PlayerAttributes();
            PrefabName = "Male_Knight";
        }

        public void SetPositionOnGrid(Vector2Int gridPosition)
        {
            _positionOnGrid = gridPosition;
        }
    }
}
