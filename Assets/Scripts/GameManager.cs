using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

internal enum GameState
{
    Welcome,
    Prepare,
    Battle,
    End,
    BattleAnimation
}

internal enum TextIndex
{
    GameState,
    Turn
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;

    [SerializeField] private TMP_Text welcomeText;

    private readonly HashSet<Player> playerPrepared = new();
    private GameState _gameState;
    private TMP_Text _gameStateText;

    private Player[] _players = new Player[2];

    private int _turn;

    private TMP_Text _turnText;

    private Player diePlayer;

    // private Player attacker;
    // private Player defender;

    private UnityAction OnBeforeBattle;

    private UnityAction<bool> OnDebugMode;

    private int turn
    {
        get => _turn;
        set
        {
            _turn = value;
            _turnText.text = "Turn: " + turn;
        }
    }

    private GameState gameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            _gameStateText.text = "Game State: " + _gameState;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // _gameFlow = new GameFlow.Welcome();
        _gameStateText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.GameState];
        _gameStateText.enabled = false;
        _turnText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.Turn];
        gameState = GameState.Welcome;
        _turnText.enabled = false;
        _players = new Player[2];
        welcomeText.text = "Press R to start";
        welcomeText.enabled = true;
        Random.InitState(42);

        #region initPlayer

        _players = new[]
        {
            Instantiate(playerPrefab), Instantiate(playerPrefab)
        };
        Player attacker;
        Player defender;
        _players[0].AddListener(AddPlayerPrepared);
        _players[1].AddListener(AddPlayerPrepared);
        var random_value = Random.Range(0, 6);
        var p1First = random_value > 3;
        attacker = p1First ? _players[0] : _players[1];

        OnDebugMode += attacker.SetDebug;
        attacker.SetDebug(_gameStateText.enabled);
        attacker.AddTurnAddOneListener(BattleEnd);
        attacker.SetAttacker();
        OnBeforeBattle += attacker.ReceiveBeforeBattleSignal;
        defender = !p1First ? _players[0] : _players[1];
        defender.SetDefender();
        defender.AddTurnAddOneListener(BattleEnd);
        defender.SetDebug(_gameStateText.enabled);
        OnDebugMode += defender.SetDebug;
        OnBeforeBattle += defender.ReceiveBeforeBattleSignal;
        attacker.transform.position = new Vector3(-5, -2, 0);
        attacker.AddPlayerDieListener(OnPlayerDie);
        defender.AddPlayerDieListener(OnPlayerDie);
        defender.transform.position = new Vector3(5, -2, 0);
        var pos = defender.GetComponentInChildren<Slider>().transform.localPosition;
        pos.x *= -1;
        defender.GetComponentInChildren<Slider>().transform.localPosition = pos;
        defender.GetComponentInChildren<Image>().transform.localRotation = Quaternion.Euler(0, 180, 0);
        defender.enemy = attacker;
        attacker.enemy = defender;
        attacker.transform.name = "Player1";
        defender.transform.name = "Player2";
        foreach (var player in _players) player.gameObject.SetActive(false);

        #endregion
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) DebugMode();

        switch (gameState)
        {
            case GameState.Welcome:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    welcomeText.enabled = false;
                    playerPrepared.Clear();
                    gameState = GameState.Prepare;
                    OnBeforeBattle?.Invoke();
                    _turnText.enabled = true;
                    turn = 1;
                    diePlayer = null;
                    foreach (var player in _players) player.gameObject.SetActive(true);
                }

                break;
            case GameState.Prepare:
                break;
            case GameState.Battle:
                // Debug.Log("Battleing");

                // var p1 = _players[0].GetDice();
                // var p2 = _players[1].GetDice();
                //
                // if (p1 == null && p2 == null)
                //     _gameState = GameState.End;
                // else if (p1 == null && p2 != null)
                //     Debug.Log("Draw");
                // else if (p1 != null && p2 == null)
                //     Debug.Log("Draw");
                // else
                //     Judege(p1.Value, p2.Value);
                // attacker.GetDiceView();

                break;
            case GameState.BattleAnimation:
                gameState = GameState.Battle;
                break;
            case GameState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void BattleEnd()
    {
        print("battle end");
        if (diePlayer != null) return;

        NextRound();

        turn++;
    }

    private void OnPlayerDie(Player player)
    {
        diePlayer = player;
        Debug.Log("Player Die");
        player.enemy.score++;
        if (player.enemy.score >= 2)
        {
            gameState = GameState.End;
            welcomeText.text = "Player " + player.enemy.name + " Win!\n Press R to restart";
            welcomeText.enabled = true;
            print("Player " + player.enemy.name + " Win!");
        }
        else
        {
            NextRound();
            turn = 1;
        }
    }

    private void NextRound()
    {
        gameState = GameState.Prepare;
        foreach (var player in _players)
        {
            var state = player.GetCombatState();
            state = state == COMBAT_STATE.Attack ? COMBAT_STATE.Defend : COMBAT_STATE.Attack;
            player.NextTurn(state);
        }
    }


    private void DebugMode()
    {
        // var componentInChildren = GetComponentInChildren<Canvas>();
        var next = !_gameStateText.enabled;
        _gameStateText.enabled = next;
        // componentInChildren.enabled = next;
        OnDebugMode?.Invoke(next);
    }

    private void AddPlayerPrepared(Player player)
    {
        playerPrepared.Add(player);
        if (playerPrepared.Count == 2)
        {
            if (gameState == GameState.Prepare)
                gameState = GameState.Battle;
            else if (gameState == GameState.Battle) gameState = GameState.End;

            playerPrepared.Clear();
        }
    }
}