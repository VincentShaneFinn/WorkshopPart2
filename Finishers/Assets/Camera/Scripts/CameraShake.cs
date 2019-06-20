using Finisher.Characters.Player.Systems;
using System.Collections;
using UnityEngine;

namespace Finisher.Cameras
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 _initialCameraPosition;

        private float _remainingShakeTime;

        private PlayerCombatSystem playerCombatSystem;
        private PlayerHealthSystem playerHealthSystem;

        public void shake()
        {
            Shake();
        }

        private void Awake()
        {
            _initialCameraPosition = transform.localPosition;

            playerCombatSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<PlayerCombatSystem>();
            playerHealthSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<PlayerHealthSystem>();
        }

        private void Start()
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

        private void OnDestroy()
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

        private void Update()
        {
            if (_remainingShakeTime <= 0)
            {
                transform.localPosition = _initialCameraPosition;
                return;
            }

            _remainingShakeTime -= Time.deltaTime;
        }

        private void Shake(float strength = 0.5f, float duration = 0.1f)
        {
            _remainingShakeTime = duration;

            if (_remainingShakeTime > 0)
            {
                StartCoroutine(ShakeCamera(strength, duration));
            }
        }

        private IEnumerator ShakeCamera(float strength, float duration)
        {
            float delay = 0.01f;
            float moveTimes = duration / delay;

            for (int i = 0; i < moveTimes; i++)
            {
                Vector3 startPos = transform.position;
                Vector3 newPos = Random.insideUnitCircle * strength / 4;
                newPos += startPos;

                var currentPos = transform.position;
                var t = 0f;
                while (t < 1)
                {
                    t += Time.deltaTime / delay;
                    transform.position = Vector3.Lerp(currentPos, newPos, t);
                    yield return null;
                }
            }
        }
    }
}