using TMPro;

namespace remake.GameFlow
{
    public class RoundStartState : IGameState
    {
        private readonly GM gm;
        private readonly TMP_Text turnText;

        public RoundStartState(GM gm, rPlayer[] players, TMP_Text roundText, TMP_Text turnText)
        {
            this.gm = gm;
            this.roundText = roundText;
            this.players = players;
            this.turnText = turnText;
        }

        public TMP_Text roundText { get; set; }

        public rPlayer[] players { get; set; }


        public void OnExit()
        {
            // throw new System.NotImplementedException();
            roundText.enabled = false;
            foreach (var rPlayer in players) rPlayer.NewRound();
        }


        public void Update()
        {
        }

        public void OnPressR()
        {
            gm.ToPrepareState();
        }

        public void OnEnter()
        {
            gm.round++;
            gm.turn = 0;
            roundText.enabled = true;
            turnText.enabled = false;
            // players[0].AtkReset();
            players[0].gameObject.SetActive(true);
            // players[1].AtkReset();
            players[1].gameObject.SetActive(true);
        }
    }
}