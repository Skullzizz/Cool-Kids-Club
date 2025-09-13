using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeBootstrap : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SFXVolume";

    const string KeyMusic = "MusicVolume";
    const string KeySFX = "SFXVolume";
    const float MinLinear = 0.0001f;

    private void Awake()
    {
        float music = PlayerPrefs.GetFloat(KeyMusic, 0.8f);
        float sfx = PlayerPrefs.GetFloat(KeySFX, 0.8f);

        mixer.SetFloat(musicParam, LinearToDb(music));
        mixer.SetFloat(sfxParam, LinearToDb(sfx));
    }

    static float LinearToDb(float x)
    {
        return Mathf.Log10(Mathf.Max(x, MinLinear)) * 20;
    }
}
