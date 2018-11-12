using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIG.Scripts.Common.Events
{
    public class InfoEventArgs<T> : EventArgs 
    {
        public T Info { get; internal set; }

        public InfoEventArgs()
        {
            Info = default(T);
        }

        public InfoEventArgs(T info)
        {
            Info = info;
        }
    }
}
