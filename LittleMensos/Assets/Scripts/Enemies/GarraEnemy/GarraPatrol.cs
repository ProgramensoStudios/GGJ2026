using UnityEngine;
using UnityEngine.Splines;

public class GarraPatrol : MonoBehaviour,IPatrolBehaviour
{
    [SerializeField] private Transform player;
    Animator anim;

    SpriteRenderer sprite;
    private void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerMovement>().gameObject.transform;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }


    public void Execute()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        UpdateFacing(dir.x);
        anim.SetBool("IsAttack", false);
    }

    void UpdateFacing(float xDir)
    {
        if (xDir > 0.05f)
            sprite.flipX = false;
        else if (xDir < -0.05f)
            sprite.flipX = true;
    }

    private void OnBecameVisible()
    {
        SFXManager.Instance.Play("GarraEnemy", transform.position);
    }
}
