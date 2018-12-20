using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Core
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] GameObject gameManager;

        void Awake()
        {
            //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
            if (GameManager.instance == null)
            {
                //Instantiate gameManager prefab
                Instantiate(gameManager);
            }
        }

    }
}
