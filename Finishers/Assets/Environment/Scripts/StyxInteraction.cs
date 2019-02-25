using Finisher.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StyxInteraction : MonoBehaviour
{
    public AnimationClip animationToPlay;
    public GameObject vialUI; // use PLayerUI>BottumLeft>VialIcon
    public Sprite fullVial;  // use Assests>UI>UIButtons>Textures>VialFull

    protected bool interactable = false;

    // Start is called before the first frame update
    void Start()
    {
        interactable = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (FinisherInput.Interact() && interactable)
        {
            other.GetComponent<CharacterState>().EnterInvulnerableActionState(animationToPlay);
            StartCoroutine(pickupItem());
            interactable = false;
            vialUI.GetComponent<Image>().sprite = fullVial;
        }
    }

    IEnumerator pickupItem()
    {

        yield return new WaitForSeconds(1f);

        //DoSomethin

    }
}
