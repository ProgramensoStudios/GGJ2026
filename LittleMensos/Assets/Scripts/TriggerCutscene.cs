using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System;

public class TriggerCutscene : MonoBehaviour
{
    public GameObject cutscene;
    public GameObject enemies;
    public Canvas marco;
    public float timeToEnd;
    private bool hasPlayed = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PostProcessChange mainCam;

    [SerializeField] private DinamicAudio dynamicAudio;

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
        dynamicAudio.AddLayerSound(3);
        dynamicAudio.LessLayerSound(0);
        dynamicAudio.LessLayerSound(1);
        dynamicAudio.LessLayerSound(2);
        

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
        marco.enabled = false;
    }
}
