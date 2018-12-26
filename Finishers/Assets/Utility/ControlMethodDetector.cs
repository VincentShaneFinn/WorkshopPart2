using UnityEngine;

public enum ControlType { Xbox, PS4, PC }

public class ControlMethodDetector : MonoBehaviour {

    public static ControlMethodDetector instance = null;

    private static ControlType currentControlType; public static ControlType GetCurrentControlType() { return currentControlType; }
    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;

    // Start is called before the first frame update
    void Awake()
    {
        SingletonSetup();
    }

    private void SingletonSetup()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        string[] names = Input.GetJoystickNames();
        PS4_Controller = 0;
        Xbox_One_Controller = 0;
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
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
