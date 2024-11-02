using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private Player[] _players = new Player[2];
    private Player attacker;
    private Player defender;

    private UnityAction OnBeforeBattle;

    // Start is called before the first frame update
    private void Start()
    {
        // _gameFlow = new GameFlow.Welcome();
        _gameState = GameState.Welcome;
        _players = new Player[2];
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_gameState)
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
                    OnBeforeBattle += attacker.ReceiveBeforeBattleSignal;
                    defender = !p1First ? _players[0] : _players[1];
                    OnBeforeBattle += defender.ReceiveBeforeBattleSignal;
                    attacker.transform.position = new Vector3(-5, 0, 0);
                    defender.transform.position = new Vector3(5, 0, 0);
                    playerPrepared.Clear();
                    _gameState = GameState.BeforeBattle;
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
                break;
            case GameState.BattleAnimation:
                _gameState = GameState.Battle;
                break;
            case GameState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Judege(int p1, int p2)
    {
        if (p1 > p2)
            Debug.Log("Player 1 win");
        else if (p1 < p2)
            Debug.Log("Player 2 win");
        else
            Debug.Log("Draw");
        _gameState = GameState.BattleAnimation;
    }


    private void AddPlayerPrepared(Player player)
    {
        playerPrepared.Add(player);
        if (playerPrepared.Count == 2)
            // start battle
            _gameState = GameState.Battle;
    }
}