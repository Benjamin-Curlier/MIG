using MIG.Scripts.Common.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIG.Scripts.Common.Extensions
{
    public static class EnumExtensions
    {
        public static bool HasFlag(this InputType actual,  InputType flag)
        {
            return (actual & flag) == flag;
        }
    }
}
