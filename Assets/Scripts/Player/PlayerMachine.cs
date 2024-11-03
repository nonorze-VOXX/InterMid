using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CombatState
{
    Defend,
    Attack,
    NotAssigned
}

[Serializable]
public class PlayerMachine
{
    private readonly Player _player;
    private CombatState _combatState;

    // UnityAction OnPlayerPrepared;

    private IPlayerState _state;

    private UnityAction<string> OnStateChange;


    public PlayerMachine(Player player)
    {
        _player = player;
        _combatState = CombatState.NotAssigned;
    }

    public CombatState CombatState
    {
        get => _combatState;
        set => _combatState = value;
        // Debug.Log("Combat State: " + _combatState);
    }

    public List<DiceController> Dices { get; } = new();

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
        for (var i = 0; i < 3; i++)
        {
            var diceController = new DiceController(diceViews[i]);
            Dices.Add(diceController);
            diceController.AddOnDestroyListener(_player.OnDiceDestroy);
        }

        State = new ThrowDiceState(this, Dices);
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
        if (CombatState == CombatState.Attack)
            State = new PlayerAttackState(this, _player.Atk);
        else if (CombatState == CombatState.Defend)
            State = new PlayerDefendState(this);
        else
            Debug.Log("no assign and battleing ");
    }

    public void Prepared()
    {
        _player.Prepared();
    }

    public Vector2 GetUsingDicePosition()
    {
        return _player.transform.position + new Vector3(-1, 0) *
            Vector2.Dot(_player.transform.position.normalized, Vector2.right)
            ;
    }

    public Vector2 GetUsingEnemyPosition()
    {
        return _player.enemy.GetDiceUsingPosition();
    }

    public int GetEnemyDiceValue()
    {
        return _player.enemy.GetUsingDiceValue();
    }

    public Vector2 GetFlyOutPosition()
    {
        return _player.transform.position + new Vector3(5, 10) *
            Vector2.Dot(_player.transform.position.normalized, Vector2.right)
            ;
    }

    public Vector2 GetEnemyPosition()
    {
        return _player.enemy.transform.position;
    }

    public void CauseDamageToEnemy(int atk)
    {
        _player.enemy.TakeDamage(atk);
    }
}