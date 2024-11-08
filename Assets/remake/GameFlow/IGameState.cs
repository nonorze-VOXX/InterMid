namespace remake.GameFlow
{
    public interface IGameState
    {
        void OnExit();
        void Update();
        void OnPressR();
        void OnEnter();
    }
}