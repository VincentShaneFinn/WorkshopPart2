using UnityEngine;

public enum ControlType { Xbox, PS4, PC }

public class ControlMethodDetector : MonoBehaviour
{

    private static ControlType currentControlType; public static ControlType GetCurrentControlType() { return currentControlType; }

    void Update()
    {
        string[] names = Input.GetJoystickNames();
        int index = 0;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length > 0)
            {
                index = i;
                break;
            }
        }
        if (names[index].Length == 19)
        {
            currentControlType = ControlType.PS4;
        }
        else if (names[index].Length == 33)
        {
            currentControlType = ControlType.Xbox;
        }
        else
        {
            currentControlType = ControlType.PC;
        }
    }
}