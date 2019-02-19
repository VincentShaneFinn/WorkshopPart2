using Finisher.Characters.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionBreak : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HealthSystem c=null;
        Transform t=transform;
        while (c == null && t.parent != null) {
            c=t.gameObject.GetComponentInParent<HealthSystem>();
            t = t.parent;
        }
        if (c != null)
        {
            c.OnDamageTaken += detach;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void detach()
    {
        GameObject g= Instantiate(gameObject,transform.position,transform.rotation);
        g.transform.SetParent(null);
        g.AddComponent<Rigidbody>();
        Destroy(gameObject);
    }
}
