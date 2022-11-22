using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{


    public GameObject LoadingScreen;
    public CanvasGroup alphaController;
    public Image LoadingBarFill;
    string scenename;

    public static LoadingScene main;
    private void Start()
    {
        main = this;
    }
    public void LoadScene(string sceneName)
    {
        this.scenename = sceneName;
        StartCoroutine(ToggleLoading(true));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }


        StartCoroutine(ToggleLoading(false));
    }

    IEnumerator ToggleLoading(bool onoff)
    {

        LoadingScreen.SetActive(true);
        DontDestroyOnLoad(this.gameObject);
        int factor = onoff ? 1 : -1;
        int final = onoff ? 1 : 0;
        while(alphaController.alpha != final)
        {
            alphaController.alpha += 4 * Time.deltaTime * factor;
            yield return null;
        }


        if(alphaController.alpha == 1)
        {

            StartCoroutine(LoadSceneAsync(scenename));
        }
        else
        {

            LoadingScreen.SetActive(false);
        }


        yield return null;
    }


    public void LoadScene(string sceneName, Action callback)
    {
        this.scenename = sceneName;
        StartCoroutine(ToggleLoading(true, callback));
    }

    IEnumerator LoadSceneAsync(string sceneName, Action callback)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }
        if (callback != null)
        {
            callback();
        }

        StartCoroutine(ToggleLoading(false, null));
    }

    IEnumerator ToggleLoading(bool onoff, Action callback)
    {

        LoadingScreen.SetActive(true);
        DontDestroyOnLoad(this.gameObject);
        int factor = onoff ? 1 : -1;
        int final = onoff ? 1 : 0;
        while (alphaController.alpha != final)
        {
            alphaController.alpha += 4 * Time.deltaTime * factor;
            yield return null;
        }


        if (alphaController.alpha == 1)
        {

            StartCoroutine(LoadSceneAsync(scenename, callback));
        }
        else
        {

            LoadingScreen.SetActive(false);
        }


        yield return null;
    }

}
