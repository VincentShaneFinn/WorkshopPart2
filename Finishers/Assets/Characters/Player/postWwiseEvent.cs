using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class postWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event MyEvent;
    // Use this for initialization.
    public void PlayFootstepSound()
    {
        MyEvent.Post(gameObject);
    }
}
