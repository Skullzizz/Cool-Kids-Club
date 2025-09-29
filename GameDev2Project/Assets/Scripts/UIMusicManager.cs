using System.Collections;
using UnityEngine;

public class UIMusicManager : MonoBehaviour
{
    AudioSource source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(FadeIn(source, 2f));

    }

    private void Update()
    {
        if (source.isPlaying == false)
        {
            StartCoroutine(FadeIn(source, 2f));
        }
    }

    IEnumerator FadeOut(AudioSource source, float fadeTime)
    {
        float startVolume = source.volume;
        while (source.volume > 0f)
        {
            source.volume -= startVolume * Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        source.Stop();
    }

    IEnumerator FadeIn(AudioSource source, float fadeTime)
    {
        source.Play();
        source.volume = 0f;
        while (source.volume < 1f)
        {
            source.volume += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    IEnumerator ChangeFadeIn(AudioSource source, float fadeTime, AudioClip nextSong)
    {
        yield return StartCoroutine(FadeOut(source, fadeTime));
        source.clip = nextSong;
        source.Play();
        source.volume = 0f;
        while (source.volume < 1f)
        {
            source.volume += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }
    IEnumerator ChangeFadeMenu(AudioSource source, float fadeTime, AudioClip nextSong)
    {
        source.clip = nextSong;
        source.Play();
        source.volume = 0f;
        while (source.volume < 1f)
        {
            source.volume += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public void FadeChange(ref AudioSource source, AudioClip nextSource)
    {
        StartCoroutine(ChangeFadeIn(source, 2f, nextSource));
    }

    public void MenuFadeChange(ref AudioSource source, AudioClip nextSource)
    {
        StartCoroutine(ChangeFadeMenu(source, 2f, nextSource));
    }


}
