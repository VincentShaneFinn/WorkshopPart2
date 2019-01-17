using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] private FinisherSkillsDamageSystem finisherSkillsDamageSystem;
        public float FinisherMeterCost { get { return finisherSkillsDamageSystem.FinisherMeterCost; } }

        [SerializeField] private float destroyInNSeconds = 1f;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; }

            var targetHealthSystem = col.gameObject.GetComponent<HealthSystem>();
            if (targetHealthSystem) // hit an enemy
            {
                finisherSkillsDamageSystem.HitCharacter(targetHealthSystem);
            }
        }

    }
}
