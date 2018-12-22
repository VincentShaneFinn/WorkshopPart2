using UnityEngine;

namespace Finisher.Core.Loader
{
    // THIS IS ATTACHED TO THE MAIN CAMERA IN CAMERA RIG, AND IS THE FIRST THING TO DRAG INTO EACH SCENE
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
