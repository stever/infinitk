namespace MoonPad.Engine
{
    public interface IAction
    {
        void Update(double timeSinceLastUpdate);
        bool Completed { get; }
        void Finalise();
    }
}
