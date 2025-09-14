using UnityEngine;
using UnityEngine.Rendering;

public class PauseMenuMusic : MonoBehaviour
{
    private AudioSource musicSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        if (musicSource != null)
        {
            musicSource.ignoreListenerPause = true;
        }
    }

    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource !=null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
