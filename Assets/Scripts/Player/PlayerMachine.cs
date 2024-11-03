using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class PlayerMachine
{
    // private readonly CombatState _combatState;

    private readonly Player _player;

    // UnityAction OnPlayerPrepared;
    private List<DiceController> _dices = new();

    private IPlayerState _state;

    private UnityAction<string> OnStateChange;


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
            OnStateChange?.Invoke("Player State: " + _state.GetType().Name);
            _state.OnEnter();
        }
    }

    public void AddListener(UnityAction<string> action)
    {
        OnStateChange += action;
    }


    public void OnStart()
    {
        var diceViews = _player.GetDiceView();
        for (var i = 0; i < 3; i++) _dices.Add(new DiceController(diceViews[i]));
        State = new ThrowDiceState(this, _dices);
    }

    public void Update()
    {
        State.Update();
    }

    public void ToMergeState()
    {
        State = new DiceMergeState(this);
    }

    public void ToBattleState()
    {
        State = new PlayerBattleState(this, _player.Atk);
    }

    public void Prepared()
    {
        _player.Prepared();
    }
}