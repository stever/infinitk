namespace InfiniTK.Engine
{
    public interface IRender : IPosition
    {
        void Load();
        void Update(double timeSinceLastUpdate);
        void Render();
    }
}
