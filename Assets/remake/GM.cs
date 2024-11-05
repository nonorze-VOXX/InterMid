using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace remake
{
    internal enum flow
    {
        Welcome,
        Prepare,
        BeforeBattle,
        Battle,
        End
    }

    public class GM : MonoBehaviour
    {
        private static readonly int PLAYER_COUNT = 2;

        private static readonly int max_turn = 5; // todo: change to 10
        // todo: speed running for deadline 

        [SerializeField] private rPlayer playerPrefab;

        [SerializeField] private TMP_Text welcomeText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text flowText;

        private flow _state;

        private int _turn;
        private rPlayer[] players;

        private bool prepareOk;
        private int shootCount;

        private int turn
        {
            get => _turn;
            set
            {
                _turn = value;
                turnText.text = "Turn: " + turn;
            }
        }

        private flow state
        {
            get => _state;
            set
            {
                flowText.text = value.ToString();
                // like on exit
                switch (_state)
                {
                    case flow.Welcome:
                        welcomeText.gameObject.SetActive(false);
                        turnText.enabled = true;
                        break;
                    case flow.Prepare:
                        break;
                    case flow.BeforeBattle:
                        break;
                    case flow.Battle:
                        break;
                    case flow.End:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // like on enter
                switch (value)
                {
                    case flow.Welcome:
                        // make text show
                        // make text show "press any key to start"
                        welcomeText.gameObject.SetActive(true);
                        welcomeText.text = "press r key to start";
                        if (turnText) turnText.enabled = false;
                        turn = 0;
                        break;
                    case flow.Prepare:
                        turn++;
                        if (turn > max_turn)
                        {
                            // do nothing, wait update do 
                        }
                        else
                        {
                            players[0].gameObject.SetActive(true);
                            players[1].gameObject.SetActive(true);
                            players[0].ResetThrowCount();
                            players[1].ResetThrowCount();
                            players[0].Prepare(OnPrepareDone);
                            players[1].Prepare(OnPrepareDone);
                            prepareOk = false;
                        }

                        break;
                    case flow.BeforeBattle:
                        break;
                    case flow.Battle:
                        players[0].Turn(OnOneShootEnd);
                        players[1].Turn(OnOneShootEnd);
                        break;
                    case flow.End:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _state = value;
            }
        }

        private void Awake()
        {
            #region get_text_object

            flowText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.GameState];
            flowText.enabled = false;
            turnText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.Turn];
            turnText.enabled = false;

            #endregion

            players = new rPlayer[PLAYER_COUNT];
            for (var i = 0; i < PLAYER_COUNT; i++) players[i] = Instantiate(playerPrefab);

            #region init_player

            players[0].SetAttack(true);
            players[1].SetAttack(false);

            players[0].SetOpponent(players[1]);
            players[1].SetOpponent(players[0]);

            players[0].transform.position = new Vector3(-5, -2, 0);
            players[1].transform.position = new Vector3(5, -2, 0);


            players[0].transform.name = "Player1";
            players[1].transform.name = "Player2";

            players[0].gameObject.SetActive(false);
            players[1].gameObject.SetActive(false);

            players[1].GetComponentInChildren<Image>().transform.localRotation = Quaternion.Euler(0, 180, 0);
            var pos = players[1].GetComponentInChildren<Slider>().transform.localPosition;
            pos.x *= -1;
            players[1].GetComponentInChildren<Slider>().transform.localPosition = pos;

            #endregion

            state = flow.Welcome;
            ChangeDebugMode(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                // todo: debug mode

            {
                var nextDebugMode = !flowText.enabled;
                ChangeDebugMode(nextDebugMode);
            }

            switch (state)
            {
                case flow.Welcome:
                    if (Input.GetKeyDown(KeyCode.R)) state = flow.Prepare;

                    break;
                case flow.Prepare:

                    if (turn > max_turn)
                    {
                        // todo: round end , judge winner
                    }

                    if (prepareOk) state = flow.BeforeBattle;
                    break;
                case flow.BeforeBattle:
                    state = flow.Battle;
                    break;
                case flow.Battle:

                    break;
                case flow.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeDebugMode(bool nextDebugMode)
        {
            flowText.enabled = nextDebugMode;
            // foreach (var rPlayer in players) rPlayer.SetDebugText(nextDebugMode);
        }

        private void OnPrepareDone()
        {
            prepareOk = true;
        }

        private void OnOneShootEnd()
        {
            shootCount++;
            players[0].UsedDice();
            players[1].UsedDice();
            if (players[0].Hp <= 0)
            {
                if (players[1].score >= 2) state = flow.End;
                else state = flow.BeforeBattle;
                // next turn
            }
            else if (players[1].Hp <= 0)
            {
                if (players[0].score >= 2) state = flow.End;
                else state = flow.BeforeBattle;
                // next turn
            }
            else if (shootCount >= rPlayer.maxDiceCount)
            {
                shootCount = 0;
                state = flow.Prepare;
            }
            else
            {
                state = flow.BeforeBattle;
            }
        }
    }
}