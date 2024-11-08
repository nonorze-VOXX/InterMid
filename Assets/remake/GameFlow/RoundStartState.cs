using TMPro;

namespace remake.GameFlow
{
    public class RoundStartState : IGameState
    {
        private readonly GM gm;

        public RoundStartState(GM gm, rPlayer[] players, TMP_Text roundText)
        {
            this.gm = gm;
            this.roundText = roundText;
            this.players = players;
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
            players[0].gameObject.SetActive(true);
            players[1].gameObject.SetActive(true);
        }
    }
}