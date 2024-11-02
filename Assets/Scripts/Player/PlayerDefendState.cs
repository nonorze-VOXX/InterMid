using System;

[Serializable]
public class PlayerDefendState : IPlayerState
{
    public PlayerDefendState(PlayerMachine m) : base(m)
    {
    }


    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
}