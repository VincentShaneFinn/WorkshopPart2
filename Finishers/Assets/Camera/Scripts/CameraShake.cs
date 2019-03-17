using UnityEngine;
using System.Collections;

using Finisher.Characters.Player.Systems;

namespace Finisher.Cameras
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 _initialCameraPosition;

        private float _remainingShakeTime;

        private PlayerCombatSystem playerCombatSystem;
        private PlayerHealthSystem playerHealthSystem;

        private void Awake()
        {
            _initialCameraPosition = transform.localPosition;

            playerCombatSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<PlayerCombatSystem>();
            playerHealthSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<PlayerHealthSystem>();
        }

        void Start()
        {            
            if (playerCombatSystem)
            {
                playerCombatSystem.OnHitCameraShake += Shake;
            }

            if (playerHealthSystem)
            {
                playerHealthSystem.OnDamageTaken += shake;
            }
        }

        void OnDestroy()
        {
            if (playerCombatSystem)
            {
                playerCombatSystem.OnHitCameraShake -= Shake;
            }
            if (playerHealthSystem)
            {
                playerHealthSystem.OnDamageTaken -= shake;
            }
        }

        void Update()
        {
            if (_remainingShakeTime <= 0)
            {
                transform.localPosition = _initialCameraPosition;
                return;
            }            

            _remainingShakeTime -= Time.deltaTime;
        }

        private void shake()
        {
            Shake();
        }

        private void Shake(float strength = 0.5f, float duration = 0.1f)
        {
            _remainingShakeTime = duration;

            if (_remainingShakeTime > 0)
            {
                StartCoroutine(ShakeCamera(strength, duration));
            }
        }

        IEnumerator ShakeCamera(float strength, float duration)
        {
            float delay = 0.01f;
            float moveTimes = duration / delay;
            
            for (int i = 0; i < moveTimes; i++)
            {
                Vector3 newPos = Random.insideUnitCircle * strength;

                transform.Translate(Vector3.Lerp(transform.localPosition, newPos, Time.deltaTime));
                yield return new WaitForSeconds(delay);
            }
        }
    }
}