using UnityEngine;

public class PatrolFly : MonoBehaviour, IPatrolBehaviour
{
    public void Execute()
    {
        Debug.Log("patruyar");
        transform.position += transform.up * Mathf.Sin(Time.time) * 10f * Time.deltaTime;
    }
}

