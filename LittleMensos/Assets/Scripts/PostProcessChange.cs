using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessChange : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MaskManager maskManager;
    public Volume volume;

    [Header("Vignette Settings")]
    public float vignetteOnIntensity = 0.4f;
    public float vignetteOffIntensity = 0.2f;
    public float fadeDuration = 0.5f;

    [Header("LiftGammaGain Settings")]
    public Color noneMaskLift = Color.white;
    public Color dashMaskLift = Color.red;
    public Color climbMaskLift = Color.blue;

    [Range(-1f, 1f)]
    public float liftIntensity = 0.2f;

    public bool firstTime = true;

    private Vignette vignette;
    private LiftGammaGain liftGammaGain;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.active = true;

            if (!firstTime)
                vignette.intensity.value = vignetteOnIntensity;
        }
        else
        {
            Debug.LogError("Vignette override not found in Volume");
        }

        if (volume.profile.TryGet(out liftGammaGain))
        {
            liftGammaGain.active = false;
        }
        else
        {
            Debug.LogError("LiftGammaGain override not found in Volume");
        }
    }

    void OnEnable()
    {
        if (maskManager != null)
            maskManager.OnMaskChanged += HandleMaskChanged;
    }

    void OnDisable()
    {
        if (maskManager != null)
            maskManager.OnMaskChanged -= HandleMaskChanged;
    }

    void HandleMaskChanged(MaskType mask)
    {
        float targetVignette;
        Color targetLiftColor;

        switch (mask)
        {
            case MaskType.None:
                targetVignette = vignetteOnIntensity;
                targetLiftColor = noneMaskLift;
                break;

            case MaskType.Dash:
                targetVignette = vignetteOffIntensity;
                targetLiftColor = dashMaskLift;
                break;

            case MaskType.Climb:
                targetVignette = vignetteOffIntensity;
                targetLiftColor = climbMaskLift;
                break;

            default:
                targetVignette = vignetteOnIntensity;
                targetLiftColor = noneMaskLift;
                break;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeVignette(targetVignette));

        ApplyLift(targetLiftColor);
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

    void ApplyLift(Color color)
    {
        Vector4 liftValue = new Vector4(
            color.r * liftIntensity,
            color.g * liftIntensity,
            color.b * liftIntensity,
            0f
        );

        liftGammaGain.lift.Override(liftValue);
    }
}
