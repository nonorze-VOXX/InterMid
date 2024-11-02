using System;

[Serializable]
public abstract class IPlayerState : IState
{
    protected PlayerMachine _m;

    protected IPlayerState(PlayerMachine m)
    {
        _m = m;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void OnExit()
    {
    }
}