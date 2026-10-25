
using UnityEngine;
using System.Collections;


public static class GameUtils
{

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
