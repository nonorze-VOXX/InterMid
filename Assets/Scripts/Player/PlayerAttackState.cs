using System;

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