using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionSaveable : Saveable
{
    public bool interacted = false;
    public abstract void runInteraction();
}
