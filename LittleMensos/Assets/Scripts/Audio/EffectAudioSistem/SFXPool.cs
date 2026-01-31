using UnityEngine;

public class SFXPool : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void Play(SFXData data, Vector3 position)
    {
        transform.position = position;

        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;

        source.Play();

        if (!data.loop)
            Destroy(gameObject, data.clip.length / data.pitch);
    }

    public void Stop()
    {
        source.Stop();
    }
}
