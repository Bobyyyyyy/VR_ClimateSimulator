
/*
This file contains useful functions that can be useded thorough the whole project
*/

using UnityEngine;
using System.Collections;


public static class GameUtils
{

    /// <summary>
    /// Smoothly transitions the audio source volume from a starting value to a target value over a specified duration using a Coroutine.
    /// </summary>
    public static void StartFade(this MonoBehaviour host, AudioSource audioSource, float startVolume, float targetVolume, float duration)
    {
        host.StartCoroutine(FadeAudio(audioSource, startVolume, targetVolume, duration));
    }

    private static IEnumerator FadeAudio(AudioSource audioSource,float startVolume, float targetVolume, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        if (targetVolume <= 0) audioSource.Stop();
    }
}
