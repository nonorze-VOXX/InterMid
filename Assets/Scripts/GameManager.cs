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
    BeforeBattle,
    Battle,
    End,
    BattleAnimation
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    // IGameFlow _gameFlow;

    private readonly HashSet<Player> playerPrepared = new();
    private GameState _gameState;
    private TMP_Text _gameStateText;

    private Player[] _players = new Player[2];
    private Player attacker;
    private Player defender;

    private UnityAction OnBeforeBattle;

    private UnityAction<bool> OnDebugMode;

    private GameState gameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            _gameStateText.text = "Game State: " + _gameState;
            Debug.Log("Game State: " + _gameState);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // _gameFlow = new GameFlow.Welcome();
        _gameStateText = GetComponentInChildren<TMP_Text>();
        gameState = GameState.Welcome;
        _players = new Player[2];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) DebugMode();

        switch (gameState)
        {
            case GameState.Welcome:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Random.InitState(42);
                    var random_value = Random.Range(0, 6);
                    var p1First = random_value > 3;
                    _players = new[]
                    {
                        Instantiate(playerPrefab), Instantiate(playerPrefab)
                    };
                    _players[0].AddListener(AddPlayerPrepared);
                    _players[1].AddListener(AddPlayerPrepared);
                    attacker = p1First ? _players[0] : _players[1];
                    OnDebugMode += attacker.SetDebug;
                    attacker.SetDebug(_gameStateText.enabled);
                    attacker.SetAttacker();
                    OnBeforeBattle += attacker.ReceiveBeforeBattleSignal;
                    defender = !p1First ? _players[0] : _players[1];
                    defender.SetDefender();
                    defender.SetDebug(_gameStateText.enabled);
                    OnDebugMode += defender.SetDebug;
                    OnBeforeBattle += defender.ReceiveBeforeBattleSignal;
                    attacker.transform.position = new Vector3(-5, -2, 0);
                    defender.transform.position = new Vector3(5, -2, 0);
                    var pos = defender.GetComponentInChildren<Slider>().transform.localPosition;
                    pos.x *= -1;
                    defender.GetComponentInChildren<Slider>().transform.localPosition = pos;
                    defender.GetComponentInChildren<Image>().transform.localRotation = Quaternion.Euler(0, 180, 0);
                    defender.enemy = attacker;
                    attacker.enemy = defender;
                    playerPrepared.Clear();
                    gameState = GameState.BeforeBattle;
                    OnBeforeBattle?.Invoke();
                }

                break;
            case GameState.BeforeBattle:
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

    private void DebugMode()
    {
        // var componentInChildren = GetComponentInChildren<Canvas>();
        var next = !_gameStateText.enabled;
        _gameStateText.enabled = next;
        // componentInChildren.enabled = next;
        OnDebugMode?.Invoke(next);
    }

    private void Judege(int p1, int p2)
    {
        if (p1 > p2)
            Debug.Log("Player 1 win");
        else if (p1 < p2)
            Debug.Log("Player 2 win");
        else
            Debug.Log("Draw");
        gameState = GameState.BattleAnimation;
    }


    private void AddPlayerPrepared(Player player)
    {
        playerPrepared.Add(player);
        if (playerPrepared.Count == 2)
        {
            if (gameState == GameState.BeforeBattle)
                gameState = GameState.Battle;
            else if (gameState == GameState.Battle) gameState = GameState.End;

            playerPrepared.Clear();
        }
    }
}