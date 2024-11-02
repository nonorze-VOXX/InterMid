namespace GameFlow
{
    public abstract class IGameFlow : IState
    {
        public virtual void OnEnter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}