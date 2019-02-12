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

    //Heal Cheat
    public static bool HealCheat()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.Alpha3);
        return rv;
    }

    //Finisher Meter Cheat
    public static bool FinisherMeterCheat()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.Alpha1);
        return rv;
    }

    //Volatility Cheat
    public static bool VolatilityCheat()
    {
        bool rv = false;
        rv = Input.GetKeyDown(KeyCode.Alpha2);
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
