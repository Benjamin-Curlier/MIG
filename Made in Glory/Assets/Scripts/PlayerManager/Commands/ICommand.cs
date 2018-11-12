using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIG.Scripts.Commands;

/// <summary>
/// The command interface. Contains the evaluation & execution functions
/// </summary>
public interface ICommand {

    /// <summary>
    /// Evaluates a move in a game, to see wether it's legal or not.
    /// </summary>
    /// <returns>True if the move is legal, false if not.</returns>
    bool EvaluateCommand();

    /// <summary>
    /// Executes a given command. You MUST call evaluate first and check if the command is valid
    /// </summary>
    void ExecuteCommand();

}
