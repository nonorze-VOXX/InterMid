using TMPro;

namespace remake.GameFlow
{
    public class EndState : IGameState
    {
        private readonly TMP_Text endText;
        private readonly GM gm;
        private readonly TMP_Text roundCountText;
        private readonly TMP_Text roundText;

        private readonly TMP_Text turnText;


        public EndState(GM gm, TMP_Text endText, TMP_Text turnText, TMP_Text roundText,
            TMP_Text roundCountText)
        {
            this.gm = gm;
            this.endText = endText;
            this.turnText = turnText;
            this.roundText = roundText;
            this.roundCountText = roundCountText;
        }

        public void OnExit()
        {
            gm.ResetWinner();
            endText.enabled = false;
        }

        public void Update()
        {
        }

        public void OnPressR()
        {
            gm.ToWelcomeState();
        }

        public void OnEnter()
        {
            var n = gm.winner.transform.name;
            endText.text = n + " win. \nPress r key to restart,\n Press esc to exit the game.";
            endText.enabled = true;
            turnText.enabled = false;
            roundText.enabled = false;
            roundCountText.enabled = false;
        }
    }
}