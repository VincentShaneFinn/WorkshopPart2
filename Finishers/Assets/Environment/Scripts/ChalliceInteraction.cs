using Finisher.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChalliceInteraction : InteractionSaveable
{
    public AnimationClip animationToPlay;

    public GameObject[] doors; //TODO: replace with a list of delegates
    public GameObject effect;
    public int challicesNeeded;
    public GameObject bossStatue;
    public GameObject bossFireEffect;
    public GameObject bossEnemy;
    public GameObject InteractionButton;
    protected bool interactable = false;
    private static int challicesLit;
    //public GameObject vialUI; // use PLayerUI>BottumLeft>VialIcon
    //public Sprite emptyVial;  // use Assests>UI>UIButtons>Textures>VialEmpty

    public override void runInteraction()
    {
        lightTorch();
    }

    // Start is called before the first frame update
    private void Start()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TagNames.PlayerTag && interactable)
        {
            other.GetComponent<CharacterState>().EnterInvulnerableActionState(animationToPlay);
            GetComponent<InteractionSaveable>().interacted = true;
            other.GetComponent<CharacterState>().spawnConfig = new SpawnConfig();
            lightTorch();
            callDelegates();
        }
    }

    private void callDelegates()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
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
        //vialUI.GetComponent<Image>().sprite = emptyVial;
    }

    private IEnumerator spawnBoss()
    {
        yield return new WaitForSeconds(1f);

        bossStatue.SetActive(false);

        yield return null;

        bossFireEffect.SetActive(true);
        bossEnemy.SetActive(true);

        yield return new WaitForSeconds(1f);

        bossFireEffect.SetActive(false);
    }
}