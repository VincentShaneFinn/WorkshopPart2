using Finisher.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFinisherModeIcon : MonoBehaviour
{

    private Image image;
    public bool reverse = false;
    public CharacterStateSO playerState;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerState.IsFinisherModeActive)
        {
            image.enabled = reverse;
        }
        else
        {
            image.enabled = !reverse;
        }
    }
}
