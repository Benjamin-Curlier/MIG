using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MIG.Scripts.Attributes
{
    [Serializable]
    public class FlatModifier
    {
        public int Value { get; set; }

        public FlatModifier(int value)
        {
            Value = value;
        }
    }
}
