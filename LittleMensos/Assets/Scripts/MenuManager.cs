using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float delay;
    [SerializeField] private DinamicAudio da;

    public void ChangeScene(int Scene)
    {
        da.AddLayerSound(1);
        da.LessLayerSound(0);
        StartCoroutine(DelayToChange(Scene));
    }

    IEnumerator DelayToChange(int scene)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }
}
