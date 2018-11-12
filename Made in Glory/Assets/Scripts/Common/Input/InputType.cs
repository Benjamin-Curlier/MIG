using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIG.Scripts.Common.Input
{
    [Serializable]
    [Flags]
    public enum InputType
    {
        KEYBOARD = 0x1,
        GAMEPAD = 0x2,
        TOUCHSCREEN = 0x4
    }
}
