using UnityEngine;
using System.Collections;

public class StartFirstCutScene : MonoBehaviour
{
    [SerializeField] private GameObject sequencer;
    [SerializeField] private float secondsLoLast;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private DinamicAudio dynamicAudio;

    public void StartGame()
    {
        sequencer.SetActive(true);
        player.canMove = false;
        StartCoroutine(EndCutscene());
    }

    private IEnumerator EndCutscene()
    {
        yield return new WaitForSeconds(secondsLoLast);
        sequencer.SetActive(false);
        player.canMove = true;
        dynamicAudio.AddLayerSound(2);
        dynamicAudio.LessLayerSound(1);
        dynamicAudio.LessLayerSound(0);
    }

}
