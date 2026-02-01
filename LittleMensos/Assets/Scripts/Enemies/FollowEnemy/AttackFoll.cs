using UnityEngine;

public class AttackFoll : MonoBehaviour, IAttackBehaviour
{

    [Header("Dash")]
    [SerializeField] float dashSpeed = 6f;
    [SerializeField] float dashDuration = 0.15f;

    [Header("Attack")]
    [SerializeField] float attackSpeed = 2f;
    [SerializeField] float attackTime = 0.5f;

    float dashTimer;
    float attackTimer;
    bool dashing;

    float dirX;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnEnter(EnemyBrain brain)
    {
        anim.SetBool("IsAttack", true);
        dirX = Mathf.Sign(brain.player.position.x - transform.position.x);

        dashing = true;
        dashTimer = dashDuration;
        attackTimer = attackTime;
    }

    public void Execute()
    {
        if (dashing)
        {
            transform.position += Vector3.right * dirX * dashSpeed * Time.deltaTime;
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                dashing = false;
            }
            return;
        }

        // ataque normal (puede ser animación, hitbox, etc)
        attackTimer -= Time.deltaTime;
    }

    public bool IsFinished()
    {
        return !dashing && attackTimer <= 0f;
    }
}
