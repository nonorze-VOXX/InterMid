using System.Collections.Generic;
using remake.GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace remake
{
    internal enum TextIndex
    {
        GameState,
        Turn,
        Round,
        RoundCount,
        EndText
    }

    internal enum ButtonIndex
    {
        TurnAddOne,
        P1Triple,
        P1AttackUp,
        P1HpDecrease
    }

    public class GM : MonoBehaviour
    {
        private static readonly int PLAYER_COUNT = 2;

        private static readonly int max_turn = 10;

        [SerializeField] private Player playerPrefab;

        [SerializeField] private TMP_Text welcomeText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text flowText;
        [SerializeField] private TMP_Text roundText;
        [SerializeField] private TMP_Text roundCountText;
        [SerializeField] private TMP_Text endText;

        [SerializeField] private Button turnAddOneButton;
        [SerializeField] private Button p1TripleButton;
        [SerializeField] private Button p1AttackUpButton;
        [SerializeField] private Button p1HpDecreaseButton;

        public int shootCount;
        public Player winner;

        private readonly HashSet<Player> preparedPlayer = new();

        private IGameState _gameState;
        private int _round;


        private int _turn;
        private Player[] players;

        public bool prepareOk { get; set; }

        public int round
        {
            get => _round;
            set
            {
                _round = value;
                roundCountText.text = "Round: " + round;
            }
        }

        public int turn
        {
            get => _turn;
            set
            {
                _turn = value;
                turnText.text = "Turn: " + turn;
            }
        }

        private IGameState GameState
        {
            get => _gameState;
            set
            {
                flowText.text = value.GetType().ToString().Split('.')[^1];
                _gameState?.OnExit();
                _gameState = value;
                _gameState?.OnEnter();
            }
        }

        private void Awake()
        {
            SkillManager.ReadSkill();

            #region get_text_object

            flowText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.GameState];
            flowText.enabled = false;
            turnText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.Turn];
            turnText.enabled = false;
            roundText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.Round];
            roundText.text = "Press R to Start round";

            roundCountText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.RoundCount];
            round = 0;
            roundText.enabled = false;

            endText = GetComponentsInChildren<TMP_Text>()[(int)TextIndex.EndText];
            endText.enabled = false;

            #endregion

            #region get_button_object

            turnAddOneButton = GetComponentsInChildren<Button>()[(int)ButtonIndex.TurnAddOne];
            turnAddOneButton.onClick.AddListener(() => turn++);
            p1TripleButton = GetComponentsInChildren<Button>()[(int)ButtonIndex.P1Triple];
            p1TripleButton.onClick.AddListener(() =>
            {
                if (players.Length > 0)
                    players[0].NextThrowTripleDice();
            });
            p1AttackUpButton = GetComponentsInChildren<Button>()[(int)ButtonIndex.P1AttackUp];
            p1AttackUpButton.onClick.AddListener(() =>
            {
                if (players.Length > 0)
                    players[0].AddAttack(1);
            });
            p1HpDecreaseButton = GetComponentsInChildren<Button>()[(int)ButtonIndex.P1HpDecrease];
            p1HpDecreaseButton.onClick.AddListener(() =>
            {
                if (players.Length > 0)
                    players[0].Hp -= 50;
            });

            #endregion

            players = new Player[PLAYER_COUNT];
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

            players[1].GetComponentInChildren<SpriteRenderer>().flipX = true;
            var pos = players[1].GetComponentInChildren<Slider>().transform.localPosition;
            pos.x *= -1;
            players[1].GetComponentInChildren<Slider>().transform.localPosition = pos;

            #endregion

            ToWelcomeState();
            ChangeDebugMode(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var nextDebugMode = !flowText.enabled;
                ChangeDebugMode(nextDebugMode);
            }

            if (Input.GetKeyDown(KeyCode.R)) GameState.OnPressR();
            GameState.Update();
        }

        private void ChangeDebugMode(bool nextDebugMode)
        {
            flowText.enabled = nextDebugMode;
            turnAddOneButton.gameObject.SetActive(nextDebugMode);
            p1TripleButton.gameObject.SetActive(nextDebugMode);
            p1AttackUpButton.gameObject.SetActive(nextDebugMode);
            p1HpDecreaseButton.gameObject.SetActive(nextDebugMode);
        }


        public bool AllPlayerPrepared()
        {
            return preparedPlayer.Count == 2;
        }

        public void ClearPreparedPlayer()
        {
            preparedPlayer.Clear();
        }

        public void AddPreparedPlayer(Player player)
        {
            preparedPlayer.Add(player);
        }

        public void SetRoundText(string noOneWinThisRoundPressRToNextRound)
        {
            roundText.text = noOneWinThisRoundPressRToNextRound;
        }

        #region flowChange

        public void ToBeforeBattleState()
        {
            GameState = new BeforeBattleState(this);
        }

        public void ToWelcomeState()
        {
            GameState = new WelcomeState(this, welcomeText);
        }

        public void ToEndState()
        {
            // clean dice
            for (var i = 0; i < players.Length; i++)
            for (var j = 0; j < players[i].GetDiceCount(); j++)
                players[i].UsedDice();
            GameState = new EndState(this, players, endText, turnText, roundText, roundCountText);
        }

        public void ToRoundStartState()
        {
            GameState = new RoundStartState(this, players, roundText, turnText, roundCountText);
        }

        public void ToPrepareState()
        {
            GameState = new PrepareState(this, players, turnText);
        }

        public void ToBattleState()
        {
            GameState = new BattleState(this, players, roundText);
        }

        #endregion
    }
}