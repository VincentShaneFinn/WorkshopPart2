using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Player.Finishers
{
    public class Blade : MonoBehaviour
    {
        [SerializeField] private float speedrate = 1;
        [SerializeField] private float startSpeed = 1;
        [SerializeField] private float maxSpeed = 5f;

        [HideInInspector] public HealthSystem target;
        [HideInInspector] public FinisherSkillsDamageSystem damageSystem;
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Launch()
        {
            StartCoroutine(floatUpCoroutine());
        }

        IEnumerator floatUpCoroutine()
        {
            var startingPoint = transform.position;
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            while (transform.position.y - startingPoint.y < 2f)
            {
                transform.Translate(0, startSpeed, startSpeed, transform);
                speedUpdate();
                yield return null;
            }

            var descentSpeed = startSpeed;
            while ((transform.position - (target.transform.position + Vector3.up)).magnitude > 0.3f)
            {
                var goal = target.transform.position + Vector3.up;

                transform.Translate(0,0 , startSpeed, transform);
                speedUpdate();
                transform.LookAt(target.transform.position + Vector3.up);

                yield return null;
            }

            damageSystem.HitCharacter(gameObject, target);
            Destroy(gameObject);

        }

        private void speedUpdate()
        {
            if (startSpeed < maxSpeed)
            {
                startSpeed += speedrate * Time.deltaTime;
            }
        }

    }
}
