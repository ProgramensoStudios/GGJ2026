using UnityEngine;

public class FollowState : IState
{
    EnemyBrain brain;

    public FollowState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter() { }

    public void Update()
    {
        if (!brain.PlayerDetected())
        {
            brain.fsm.ChangeState(new PatrolState(brain));
            return;
        }

        if (brain.InAttackRange())
        {
            brain.fsm.ChangeState(new AttackState(brain));
            return;
        }

        brain.follow?.Execute();
    }

    public void Exit() { }
}
