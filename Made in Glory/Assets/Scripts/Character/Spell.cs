using MIG.Scripts.Attributes;
using System;

namespace MIG.Scripts.Character
{
    public enum EffectType
    {
        Damage = 0,
        Heal
    }

    [Serializable]
    public class SpellCharacteristics
    {
        public SpellCharacteristics()
        {
            Damage = new Characteristic(1, 100);
            Range = new Characteristic(1, 10);
            Radius = new Characteristic(1, 10);
            Cooldown = new Characteristic(0, 5);
            ManaCost = new Characteristic(10, 100);
        }

        public Characteristic Damage;
        public Characteristic Range;
        public Characteristic Radius;
        public Characteristic Cooldown;
        public Characteristic ManaCost;
            
        public bool IsABuff;
        public EffectType Effect;
    }

    [Serializable]
    public class Spell
    {
        public string Name;
        public string Icon;
        public SpellCharacteristics Attributes;

        public Spell()
        {
            Attributes = new SpellCharacteristics();
            Icon = "SpellBookPreface_01";
        }
    }
}
