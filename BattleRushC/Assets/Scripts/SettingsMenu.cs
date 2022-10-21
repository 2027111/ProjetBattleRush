using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Text sfxLabel;
    [SerializeField] Text musicLabel;

    private void Start()
    {
        musicMixer.GetFloat("Volume", out float value);
        sfxMixer.GetFloat("Volume", out float svalue);
        musicLabel.text = "" + Mathf.RoundToInt(value + 20);
        sfxLabel.text = "" + Mathf.RoundToInt(svalue + 20);
    }

    public void SetMusic(float value)
    {
        musicMixer.SetFloat("Volume", value);
        musicLabel.text = "" + Mathf.RoundToInt(value + 20);
    }

    public void SetSFX(float value) {
        sfxMixer.SetFloat("Volume", value);
        sfxLabel.text = "" + Mathf.RoundToInt(value + 20);
    }
}
