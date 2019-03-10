using Finisher.Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTargetIndicatorController : MonoBehaviour
{
    public PlayerCharacterController player;
    protected Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCombatTargetIndicator(player.CombatTarget != null ? player.CombatTarget.gameObject : null);
    }

    private void UpdateCombatTargetIndicator(GameObject target)
    {
        if (target)
        {
            if (!renderer.enabled)
            {
                renderer.enabled = true;
            }
            transform.parent = target.transform;
            float height = target.GetComponent<CapsuleCollider>().height;
            transform.localPosition = new Vector3(0,
                height + (.1f * height), 0);
        }
        else
        {
            if (renderer.enabled)
            {
                renderer.enabled = false;
            }
        }
    }
}
