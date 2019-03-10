using UnityEngine;

public enum ControlType { Xbox, PS4, PC }

public class ControlMethodDetector : MonoBehaviour
{

    private static ControlType currentControlType; public static ControlType GetCurrentControlType() { return currentControlType; }

    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    void Update()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            //print(names[x].Length);
            if (names[x].Length == 19)
            {
                print("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                print("XBOX ONE CONTROLLER IS CONNECTED");
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;

            }
        }


        if (Xbox_One_Controller == 1)
        {
            currentControlType = ControlType.Xbox;
        }
        else if (PS4_Controller == 1)
        {
            currentControlType = ControlType.PS4;
        }
        else
        {
            currentControlType = ControlType.PC;
        }
    }
}