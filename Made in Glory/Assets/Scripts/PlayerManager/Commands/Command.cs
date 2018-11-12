using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIG.Scripts.Commands;
using System;

// The base class from which all commands inherit. Used as parameter for referee & playercontroller i guess
// The type of the command and the parameters are to be determined by the class type. SO INHERIT AWAY MOTHERFUCKERS
namespace MIG.Scripts.Commands
{
    public class Command : ICommand
    {
        protected GameObject Player;

        protected Command(GameObject Player)
        {
            this.Player = Player;
        }

        public virtual bool EvaluateCommand()
        {
            throw new NotImplementedException();
        }

        public virtual void ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
