using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System;

public class TriggerCutscene : MonoBehaviour
{
    public GameObject cutscene;
    public GameObject enemies;
    public float timeToEnd;
    private bool hasPlayed = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PostProcessChange mainCam;

    public Action startGame;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
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
        yield return new WaitForSeconds(1f);
        enemies.SetActive(false);
    }
}
