using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillBox : MonoBehaviour
{

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == TagNames.PlayerTag)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
