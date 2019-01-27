using UnityEngine;
using UnityEngine.UI;

namespace Finisher.UI
{

    public class PlayerUIObjects : MonoBehaviour 
    {
        [SerializeField] public GameObject PauseMenuObject;

        // todo encapsulate into a bottom left player ui class
        [SerializeField] public Image InFinisherIndicator;
        [SerializeField] public GameObject FinisherComboObject;
        [SerializeField] public Text FinisherComboText;
    }
}