using Finisher.Characters.Systems;
using UnityEngine;

namespace Finisher.Characters.Enemies.BreakableParts
{
    public class BreakableHealthBar : MonoBehaviour
    {
        [SerializeField] private BreakablePart breakablePart;
        private HealthSystem parentHealthSystem = null;
        void Start()
        {
            parentHealthSystem = GetComponentInParent<HealthSystem>();
            if (parentHealthSystem != null)
            {
                parentHealthSystem.OnDamageTaken += detach;
            }
        }

        void OnDestroy()
        {
            if (parentHealthSystem != null)
            {
                parentHealthSystem.OnDamageTaken -= detach;
            }
        }

        public void detach()
        {
            gameObject.SetActive(false);
            breakablePart.gameObject.SetActive(true);
            parentHealthSystem.OnDamageTaken -= detach;
        }
    }
}