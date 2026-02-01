using UnityEngine;

public class FollowFly : MonoBehaviour, IFollowBehaviour
{
    public Transform player;
    public float speed;

    public void Execute()
    {
        Debug.Log("Follow");
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir*(speed * Time.deltaTime);
    }
}
