using UnityEngine;

public class PatrolState : IState
{
    private EnemyBrain brain;

    public PatrolState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter() { }

    public void Update()
    {
        brain.patrol?.Execute();

        if (brain.PlayerDetected())
            brain.fsm.ChangeState(brain.followState);
    }

    public void Exit() { }
}
