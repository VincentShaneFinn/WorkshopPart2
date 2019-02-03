using Finisher.Characters;
using System.Collections;
using UnityEngine;

public class StyxInteraction : MonoBehaviour
{
    public AnimationClip animationToPlay;

    protected bool interactable = false;

    // Start is called before the first frame update
    void Start()
    {
        interactable = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E) && interactable)
        {
            other.GetComponent<CharacterState>().EnterInvulnerableActionState(animationToPlay);
            StartCoroutine(pickupItem());
            interactable = false;
        }
    }

    IEnumerator pickupItem()
    {

        yield return new WaitForSeconds(1f);

        //DoSomethin

    }
}
