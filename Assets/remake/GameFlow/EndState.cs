using System;

namespace remake.GameFlow
{
    public class EndState : IGameState
    {
        private readonly GM gm;


        public EndState(GM gm)
        {
            this.gm = gm;
        }

        public void OnExit()
        {
            gm.ResetWinner();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void OnPressR()
        {
            gm.ToWelcomeState();
        }

        public void OnEnter()
        {
            throw new NotImplementedException();
        }
    }
}