using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherInput : MonoBehaviour
{
    //Special Attack         
    public static bool SpecialAttack()
    {
        switch(ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetAxisRaw("XBOXLeftTrigger") > 0;
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.SpecialAttack);
        }
        return false;
    }

    //Sprinting
    public static bool isSpriting()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXLeftStickButton");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.Sprint);
        }
        return false;
    }

    //Light Attack
    public static bool LightAttack()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXRightBumper");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.LightAttack);
        }
        return false;
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
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXLeftBumper");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.Dodge) || Input.GetKeyDown(KeyCode.Mouse3);
        }
        return false;
    }

    //Parry
    public static bool Parry()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetAxisRaw("XBOXLeftTrigger") > 0;
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.Parry) || Input.GetKeyDown(KeyCode.Mouse4);
        }
        return false;
    }

    //Grab
    public static bool Grab()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXRightStickButton");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.Grab);
        }
        return false;
    }

    //Finisher
    public static bool Finisher()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXRightStickButton") && Input.GetButton("XBOXLeftStickButton");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.Finisher);
        }
        return false;
    }

    //Finisher1
    public static bool Finisher1()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXX");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.SelectFinisher1);
        }
        return false;
    }

    //Finisher2
    public static bool Finisher2()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXY");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButtonDown(InputNames.SelectFinisher2);
        }
        return false;
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
        float DPadX = 0;
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.PS4:
                DPadX = Input.GetAxis("DPadX");
                break;
            case ControlType.Xbox:
                DPadX = Input.GetAxis("XBOXDPadX");
                break;
            case ControlType.PC:
                return Input.GetKeyDown(KeyCode.Alpha3);
        }
        if (DPadX == 1 && previousHealDPadX == 0)
        {
            previousHealDPadX = DPadX;
            return true;
        }

        previousHealDPadX = DPadX;
        return false;
    }

    static float previousFinisherDPadX = 0;
    //Finisher Meter Cheat
    public static bool FinisherMeterCheat()
    {
        float DPadX = 0;
        switch(ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.PS4:
                DPadX = Input.GetAxis("DPadX");
                break;
            case ControlType.Xbox:
                DPadX = Input.GetAxis("XBOXDPadX");
                break;
            case ControlType.PC:
                return Input.GetKeyDown(KeyCode.Alpha1);
        }
        if (DPadX == -1 && previousFinisherDPadX == 0)
        {
            previousFinisherDPadX = DPadX;
            return true;
        }

        previousFinisherDPadX = DPadX;
        return false;
    }

    static float previousvolDPadY = 0;
    //Volatility Cheat
    public static bool VolatilityCheat()
    {
        float DPadY = 0;
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.PS4:
                DPadY = Input.GetAxis("DPadY");
                break;
            case ControlType.Xbox:
                DPadY = Input.GetAxis("XBOXDPadY");
                break;
            case ControlType.PC:
                return Input.GetKeyDown(KeyCode.Alpha2);
        }
        if (DPadY == 1 && previousvolDPadY == 0)
        {
            previousvolDPadY = DPadY;
            return true;
        }
        previousvolDPadY = DPadY;
        return false;
    }

    static float previousinvDPadY = 0;
    //Invulnerability Cheat
    public static bool InvulnerabilityCheat()
    {
        float DPadY = 0;
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.PS4:
                DPadY = Input.GetAxis("DPadY");
                break;
            case ControlType.Xbox:
                DPadY = Input.GetAxis("XBOXDPadY");
                break;
            case ControlType.PC:
                return Input.GetKeyDown(KeyCode.I);
        }
        
        if (DPadY == -1 && previousinvDPadY == 0)
        {
            previousinvDPadY = DPadY;
            return true;
        }
        previousinvDPadY = DPadY;
        return false;
    }

    //Interact
    public static bool Interact()
    {
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                return Input.GetButtonDown("XBOXY");
            case ControlType.PS4:
            case ControlType.PC:
                return Input.GetButton("Interact");
        }
        return false;
    }

    //Pause
    public static bool Pause()
    {
        bool rv = false;
        switch (ControlMethodDetector.GetCurrentControlType())
        {
            case ControlType.Xbox:
                rv = Input.GetButtonDown("XBOXStart");
                break;
            case ControlType.PS4:
                rv = Input.GetButtonDown("PS4Start");
                break;
        }
        rv = rv || Input.GetKeyDown(KeyCode.Escape);
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
