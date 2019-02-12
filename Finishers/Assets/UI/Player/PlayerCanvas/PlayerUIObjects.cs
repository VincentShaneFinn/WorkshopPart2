using UnityEngine;
using UnityEngine.UI;

namespace Finisher.UI
{

    public class PlayerUIObjects : MonoBehaviour 
    {
        [SerializeField] public GameObject PauseMenuObject;

        // todo encapsulate into a bottom left player ui class
        [SerializeField] public Image InFinisherIndicator;
        public CursorMode cursorMode = CursorMode.ForceSoftware;
        public Texture2D cursorTexture;
        public Vector2 hotSpot;


        void Start()
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
    }

}