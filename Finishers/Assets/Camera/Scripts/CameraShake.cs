using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public float Strength;
    public float Duration;

    private Vector3 _initialCameraPosition;
    private float _remainingShakeTime;

    private void Awake()
    {
        _initialCameraPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("shake");
            Shake();
        }

        if (_remainingShakeTime <= 0)
        {
            transform.localPosition = _initialCameraPosition;
            return;
        }

        transform.Translate(Random.insideUnitCircle * Strength);

        _remainingShakeTime -= Time.deltaTime;
    }

    public void Shake()
    {
        _remainingShakeTime = Duration;
    }

}