using System.Linq;
using UnityEngine;

public class PlayerDefendState : PlayerBattleState
{
    public PlayerDefendState(PlayerMachine m) : base(m)
    {
    }


    protected override void OnMoveDone()
    {
        base.OnMoveDone();
        _m.DefenderMoveDone();
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
    }

    public void AttackerShootDone()
    {
        var diceControllers = _m.Dices;
        var diceController = diceControllers.First();

        Debug.Log("defend dice be destroyed");
        diceController.Destroy();
        diceControllers.Remove(diceController);
        if (diceControllers.Count == 0)
            _m.Prepared();
        // state = DefendState.NoDice;
        else
            _m.ToBattleState();
    }
}