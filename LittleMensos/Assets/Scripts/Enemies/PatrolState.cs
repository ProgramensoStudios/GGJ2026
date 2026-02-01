using UnityEngine;

public class PatrolState : IState
{
    EnemyBrain brain;

    public PatrolState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter() { }

    public void Update()
    {
        if (brain.PlayerDetected())
        {
            brain.fsm.ChangeState(new FollowState(brain));
            return;
        }

        brain.patrol?.Execute();
    }

    public void Exit() { }
}
