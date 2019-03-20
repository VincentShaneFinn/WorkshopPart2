using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBackBlade : MonoBehaviour
{

    [SerializeField] GameObject backBlade;
    [SerializeField] GameObject bladeMeshObject;

    public void HideBladeMesh()
    {
        bladeMeshObject.SetActive(false);
    }

    void ShowBackBlade()
    {
        backBlade.SetActive(true);
    }

    void HideBackBlade()
    {
        backBlade.SetActive(false);
        bladeMeshObject.SetActive(true);
    }

}
