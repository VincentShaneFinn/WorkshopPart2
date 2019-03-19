using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistance
{
    private Persistance()
    {
        // Prevent outside instantiation
    }

    private static readonly Persistance _singleton = new Persistance();

    public static Persistance GetSingleton()
    {
        return _singleton;
    }

    public bool riposteTutorialReady = true;
    public bool finisherTutorialReady = true;
}

