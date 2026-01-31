using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BaseAudio : MonoBehaviour
{
    [SerializeField] private AudioSource bgSource;
    private Coroutine fadeCoroutine;


    private void Awake()
    {
        bgSource.loop = true;
    }

    private void Start()
    {
        bgSource.volume = 0f;
        bgSource.Play();
        FadeTo(1f, 2f); // Fade In
    }

    public void ChangeSound(AudioClip newClip)
    {
        StartCoroutine(ChangeRoutine(newClip));
    }

    public void ChangeLoop()
    {
        bgSource.loop = !bgSource.loop;
    }

    private IEnumerator ChangeRoutine(AudioClip newClip)
    {
        yield return Fade(0f, 1.5f);

        bgSource.clip = newClip;
        bgSource.Play();

        yield return Fade(1f, 1.5f);
    }

    public void FadeTo(float targetVolume, float time)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(targetVolume, time));
    }

    private IEnumerator Fade(float target, float time)
    {
        float start = bgSource.volume;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            bgSource.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }

        bgSource.volume = target;
    }
}

