using UnityEngine;
using System.Collections;

using Finisher.Characters.Player.Systems;

namespace Finisher.Cameras
{
    public class CameraShake : MonoBehaviour
    {
        private float _strength;

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
            if(playerHealthSystem)
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

            transform.Translate(Random.insideUnitCircle * _strength);

            _remainingShakeTime -= Time.deltaTime;
        }

        private void shake()
        {
            Shake();
        }

        private void Shake(float strength = .25f, float duration = 0.1f)
        {
            _strength = strength;
            _remainingShakeTime = duration;
        }
    }
}