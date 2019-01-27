using Finisher.Characters.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private WeaponColliderManager colliderManager;
    // Start is called before the first frame update
    void Start()
    {
        colliderManager = GetComponent<WeaponColliderManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    [SerializeField] private GameObject soulEffect;
    public void toggleSoul()
    {
        if (soulEffect.gameObject.activeInHierarchy)
        {

            soulEffect.gameObject.SetActive(false);
            colliderManager.currentBonus = 0;
        }
        else {
            soulEffect.gameObject.SetActive(true);
            colliderManager.currentBonus = colliderManager.soulBonusDamage;
        }
    }
    public void soulOn() {
        soulEffect.gameObject.SetActive(true);
        colliderManager.currentBonus = colliderManager.soulBonusDamage;
    }

    public void soulOff() {
        soulEffect.gameObject.SetActive(false);
        colliderManager.currentBonus = 0;
    }
}
