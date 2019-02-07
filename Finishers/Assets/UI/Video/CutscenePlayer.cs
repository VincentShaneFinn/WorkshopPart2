using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    private RawImage rawImage;
    private UnityEngine.Video.VideoPlayer videoPlayer;
    private AudioSource audioSource;
    [SerializeField] Image imageToAppear;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        waitForInput();
    }

    private void waitForInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayCutscene();
        }
    }

    public void PlayCutscene()
    {
        StartCoroutine(PlayVideo());
        audioSource.Play();
    }

    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();

        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }

        float alphaCount = 0;
        float totalTime = 2;

        //Start at 0 alpha
        imageToAppear.gameObject.SetActive(true);

        //undim slowly, to a fully black screen
        while (alphaCount < totalTime)
        {
            imageToAppear.color = new Color(0, 0, 0, alphaCount / totalTime);
            alphaCount += Time.deltaTime;
            yield return null;
        }

        rawImage.texture = videoPlayer.texture;
        rawImage.enabled = true;
        videoPlayer.Play();

        yield return new WaitForSeconds(.2f);
        imageToAppear.gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        print("Quit Game");
        Application.Quit();
    }
}
