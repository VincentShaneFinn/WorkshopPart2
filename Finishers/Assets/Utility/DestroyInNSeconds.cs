using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInNSeconds : MonoBehaviour
{
    [SerializeField] float NSeconds = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, NSeconds);
    }

}
