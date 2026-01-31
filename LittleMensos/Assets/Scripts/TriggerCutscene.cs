using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class TriggerCutscene : MonoBehaviour
{
    public GameObject cutscene;
    public float timeToEnd;
    private bool hasPlayed = false;

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
    }
}
