using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string musicParam = "MusicVolume";
    public string sfxParam = "SFXVolume";

    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text musicVolumeText;
    public TMP_Text sfxVolumeText;  

    private const string KeyMusic = "MusicVolume";
    private const string KeySFX = "SFXVolume";

    private const float MinLinear = 0.0001f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        float music = PlayerPrefs.GetFloat(KeyMusic, 0.8f);
        float sfx = PlayerPrefs.GetFloat(KeySFX, 0.8f);

        if (musicSlider) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfx);

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, MinLinear)) * 20f;
        audioMixer.SetFloat(musicParam, dB);
        PlayerPrefs.SetFloat(KeyMusic, volume);
        PlayerPrefs.Save();

        if(musicVolumeText)
            musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, MinLinear)) * 20f;
        audioMixer.SetFloat(sfxParam, dB);
        PlayerPrefs.SetFloat(KeySFX, volume);
        PlayerPrefs.Save();

        if (sfxVolumeText)
            sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
}
