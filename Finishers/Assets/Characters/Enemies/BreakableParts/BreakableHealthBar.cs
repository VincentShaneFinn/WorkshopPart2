using Finisher.Characters.Systems;
using UnityEngine;

namespace Finisher.Characters.Enemies.BreakableParts
{
    public class BreakableHealthBar : MonoBehaviour
    {
        private int hp = 3;

        [SerializeField] private BreakablePart breakablePart;
        private HealthSystem parentHealthSystem = null;
        void Start()
        {
            parentHealthSystem = GetComponentInParent<HealthSystem>();
            if (parentHealthSystem != null)
            {
                parentHealthSystem.OnDamageTaken += hitArmor;
            }
        }

        void OnDestroy()
        {
            if (parentHealthSystem != null)
            {
                parentHealthSystem.OnDamageTaken -= hitArmor;
            }
        }

        public void hitArmor()
        {
            if(UnityEngine.Random.Range(0,1f) < 0.5f)
            {
                hp--;
            }
            if (hp <= 0)
            {
                breakablePart.gameObject.SetActive(true);
                breakablePart.gameObject.transform.SetParent(null);
                Destroy(gameObject);
            }
        }
    }
}