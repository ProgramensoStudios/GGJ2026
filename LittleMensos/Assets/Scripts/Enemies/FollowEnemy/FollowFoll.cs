using UnityEngine;

public class FollowFoll : MonoBehaviour, IFollowBehaviour
{
    public Transform player;
    public float speed;

    Animator anim;
    SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Execute()
    {
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsWalking", true);

        float dirX = player.position.x - transform.position.x;

        if (dirX != 0)
            sr.flipX = dirX < 0;

        float moveX = Mathf.Sign(dirX) *     speed * Time.deltaTime;
        transform.position += new Vector3(moveX, 0f, 0f);
    }
}
