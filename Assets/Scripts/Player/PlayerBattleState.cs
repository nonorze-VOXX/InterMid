using System.Linq;

public class PlayerBattleState : IPlayerState
{
    public PlayerBattleState(PlayerMachine m) : base(m)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        var diceControllers = _m.Dices;
        if (diceControllers.Count == 0)
        {
            _m.Prepared();
        }
        else
        {
            diceControllers.First().SetTargetPosition(_m.GetUsingDicePosition());
            diceControllers.First().SetMoveable(true);
            diceControllers.First().AddOnMoveDoneListener(() => { OnMoveDone(); });
        }
    }

    protected virtual void OnMoveDone()
    {
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
}