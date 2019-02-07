using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotation : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Transform parent;
    [SerializeField] private float offset = 0;
    [SerializeField] private float offrate = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (particle == null) {
            particle = GetComponent<ParticleSystem>();
        }
        var main = particle.main;
        var rot = particle.main.startRotation;
        float y = parent.rotation.eulerAngles.y;
        rot.mode = ParticleSystemCurveMode.Constant;
        rot.constant = (y + offset) * Mathf.Deg2Rad * -1 * offrate;
        main.startRotation = rot;
    }
}
