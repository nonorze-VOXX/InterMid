using System.Linq;
using UnityEngine;

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
            _m.Prepared();
        else
            diceControllers.First().SetTargetPosition(_m.GetUsingDicePosition(), () => { OnMoveDone(); });
    }


    protected virtual void OnMoveDone()
    {
        Debug.Log(_m.CombatState + "move doen");
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
}