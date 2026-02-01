using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Cinemachine;
using System;

public class TriggerCutscene : MonoBehaviour
{
    public GameObject cutscene;
    public GameObject enemies;
    public Canvas marco;

    public float timeToEnd;
    public float fadeDuration = 2f;

    private bool hasPlayed = false;

    [SerializeField] private PlayerMovement player;
    [SerializeField] private PostProcessChange mainCam;

    public Action startGame;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        marco.enabled = true;
        enemies.SetActive(true);
        player.canMove = false;
        cutscene.gameObject.SetActive(true);

        StartCoroutine(SetOff());
        hasPlayed = true;
    }

    public IEnumerator SetOff()
    {
        yield return new WaitForSeconds(timeToEnd);

        cutscene.gameObject.SetActive(false);
        player.canMove = true;
        startGame?.Invoke();
        mainCam.startEffect = true;

        yield return new WaitForSeconds(2f);

        // Fade all Marco child images together
        yield return StartCoroutine(FadeMarcoImages());

        enemies.SetActive(false);
        marco.enabled = false;
    }

    IEnumerator FadeMarcoImages()
    {
        Image[] images = marco.GetComponentsInChildren<Image>();
        float elapsed = 0f;

        // Cache starting colors
        Color[] startColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            startColors[i] = images[i].color;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            for (int i = 0; i < images.Length; i++)
            {
                Color c = startColors[i];
                c.a = Mathf.Lerp(startColors[i].a, 0f, t);
                images[i].color = c;
            }

            yield return null;
        }

        // Force alpha to 0 at the end
        for (int i = 0; i < images.Length; i++)
        {
            Color c = images[i].color;
            c.a = 0f;
            images[i].color = c;
        }
    }
}
