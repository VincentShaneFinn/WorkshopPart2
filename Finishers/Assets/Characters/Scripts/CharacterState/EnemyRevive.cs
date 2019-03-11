using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Finisher.Characters
{
    public class EnemyRevive : MonoBehaviour
    {
        protected Systems.HealthSystem healthSystem;
        protected CharacterState characterState;
        // Start is called before the first frame update
        void Awake()
        {
            characterState = GetComponent<CharacterState>();
            healthSystem = GetComponent<Systems.HealthSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                characterState.DyingState.Revive();
                healthSystem.Revive();
            }
        }
    }
}