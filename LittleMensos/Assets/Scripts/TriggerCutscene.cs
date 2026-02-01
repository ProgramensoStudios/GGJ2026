using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System;

public class TriggerCutscene : MonoBehaviour
{
    public GameObject cutscene;
    public float timeToEnd;
    private bool hasPlayed = false;
    [SerializeField] private PostProcessChange mainCam;

    public Action startGame;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        cutscene.gameObject.SetActive(true);
        StartCoroutine(SetOff());
        hasPlayed = true;
        
    }
    public IEnumerator SetOff()
    {
        yield return new WaitForSeconds(timeToEnd);
        cutscene.gameObject.SetActive(false);
        startGame?.Invoke();
        mainCam.startEffect = true;
    }
}
