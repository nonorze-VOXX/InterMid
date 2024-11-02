using System;
using System.Collections.Generic;

[Serializable]
public class PlayerMachine
{
    // private readonly CombatState _combatState;

    private readonly Player _player;

    // UnityAction OnPlayerPrepared;
    private List<DiceController> _dices = new();

    private IPlayerState _state;


    public PlayerMachine(Player player)
    {
        _player = player;
        // _combatState = combatState;
        // OnStart();
    }

    private IPlayerState State
    {
        get => _state;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _state?.OnExit();
            _state = value;
        }
    }


    public void OnStart()
    {
        var diceViews = _player.GetDiceView();
        for (var i = 0; i < 3; i++) _dices.Add(new DiceController(diceViews[i]));
        _state = new ThrowDiceState(this, _dices);
    }

    public void Update()
    {
        _state.Update();
    }

    public void ToMergeState()
    {
        _state = new DiceMergeState(this);
    }

    public void ToBattleState()
    {
        _state = new PlayerBattleState(this, _player.Atk);
    }
}