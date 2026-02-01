using System.Collections;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public float delay;
    [SerializeField] private DinamicAudio da;
    [SerializeField] private Canvas menu;
    [SerializeField] private StartFirstCutScene cutsceneStart;

    public void ChangeScene(int Scene)
    {
        da.AddLayerSound(1);
        da.LessLayerSound(0);
        SFXManager.Instance.Play("Inicio", Vector3.zero);
        StartCoroutine(DelayToChange());
    }

    IEnumerator DelayToChange()
    {
        cutsceneStart.StartGame();
        yield return new WaitForSeconds(delay);
        menu.enabled = false;
    }
}
