using UnityEngine;

namespace Finisher.Core
{
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        public bool GamePaused;

        // Start is called before the first frame update
        void Awake()
        {
            SingletonSetup();
            InitGame();
        }

        private void SingletonSetup()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        private void InitGame()
        {
            print("Initialize Game");
        }
    }
}
