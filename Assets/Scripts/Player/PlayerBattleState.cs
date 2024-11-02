using System;

[Serializable]
public class PlayerBattleState : IPlayerState
{
    private int Atk;

    public PlayerBattleState(PlayerMachine m, int attack) : base(m)
    {
        Atk = attack;
    }


    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
}