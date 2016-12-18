namespace InfiniTK.GameEngine
{
    public interface IAction
    {
        void Update(double timeSinceLastUpdate);
        bool Completed { get; }
        void Finalise();
    }
}
