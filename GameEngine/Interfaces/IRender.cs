namespace InfiniTK.GameEngine
{
    public interface IRender : IPosition
    {
        void Load();
        void Update(double timeSinceLastUpdate);
        void Render();
    }
}
