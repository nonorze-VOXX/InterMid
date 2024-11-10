using TMPro;

namespace remake.GameFlow
{
    public class WelcomeState : IGameState
    {
        private readonly GM gm;
        private readonly TMP_Text turnText;
        private readonly TMP_Text welcomeText;

        public WelcomeState(GM gm, TMP_Text welcomeText, TMP_Text turnText = null)
        {
            this.gm = gm;
            this.welcomeText = welcomeText;
            this.turnText = turnText;
        }

        public void OnExit()
        {
            welcomeText.gameObject.SetActive(false);
        }

        public void Update()
        {
        }

        public void OnPressR()
        {
            gm.ToRoundStartState();
        }

        public void OnEnter()
        {
            welcomeText.gameObject.SetActive(true);
            welcomeText.text = "press r key to start";
            if (turnText) turnText.enabled = false;
        }
    }
}