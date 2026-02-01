using UnityEngine;

public class FollowState : IState
{
    private EnemyBrain brain;

    public FollowState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter() { }

    public void Update()
    {
        brain.follow?.Execute();

        if (brain.InAttackRange())
            brain.fsm.ChangeState(brain.attackState);
    }

    public void Exit() { }
}
