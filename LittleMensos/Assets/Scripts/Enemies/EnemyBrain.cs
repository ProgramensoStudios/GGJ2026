using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public StateMachine fsm;

    [Header("Behaviours (roles)")]
    public IPatrolBehaviour patrol;
    public IFollowBehaviour follow;
    public IAttackBehaviour attack;

    [Header("Detection")]
    public Transform player;
    public float detectRange = 10f;
    public float attackRange = 2f;

    [Header("States")]
    public PatrolState patrolState;
    public FollowState followState;
    public AttackState attackState;

    void Awake()
    {
        fsm = new StateMachine();

        patrol = GetComponent<IPatrolBehaviour>();
        follow = GetComponent<IFollowBehaviour>();
        attack = GetComponent<IAttackBehaviour>();

        patrolState = new PatrolState(this);
        followState = new FollowState(this);
        attackState = new AttackState(this);
    }

    void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerMovement>().gameObject.transform;

        fsm.ChangeState(patrolState);
    }

    void Update()
    {
        fsm.Update();
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
    }

    //  Decisiones (NO comportamiento)
    public bool PlayerDetected()
    {
        return Vector3.Distance(transform.position, player.position) <= detectRange;
    }

    public bool InAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }


}
