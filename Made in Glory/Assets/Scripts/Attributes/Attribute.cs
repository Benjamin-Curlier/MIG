using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIG.Scripts.Attributes
{
    [Serializable]
    public class Characteristic
    {
        public float BaseValue;

        private float _finalValue;
        private List<Modifier> _modifiers = new List<Modifier>();
        private List<FlatModifier> _flatModifiers = new List<FlatModifier>();

        private float _minValue;
        private float _maxValue;

        public Characteristic()
        {
            BaseValue = 1.0f;
        }

        public Characteristic(float minValue, float maxValue)
        {
            BaseValue = minValue;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public float FinalValue
        {
            get
            {
                return CalculateValue();
            }
        }

        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
        }

        public float MinValue
        {
            get
            {
                return _minValue;
            }
        }

        protected float CalculateValue()
        {
            _finalValue = BaseValue;

            ApplyFlatModifiers();
            ApplyModifiers();

            return _finalValue;
        }

        private void ApplyFlatModifiers()
        {
            int flatTotal = 0;

            flatTotal = _flatModifiers.Sum(x => x.Value);
            _finalValue += flatTotal;
        }

        private void ApplyModifiers()
        {
            float modifiersTotal = 0.0f;

            modifiersTotal = _modifiers.Sum(x => x.Value);
            _finalValue *= (1.0f + modifiersTotal);
        }

        public void AddModifier(float modifierValue)
        {
            _modifiers.Add(new Modifier(modifierValue / 100f));
        }

        public void RemoveModifier(Modifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        public void AddFlatModifier(int flatModifierValue)
        {
            _flatModifiers.Add(new FlatModifier(flatModifierValue));
        }

        public void RemoveFlatModifier(FlatModifier bonus)
        {
            _flatModifiers.Remove(bonus);
        }

        public void ResetFlatModifiers() => _flatModifiers.Clear();

        public void ResetModifiers() => _modifiers.Clear();

        internal void RemoveModifier(float v)
        {
            var modifier = _modifiers.FirstOrDefault(x => x.Value == v);

            if (modifier != null)
                _modifiers.Remove(modifier);
        }

        internal void RemoveFlatModifier(int v)
        {
            var flatModifier = _flatModifiers.FirstOrDefault(x => x.Value == v);

            if (flatModifier != null)
                _flatModifiers.Remove(flatModifier);
        }
    }
}
