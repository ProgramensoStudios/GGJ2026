using UnityEngine;
using UnityEngine.Splines;

public class FollowFly : MonoBehaviour, IFollowBehaviour
{
    public Transform player;
    public float speed;
    private Animator anim;
    private SpriteRenderer sprite;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Execute()
    {
        anim.SetBool("IsFollow", true);
        anim.SetBool("IsAttack", false);
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir*(speed * Time.deltaTime);
        UpdateFacing(dir.x);
    }

    void UpdateFacing(float xDir)
    {
        if (xDir > 0.05f)
            sprite.flipX = false;
        else if (xDir < -0.05f)
            sprite.flipX = true;
    }
}
