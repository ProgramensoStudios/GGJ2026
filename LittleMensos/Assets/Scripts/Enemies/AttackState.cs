using UnityEngine;

public class AttackState : IState
{
    private EnemyBrain brain;

    public AttackState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter() { }

    public void Update()
    {
        brain.attack?.Execute();

        if (!brain.InAttackRange())
            brain.fsm.ChangeState(brain.followState);
    }

    public void Exit() { }
}
