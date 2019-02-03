using Finisher.Characters;
using System.Collections;
using UnityEngine;

public class ChalliceInteraction : MonoBehaviour
{
    public AnimationClip animationToPlay;

    protected bool interactable = false;
    public GameObject effect;

    public GameObject bossStatue;
    public GameObject bossFireEffect;
    public GameObject bossEnemy;

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
            StartCoroutine(lightTorchSequence());
            interactable = false;
        }
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
}
