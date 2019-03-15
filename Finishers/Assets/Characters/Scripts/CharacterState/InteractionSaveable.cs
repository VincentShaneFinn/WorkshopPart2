using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionSaveable : MonoBehaviour
{
    public int uniqueId;
    public bool interacted = false;
    public abstract void runInteraction();
}
