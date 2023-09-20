public abstract class State<T>
{
    protected T context;
    protected StateMachine<T> stateMachine;

    public State() { }
    public State(T context, StateMachine<T> stateMachine) { SetMachineAndContext(context, stateMachine); }

    public void SetMachineAndContext(T context, StateMachine<T> stateMachine)
    {
        this.context = context;
        this.stateMachine = stateMachine;

        OnInitialize();
    }

    public virtual void OnInitialize() { }
    public virtual void OnEnter() { }

    public abstract void Update(float deltaTime);
    public virtual void FixedUpdate(float fixedDeltaTime) { }
    public virtual void OnExit() { }
}
