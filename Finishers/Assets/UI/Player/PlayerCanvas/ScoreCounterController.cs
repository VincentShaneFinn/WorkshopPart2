using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Finisher.Characters.Systems;

namespace Finisher.UI
{
    public class ScoreCounterController : MonoBehaviour
    {
        [SerializeField] private GameObject comboObject;
        [SerializeField] private Text comboText;

        CombatSystem playerCombatSystem;

        private int currentCombo;

        void Start()
        {
            playerCombatSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<CombatSystem>();

            changeComboText(0);

            if (playerCombatSystem)
            {
                playerCombatSystem.OnHitCounterChange += changeComboText;
            }
        }

        void OnDestroy()
        {
            if (playerCombatSystem)
            {
                playerCombatSystem.OnHitCounterChange -= changeComboText;
            }
        }

        private void changeComboText(int value)
        {
            currentCombo = value;
            if(currentCombo >= 3)
            {
                comboObject.SetActive(true);
            }
            else
            {
                comboObject.SetActive(false);
            }

            comboText.text = currentCombo.ToString();
        }
    }
}