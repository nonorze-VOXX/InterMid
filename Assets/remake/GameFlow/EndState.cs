using TMPro;

namespace remake.GameFlow
{
    public class EndState : IGameState
    {
        private readonly GM gm;
        private readonly TMP_Text endText;


        public EndState(GM gm, TMP_Text endText)
        {
            this.gm = gm;
            this.endText = endText;
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
            endText.text = "press r key to restart,\n Press esc to exit the game.";
            endText.enabled = true;
        }
    }
}