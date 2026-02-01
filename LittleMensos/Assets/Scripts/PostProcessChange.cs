using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessChange : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    public Volume volume;

    [Header("Vignette Settings")]
    public float vignetteOnIntensity = 0.4f;
    public float vignetteOffIntensity = 0.2f;
    public float fadeDuration = 0.5f;

    public bool firstTime = true;

    private Vignette vignette;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.active = true;
            if (firstTime) return;
            vignette.intensity.value = vignetteOnIntensity;
        }
        else
        {
            Debug.LogError("Vignette override not found in Volume");
        }
    }

    void OnEnable()
    {
        //player.OnMaskStateChanged += HandleMaskChanged;
    }

    void OnDisable()
    {
        //player.OnMaskStateChanged -= HandleMaskChanged;
    }

    void HandleMaskChanged(bool maskOn)
    {
        float target = maskOn ? vignetteOffIntensity : vignetteOnIntensity;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeVignette(target));
    }

    IEnumerator FadeVignette(float target)
    {
        float start = vignette.intensity.value;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            vignette.intensity.value = Mathf.Lerp(start, target, t);
            yield return null;
        }

        vignette.intensity.value = target;
    }
}
