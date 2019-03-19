using Finisher.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChalliceInteraction : InteractionSaveable
{
    public AnimationClip animationToPlay;

    protected bool interactable = false;
    public GameObject effect;

    public GameObject bossStatue;
    public GameObject bossFireEffect;
    public GameObject bossEnemy;

    public GameObject vialUI; // use PLayerUI>BottumLeft>VialIcon
    public Sprite emptyVial;  // use Assests>UI>UIButtons>Textures>VialEmpty

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
            GetComponent<InteractionSaveable>().interacted = true;
            other.GetComponent<CharacterState>().spawnConfig = new SpawnConfig();
            lightTorch();
        }
    }

    private void lightTorch()
    {
        StartCoroutine(lightTorchSequence());
        interactable = false;
        vialUI.GetComponent<Image>().sprite = emptyVial;
    }

    IEnumerator lightTorchSequence()
    {
        GameObject obj = Instantiate(effect);
        obj.transform.position = transform.position;
        obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        yield return new WaitForSeconds(1f);

        bossStatue.SetActive(false);

        yield return null;

        bossFireEffect.SetActive(true);
        bossEnemy.SetActive(true);

        yield return new WaitForSeconds(1f);

        bossFireEffect.SetActive(false);

    }

    public override void runInteraction()
    {
        lightTorch();
    }
}
