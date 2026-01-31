using UnityEngine;
using System.Collections;

public class FadeObstacles : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 0.25f;
    [SerializeField] float fadedAlpha = 0.3f;

    private Material _material;
    private Coroutine _fadeRoutine;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    public void FadeOut()
    {
        StartFade(fadedAlpha);
    }

    public void FadeIn()
    {
        StartFade(1f);
    }

    void StartFade(float targetAlpha)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeCoroutine(targetAlpha));
    }

    IEnumerator FadeCoroutine(float targetAlpha)
    {
        Color color = _material.GetColor(BaseColorID);
        float startAlpha = color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            _material.SetColor(BaseColorID, color);
            yield return null;
        }

        color.a = targetAlpha;
        _material.SetColor(BaseColorID, color);
        _fadeRoutine = null;
    }
}
