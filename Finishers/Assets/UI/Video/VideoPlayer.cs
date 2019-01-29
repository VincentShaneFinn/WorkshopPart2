using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayer : MonoBehaviour
{
    [SerializeField] UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField] Image imageToAppear;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
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
            StartCoroutine(Quit());
        }
    }

    IEnumerator Quit()
    {
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

        //play video clip that starts with a black screen and disable the black image after a second
        videoPlayer.Play();

        //NOTE: this should actually slowly dim the image
        yield return new WaitForSeconds(.2f);
        imageToAppear.gameObject.SetActive(false);
        yield return new WaitForSeconds(8f);

        print("Quit Game");
        Application.Quit();
    }
}
