using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Text sfxLabel;
    [SerializeField] Text musicLabel;
    Resolution[] res;
    public Dropdown resolutionDropdown;



    private void Start()
    {
        SetupVolume();
        SetupRes();

    }

    private void SetupRes()
    {
        res = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolusionIndex = 0;
        for (int i = 0; i < res.Length; i++)
        {
            string option = $"{res[i].width} x  {res[i].height} ({res[i].refreshRate})";
            options.Add(option);
            if(res[i].width == Screen.currentResolution.width && res[i].height == Screen.currentResolution.height)
            {
                currentResolusionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        //resolutionDropdown.value = currentResolusionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resIndex)
    {
        Screen.SetResolution(res[resIndex].width, res[resIndex].height, Screen.fullScreen);
    }


    private void SetupVolume()
    {

        musicMixer.GetFloat("Volume", out float value);
        sfxMixer.GetFloat("Volume", out float svalue);
        SetMusic(value);
        SetSFX(svalue);
    }

    public void SetMusic(float value)
    {
        musicMixer.SetFloat("Volume", value);
        musicLabel.text = "" + Mathf.RoundToInt(value + 80);
    }

    public void SetSFX(float value) {
        sfxMixer.SetFloat("Volume", value);
        sfxLabel.text = "" + Mathf.RoundToInt(value + 80);
    }


    public void SetFullscreen(int isFullscreen)
    {
        Screen.fullScreen = isFullscreen == 1;
    }
}
