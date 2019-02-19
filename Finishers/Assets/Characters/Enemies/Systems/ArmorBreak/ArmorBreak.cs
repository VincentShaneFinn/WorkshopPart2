using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBreak : MonoBehaviour
{
    [SerializeField] Joint joint;

    public void breakArmor() {
        Destroy(joint);
    }
}
