using System;

namespace MIG.Scripts.Attributes
{
    [Serializable]
    public class Modifier
    {
        public float Value { get; set; }

        public Modifier(float value)
        {
            Value = value;
        }
    }
}
