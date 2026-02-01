using UnityEngine;
using System.Collections;

public class StartFirstCutScene : MonoBehaviour
{
    [SerializeField] private GameObject sequencer;
    [SerializeField] private float secondsLoLast;
    [SerializeField] private PlayerMovement player;

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
    }

}
