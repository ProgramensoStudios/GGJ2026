using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessChange : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MaskManager maskManager;
    [SerializeField] private Volume volume;

    [Header("Vignette Settings")]
    [SerializeField] private float vignetteOnIntensity = 0.4f;
    [SerializeField] private float vignetteOffIntensity = 0.2f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Lift Gamma Gain - NONE")] //GRI
    [SerializeField] private Vector4 noneGamma = new Vector4(1.00f, 1.00f, 1.00f, -0.39f);
    [SerializeField] private Vector4 noneGain  = new Vector4(0.85f, 1.00f, 1.00f, 0.49f);

    [Header("Lift Gamma Gain - DASH")] //CAFE
    [SerializeField] private Vector4 dashGamma = new Vector4(1.00f, 0.64f, 0.58f, -0.43f);
    [SerializeField] private Vector4 dashGain  = new Vector4(0.99f, 1.00f, 1.00f, 0.76f);

    [Header("Lift Gamma Gain - CLIMB")] //AZUL
    [SerializeField] private Vector4 climbGamma = new Vector4(0f, 0f, 0.15f, -0.18f);
    [SerializeField] private Vector4 climbGain  = new Vector4(0f, 0f, 0.15f, 0.73f);

    private Vignette vignette;
    private LiftGammaGain liftGammaGain;
    private Coroutine fadeRoutine;

    void Awake()
    {
        
        if (!volume.profile.TryGet(out vignette))
            Debug.LogError("Vignette override not found in Volume");

        if (!volume.profile.TryGet(out liftGammaGain))
            Debug.LogError("LiftGammaGain override not found in Volume");

        vignette.active = true;
        liftGammaGain.active = true;
        //Debug.Log("Gamma: " + liftGammaGain.gamma.value);
       //Debug.Log("Gain: " + liftGammaGain.gain.value);
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
        float targetVignette = vignetteOnIntensity;

        switch (mask)
        {
            case MaskType.None:
                ApplyLiftGammaGain(noneGamma, noneGain);
                targetVignette = vignetteOnIntensity;
                break;

            case MaskType.Dash:
                ApplyLiftGammaGain(dashGamma, dashGain);
                targetVignette = vignetteOffIntensity;
                break;

            case MaskType.Climb:
                ApplyLiftGammaGain(climbGamma, climbGain);
                targetVignette = vignetteOffIntensity;
                break;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeVignette(targetVignette));
    }

    IEnumerator FadeVignette(float target)
    {
        float start = vignette.intensity.value;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        vignette.intensity.value = target;
    }

    void ApplyLiftGammaGain(Vector4 gamma, Vector4 gain)
    {
        liftGammaGain.gamma.Override(gamma);
        liftGammaGain.gain.Override(gain);
    }

}
