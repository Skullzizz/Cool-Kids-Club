using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    private static AudioSource uiAudioSource;

    void Awake()
    {
        if (uiAudioSource == null)
            uiAudioSource = GameObject.Find("UI_Audio").GetComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (clickSound != null && uiAudioSource != null)
            uiAudioSource.PlayOneShot(clickSound);
    }
}
