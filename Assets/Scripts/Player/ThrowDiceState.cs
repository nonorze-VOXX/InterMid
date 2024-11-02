using System.Collections.Generic;

internal class ThrowDiceState : IPlayerState
{
    private readonly List<DiceController> _diceControlers;
    private bool _isRolling;
    private int _throwCount;

    public ThrowDiceState(PlayerMachine m, List<DiceController> diceControlers) : base(m)
    {
        _diceControlers = diceControlers;
        _throwCount = 0;
    }

    public override void Update()
    {
        // if (true) _m.ToBattleState();
        if (_throwCount >= _diceControlers.Count)
        {
            _m.ToMergeState();
        }
        else
        {
            if (!_isRolling)
            {
                _diceControlers[_throwCount].SetViewActive(true);
                _isRolling = true;
                _diceControlers[_throwCount].Roll(OnRollEnd);
            }
        }
    }

    private void OnRollEnd()
    {
        _throwCount++;
        _isRolling = false;
    }

    public override void OnExit()
    {
    }
}