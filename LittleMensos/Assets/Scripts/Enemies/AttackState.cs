using UnityEngine;

public class AttackState : IState
{
    EnemyBrain brain;

    public AttackState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        brain.attack?.OnEnter(brain);
    }

    public void Update()
    {
        brain.attack?.Execute();

        if (brain.attack != null && brain.attack.IsFinished())
        {
            brain.fsm.ChangeState(new FollowState(brain));
        }
    }

    public void Exit() { }
}
