﻿using System;
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
            diceController.SetTargetPosition(_m.GetUsingEnemyPosition(), () =>
            {
                Debug.Log("Attack atk: " + diceController.GetDiceValue());
                Debug.Log("Attack target: " + _m.GetEnemyDiceValue());
                if (diceController.GetDiceValue() <= _m.GetEnemyDiceValue())
                {
                    Debug.Log("Attack failed");
                    diceController.SetTargetPosition(_m.GetFlyOutPosition(),
                        () => { NextDice(diceController, diceControllers); });
                }
                else
                {
                    diceController.SetTargetPosition(_m.GetEnemyPosition(), () =>
                    {
                        _m.CauseDamageToEnemy(Atk);
                        NextDice(diceController, diceControllers);
                    });
                }
            });
        }
    }
}