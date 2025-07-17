using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface for attackable object.
/// </summary>
public interface Attackable {
    /// <summary>
    /// Use this to store wetaher the object is interactable.
    /// </summary>
    bool isAttackable {
        get;
        set;
    }

    /// <summary>
    /// Is called when object is attcked.
    /// </summary>
    void Attacked();

    /// <summary>
    /// Is called when object is aimed at.
    /// </summary>
    void Aimed();
}
