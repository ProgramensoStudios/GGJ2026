using UnityEngine;

public class FollowFly : MonoBehaviour, IFollowBehaviour
{
    public Transform player;
    public float speed = 60f;

    public void Execute()
    {
        Debug.Log("Follow");
       transform.RotateAround(player.position, new Vector3(-3,-0.3f,-3), speed * Time.deltaTime);
    }
}
