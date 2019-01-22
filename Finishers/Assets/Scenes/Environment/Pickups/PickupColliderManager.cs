using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Environment.Pickups
{
    public class PickupColliderManager : MonoBehaviour
    {
        [SerializeField] private float amount;
        [SerializeField] private PickupType pickupType;

        private enum PickupType { Health, Finisher };
        private bool GrantedSomething = false;

        void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.tag == TagNames.PlayerTag)
            {
                switch (pickupType)
                {
                    case PickupType.Health:
                        GrantHealth(collider.gameObject.GetComponent<HealthSystem>());
                        break;
                    case PickupType.Finisher:
                        GrantFinisher(collider.gameObject.GetComponent<FinisherSystem>());
                        break;
                }
                if (GrantedSomething)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void GrantHealth(HealthSystem playerHealthSystem)
        {
            if (playerHealthSystem)
            {
                GrantedSomething = true;
                if (playerHealthSystem.GetHealthAsPercent() >= 1 - Mathf.Epsilon)
                {
                    GrantedSomething = false;
                }
                playerHealthSystem.IncreaseHealth(amount);
            }
        }

        private void GrantFinisher(FinisherSystem playerFinisherSystem)
        {
            if (playerFinisherSystem)
            {
                GrantedSomething = true;
                if (playerFinisherSystem.GetFinisherMeterAsPercent() >= 1 - Mathf.Epsilon)
                {
                    GrantedSomething = false;
                }
                playerFinisherSystem.GainFinisherMeter(amount);
            }
        }

    }
}
