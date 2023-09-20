using System;
using System.Collections.Generic;

public sealed class StateMachine<T>
{
    private T context;

    private State<T> currentState;
    public State<T> CurrentState => currentState;

    private State<T> previousState;
    public State<T> PreviousState => previousState;

    private float elapsedTimeInState = 0f;
    public float ElapsedTimeInState => elapsedTimeInState;

    private Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();

    public StateMachine(T context, State<T> startState)
    {
        this.context = context;

        AddState(startState);
        currentState = startState;
        currentState.OnEnter();
        elapsedTimeInState = 0f;
    }

    public void AddState(State<T> state)
    {
        state.SetMachineAndContext(context, this);
        states.Add(state.GetType(), state);
    }

    public void Update(float deltaTime)
    {
        currentState.Update(deltaTime);

        elapsedTimeInState += deltaTime;
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        currentState.FixedUpdate(fixedDeltaTime);
    }

    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);
        if (currentState.GetType() == newType)
        {
            return currentState as R;
        }

        if (currentState != null)
        {
            currentState.OnExit();
        }

        previousState = currentState;

        currentState = states[newType];
        currentState.OnEnter();
        elapsedTimeInState = 0.0f;
        
        return currentState as R;
    }
}
