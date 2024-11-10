using TMPro;

namespace remake.GameFlow
{
    public class EndState : IGameState
    {
        private readonly TMP_Text endText;
        private readonly GM gm;
        private readonly Player[] players;
        private readonly TMP_Text roundCountText;
        private readonly TMP_Text roundText;

        private readonly TMP_Text turnText;


        public EndState(GM gm, Player[] players, TMP_Text endText, TMP_Text turnText, TMP_Text roundText,
            TMP_Text roundCountText)
        {
            this.gm = gm;
            this.endText = endText;
            this.turnText = turnText;
            this.roundText = roundText;
            this.roundCountText = roundCountText;
            this.players = players;
        }

        public void OnExit()
        {
            gm.winner = null;
            endText.enabled = false;
            roundText.text = "";
            foreach (var rPlayer in players) rPlayer.NewRound();
            foreach (var rPlayer in players) rPlayer.ResetScore();
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
            endText.text = n + " win. \n" +
                           "Press r key to restart,\n" +
                           " Press esc to exit the game.\n" +
                           " thanks for playing.\n" +
                           "special thanks: \n" + "https://baka-nee.itch.io/background,\n" +
                           "https://pixelfrog-assets.itch.io/kings-and-pigs";
            endText.enabled = true;
            turnText.enabled = false;
            roundText.enabled = false;
            roundCountText.enabled = false;
        }
    }
}