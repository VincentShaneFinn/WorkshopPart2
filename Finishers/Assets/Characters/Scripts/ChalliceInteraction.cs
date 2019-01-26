using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalliceInteraction : MonoBehaviour
{
    protected bool interactable = false;
    public GameObject effect;
    // Start is called before the first frame update
    void Start()
    {
        interactable = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && interactable)
        {
            GameObject obj = Instantiate(effect);
            obj.transform.position = transform.position;
            obj.transform.position += new Vector3(0, 1, 0);
            interactable = false;
        }
    }
}
