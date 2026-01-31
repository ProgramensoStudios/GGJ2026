using System.Collections;
using UnityEngine;

public class DinamicAudio : MonoBehaviour
{

    public static DinamicAudio Instance { get; private set; }
    [SerializeField] private AudioSource[] audioLayers;
    private float timeChange = 0.75f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


        void Start()
    {
        for (int index = 0; index < audioLayers.Length; index++)
        {
            AudioSource layer = audioLayers[index];
            layer.volume = 0f;
            layer.Play();
        }
        AddLayerSound(0);
    }

    public void AddLayerSound(int targetAudio)
    {
        StartCoroutine(Fade(targetAudio, 1, timeChange));
    }
    public void LessLayerSound(int targetAudio)
    {
        StartCoroutine(Fade(targetAudio, 0, timeChange));
    }

    private IEnumerator Fade(int layerAudio, float target, float time)
    {
        float start = audioLayers[layerAudio].volume;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            audioLayers[layerAudio].volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }

        audioLayers[layerAudio].volume = target;
    }
}
