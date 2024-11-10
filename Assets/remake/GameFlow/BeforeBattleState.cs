namespace remake.GameFlow
{
    public class BeforeBattleState : IGameState
    {
        private readonly GM gm;

        public BeforeBattleState(GM gm)
        {
            this.gm = gm;
        }

        public void OnExit()
        {
        }

        public void Update()
        {
            gm.ToBattleState();
        }

        public void OnPressR()
        {
        }

        public void OnEnter()
        {
        }
    }
}