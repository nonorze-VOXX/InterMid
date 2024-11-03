using System;
using System.Linq;
using UnityEngine;

// internal enum BattleState
// {
//     MoveDiceToFront,
//     Shoot
// }

[Serializable]
public class PlayerAttackState : PlayerBattleState
{
    private int Atk;
    // private BattleState state = 0;

    public PlayerAttackState(PlayerMachine m, int attack) : base(m)
    {
        Atk = attack;
    }

    protected override void OnMoveDone()
    {
        base.OnMoveDone();
        Debug.Log("overwrited on move done work");
        var diceControllers = _m.Dices;
        var diceController = diceControllers.First();
        diceController.SetTargetPosition(_m.GetUsingEnemyPosition());
        diceController.SetMoveable(true);
        diceController.AddOnMoveDoneListener(() =>
        {
            Debug.Log("Attack atk: " + diceController.GetDiceValue());
            Debug.Log("Attack target: " + _m.GetEnemyDiceValue());
            if (diceController.GetDiceValue() < _m.GetEnemyDiceValue())
            {
                Debug.Log("Attack failed");
                diceController.SetTargetPosition(_m.GetFlyOutPosition());
                diceController.SetMoveable(true);
                diceController.AddOnMoveDoneListener(() => { });
            }
            else
            {
                diceController.SetTargetPosition(_m.GetEnemyPosition());
                diceController.SetMoveable(true);
                diceController.AddOnMoveDoneListener(() =>
                {
                    _m.CauseDamageToEnemy(Atk);
                    diceController.Destroy();
                    diceControllers.Remove(diceController);

                    _m.Prepared();
                });
            }
        });
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
}