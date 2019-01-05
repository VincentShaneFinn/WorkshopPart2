using UnityEngine;

namespace Finisher.Characters.Finishers
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float destroyInNSeconds = 1f;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; }

            var healthSystem = col.gameObject.GetComponent<HealthSystem>();
            if (healthSystem) // hit an enemy
            {
                healthSystem.DamageHealth(damage);
            }
        }

    }
}
