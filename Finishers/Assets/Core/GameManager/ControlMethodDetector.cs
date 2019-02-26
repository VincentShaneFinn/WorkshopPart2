using UnityEngine;

public enum ControlType { Xbox, PS4, PC }

public class ControlMethodDetector : MonoBehaviour {

    private static ControlType currentControlType; public static ControlType GetCurrentControlType() { return currentControlType; }

    void Update()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                currentControlType = ControlType.PS4;
            }
            else if (names[x].Length == 33)
            {
                currentControlType = ControlType.Xbox;
            }
            else
            {
                currentControlType = ControlType.PC;
            }
        }
    }
}