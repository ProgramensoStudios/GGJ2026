public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public interface IPatrolBehaviour
{
    void Execute();
}

public interface IFollowBehaviour
{
    void Execute();
}

public interface IAttackBehaviour
{
    void Execute();
}
public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        if (newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}