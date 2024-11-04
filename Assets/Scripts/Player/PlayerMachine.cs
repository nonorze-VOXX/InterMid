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


    private IPlayerState _state;

    private UnityAction<string> OnStateChange;
    private PlayerAttackState playerAttackState;
    private PlayerDefendState playerDefendState;


    public PlayerMachine(Player player)
    {
        _player = player;
        _combatState = CombatState.NotAssigned;
    }

    public CombatState CombatState
    {
        get => _combatState;
        set => _combatState = value;
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

    public void InvokeStateChangeWithBattleState(string battleState)
    {
        OnStateChange?.Invoke("Player State: " + _state.GetType().Name + " " + battleState);
    }

    public void AddListener(UnityAction<string> action)
    {
        OnStateChange += action;
    }


    public void OnStart()
    {
        ToThrowState();
    }

    public void ToThrowState()
    {
        var diceViews = _player.GetNewDiceView();
        for (var i = 0; i < 3; i++)
        {
            var diceView = diceViews[i];
            var image = diceView.GetComponentInChildren<SpriteRenderer>();

            var colorHex = _combatState == CombatState.Attack ? "#933E3E" : "#578DD4";

            if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
                image.color = newColor;
            else
                Debug.LogError("Invalid color code.");

            var diceController = new DiceController(diceView);
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
        {
            playerAttackState = new PlayerAttackState(this, _player.Atk);
            State = playerAttackState;
        }
        else if (CombatState == CombatState.Defend)
        {
            playerDefendState = new PlayerDefendState(this);
            State = playerDefendState;
        }
        else
        {
            Debug.Log("no assign and battleing ");
        }
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
        return _player.transform.position + new Vector3(5, 0) *
                                          Vector2.Dot(_player.transform.position.normalized, Vector2.right)
                                          + new Vector3(0, 10)
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

    public void DefenderMoveDone()
    {
        _player.enemy.DefenderPreparedAcceptAttack();
    }

    public void AttackerReceiveDefenderPrepared()
    {
        playerAttackState.AttackerReceiveDefenderPrepared();
    }

    public void AttackerShootDone()
    {
        _player.enemy.DefenderGetAttackerShootDone();
    }

    public void DefenderGetAttackerShootDone()
    {
        playerDefendState.AttackerShootDone();
    }

    public void AttackerRunOutDice()
    {
        _player.enemy.NextTurn();
        NextTurn();
    }

    public void NextTurn()
    {
        CombatState = CombatState == CombatState.Attack ? CombatState.Defend : CombatState.Attack;
        ToThrowState();
    }
}