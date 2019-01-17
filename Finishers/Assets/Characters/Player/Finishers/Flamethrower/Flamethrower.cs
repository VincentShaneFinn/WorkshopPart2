using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] private FinisherSkillsDamageSystem flamethrowerDamageSystem;
        public float FinisherMeterCost { get { return flamethrowerDamageSystem.FinisherMeterCost; } }

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
                flamethrowerDamageSystem.HitCharacter(targetHealthSystem);
            }
        }

    }
}
