﻿using UnityEngine;

namespace Finisher.Core.Loader
{
    // THIS IS ATTACHED TO THE MAIN CAMERA IN CAMERA RIG, AND IS THE FIRST THING TO DRAG INTO EACH SCENE
    [DisallowMultipleComponent]
    public class Loader : MonoBehaviour
    {
        [SerializeField] GameObject gameManager;
        [SerializeField] GameObject controlMethodDetector;

        void Awake()
        {
            //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
            if (GameManager.instance == null)
            {
                Instantiate(gameManager);
            }

            if(ControlMethodDetector.instance == null)
            {
                Instantiate(controlMethodDetector);
            }
        }

    }
}
