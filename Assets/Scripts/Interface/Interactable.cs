using System;

using System.Collections;

using System.Collections.Generic;

using System.Numerics;

using UnityEngine;



/// <summary>

/// Interface for interactable object.

/// </summary>

public interface Interactable {

    /// <summary>

    /// Use this to store wetaher the object is interactable.

    /// </summary>

    bool isInteractable {

        get;

        set;

    }



    /// <summary>

    /// Is called when object is intreacted with.

    /// </summary>

    void Interact() {

        Interact(false);

    }



    /// <summary>

    /// Is called when object is intreacted with.

    /// </summary>

    /// <param name="isThreatened"> Indicate if is threatened </param>

    void Interact(bool isThreatened = false) {

        Debug.LogWarning("Interact() not implemented");

    }



    /// <summary>

    /// Is called when object is stared at.

    /// </summary>

    void Stared() {

        Debug.LogWarning("Stared() not implemented");

    }

}