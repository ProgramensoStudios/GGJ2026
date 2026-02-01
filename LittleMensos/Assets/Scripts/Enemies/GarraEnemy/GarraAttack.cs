using UnityEngine;

public class GarraAttack : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] float attackDuration = 0.6f;
    [SerializeField] float cooldown = 0.8f;

    float timer;
    bool finished;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnEnter(EnemyBrain brain)
    {
        anim.SetBool("IsAttack", true);
        anim.SetBool("IsWalking", false);

        timer = attackDuration + cooldown;
        finished = false;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
            finished = true;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public void OnExit()
    {
        anim.SetBool("IsAttack", false);
    }
}
