using Finisher.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChalliceInteraction : InteractionSaveable
{
    static int challicesLit;
    public int challicesNeeded;
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
        challicesLit = 0;
        challicesNeeded = 0;
        interactable = true;
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] objects = scene.GetRootGameObjects();
        foreach (GameObject obj in objects)
        {
            ChalliceInteraction[] challices = obj.GetComponentsInChildren<ChalliceInteraction>();
            challicesNeeded += challices.Length;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (FinisherInput.Interact() && interactable)
        {
            other.GetComponent<CharacterState>().EnterInvulnerableActionState(animationToPlay);
            GetComponent<InteractionSaveable>().interacted = true;
            other.GetComponent<CharacterState>().spawnConfig = new SpawnConfig();
            vialUI.GetComponent<Image>().sprite = emptyVial;
            lightTorch();
        }
    }

    private void lightTorch()
    {
        challicesLit++;
        GameObject obj = Instantiate(effect);
        obj.transform.position = transform.position;
        obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        if (challicesLit == challicesNeeded)
        {
            StartCoroutine(spawnBoss());
        }
        
        interactable = false;
    }

    IEnumerator spawnBoss()
    {
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
