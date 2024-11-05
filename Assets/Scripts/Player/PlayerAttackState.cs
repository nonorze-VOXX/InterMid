using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerAttackState : PlayerBattleState
{
    // private AttackState _state = AttackState.MoveDiceToFront;
    private int Atk;
    private bool attackerPrepared;

    private bool defenderPrepared;

    public PlayerAttackState(PlayerMachine m, int attack) : base(m)
    {
        Atk = attack;
    }

    private void NextDice(DiceController diceController, List<DiceController> diceControllers)
    {
        diceController.Destroy();
        diceControllers.Remove(diceController);

        Debug.Log("attack send next dice request");
        _m.AttackerShootDone(); // shoot done
        if (diceControllers.Count == 0)
            _m.AttackerRunOutDice();
        else
            _m.ToBattleState();
    }

    protected override void OnMoveDone()
    {
        base.OnMoveDone();
        // _m.AttackerMoveDone();
        attackerPrepared = true;
        GetPrepared();
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
    }

    public void AttackerReceiveDefenderPrepared()
    {
        defenderPrepared = true;
        GetPrepared();
    }

    private void GetPrepared()
    {
        if (attackerPrepared && defenderPrepared)
        {
            // shoot
            var diceControllers = _m.Dices;

            var diceController = diceControllers.First();
            diceController.Shoot(_m.GetUsingEnemyPosition(), DiceMoveSpeed.Fast, () =>
            {
                if (diceController.GetDiceValue() <= _m.GetEnemyDiceValue())
                    diceController.Shoot(_m.GetFlyOutPosition(), DiceMoveSpeed.Fast,
                        () => { NextDice(diceController, diceControllers); });
                else
                    diceController.Shoot(_m.GetEnemyPosition(), DiceMoveSpeed.Fast, () =>
                    {
                        _m.CauseDamageToEnemy(Atk + diceController.GetDiceValue());
                        NextDice(diceController, diceControllers);
                    });
            });
        }
    }
}