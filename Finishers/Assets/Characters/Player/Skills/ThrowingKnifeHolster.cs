using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Skills
{
    public class ThrowingKnifeHolster : MonoBehaviour
    {
        [SerializeField] ThrowingWeapon throwingWeapon;
        private ThrowingWeapon currentThrowingWeapon = null;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (currentThrowingWeapon == null)
                {
                    currentThrowingWeapon = Instantiate(throwingWeapon, transform.position, transform.rotation);
                    currentThrowingWeapon.transform.parent = transform;
                }
            }
            if(currentThrowingWeapon && Input.GetButtonDown(InputNames.SpecialAttack))
            {
                currentThrowingWeapon.ThrowWeapon();
                currentThrowingWeapon = null;
            }
            else if (currentThrowingWeapon)
            {
                Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
            }
        }

    }
}
