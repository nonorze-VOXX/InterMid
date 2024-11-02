public class DiceMergeState : IPlayerState
{
    public DiceMergeState(PlayerMachine m) : base(m)
    {
    }

    public override void Update()
    {
        _m.ToBattleState();
    }

    public override void OnExit()
    {
    }
}