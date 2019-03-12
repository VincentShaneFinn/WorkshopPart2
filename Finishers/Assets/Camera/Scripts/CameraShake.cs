using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private float _strength;

    private Vector3 _initialCameraPosition;
    private float _remainingShakeTime;

    private void Awake()
    {
        _initialCameraPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_remainingShakeTime <= 0)
        {
            transform.localPosition = _initialCameraPosition;
            return;
        }

        transform.Translate(Random.insideUnitCircle * _strength);

        _remainingShakeTime -= Time.deltaTime;
    }

    public void Shake(float strength = 1, float duration = 0.1f)
    {
        _strength = strength;
        _remainingShakeTime = duration;
    }

}