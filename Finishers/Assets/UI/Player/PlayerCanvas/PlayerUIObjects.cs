using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.UI
{

    public class PlayerUIObjects : MonoBehaviour 
    {
        [SerializeField] public GameObject PauseMenuObject;

        // todo encapsulate into a bottom left player ui class
        [SerializeField] public Image HealthBar;
        [SerializeField] public Image FinisherBar;
        [SerializeField] public Image InFinisherIndicator;
    }
}