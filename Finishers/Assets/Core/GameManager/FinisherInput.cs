using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherInput : MonoBehaviour
{
    //Special Attack
    public static bool SpecialAttack()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.SpecialAttack);
        return rv;
    }

    //Sprinting
    public static bool isSpriting()
    {
        bool rv = false;
        rv = Input.GetButton(InputNames.Sprint);
        return rv;
    }

    internal static bool Continue()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }

    //Light Attack
    public static bool LightAttack()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.LightAttack);
        return rv;
    }

    //Heavy Attack
    public static bool HeavyAttack()
    {
        bool rv = false;
        if (ControlMethodDetector.GetCurrentControlType() == ControlType.Xbox)
        {
            rv = (Input.GetAxisRaw(InputNames.HeavyAttack) > 0);
        }
        else
        {
            rv = Input.GetButtonDown(InputNames.HeavyAttack);
        }
        return rv;
    }

    //Dodge
    public static bool Dodge()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.Dodge) || Input.GetKeyDown(KeyCode.Mouse3);
        return rv;
    }

    //Parry
    public static bool Parry()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.Parry) || Input.GetKeyDown(KeyCode.Mouse4);
        return rv;
    }

    //Grab
    public static bool Grab()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.Grab);
        return rv;
    }

    //Finisher
    public static bool Finisher()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.Finisher);
        return rv;
    }

    //Finisher1
    public static bool Finisher1()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.SelectFinisher1);
        return rv;
    }

    //Finisher2
    public static bool Finisher2()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.SelectFinisher2);
        return rv;
    }
    public static bool Finisher4()
    {
        bool rv = false;
        rv = Input.GetButtonDown(InputNames.SelectFinisher4);
        return rv;
    }

    static float previousHealDPadX = 0;
    //Heal Cheat
    public static bool HealCheat()
    {
        bool rv = false;
        float DPadX = Input.GetAxis("DPadX");
        if (DPadX == 1 && previousHealDPadX == 0)
        {
            previousHealDPadX = DPadX;
            return true;
        }
        previousHealDPadX = DPadX;
        rv = Input.GetKeyDown(KeyCode.Alpha3);
        return rv;
    }

    static float previousFinisherDPadX = 0;
    //Finisher Meter Cheat
    public static bool FinisherMeterCheat()
    {
        bool rv = false;
        float DPadX = Input.GetAxis("DPadX");
        if (DPadX == -1 && previousFinisherDPadX == 0)
        {
            previousFinisherDPadX = DPadX;
            return true;
        }
        previousFinisherDPadX = DPadX;
        rv = Input.GetKeyDown(KeyCode.Alpha1);
        return rv;
    }

    static float previousvolDPadY = 0;
    //Volatility Cheat
    public static bool VolatilityCheat()
    {
        bool rv = false;
        float DPadY = Input.GetAxis("DPadY");
        if (DPadY == 1 && previousvolDPadY == 0)
        {
            previousvolDPadY = DPadY;
            return true;
        }
        previousvolDPadY = DPadY;
        rv = Input.GetKeyDown(KeyCode.Alpha2);
        return rv;
    }

    static float previousinvDPadY = 0;
    //Invulnerability Cheat
    public static bool InvulnerabilityCheat()
    {
        bool rv = false;
        float DPadY = Input.GetAxisRaw("DPadY");
        if (DPadY == -1 && previousinvDPadY == 0)
        {
            previousinvDPadY = DPadY;
            return true;
        }
        previousinvDPadY = DPadY;
        rv = Input.GetKeyDown(KeyCode.I);
        return rv;
    }

    //Interact
    public static bool Interact()
    {
        bool rv = false;
        rv = Input.GetButton("Interact");
        return rv;
    }

    //Pause
    public static bool Pause()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.Escape);
        return rv;
    }

    //ReloadScene
    public static bool ReloadScene()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.Return);
        return rv;
    }

    //Play Cutscene
    public static bool PlayCutscene()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.P);
        return rv;
    }

}
