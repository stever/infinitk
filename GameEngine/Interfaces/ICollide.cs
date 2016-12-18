namespace InfiniTK.GameEngine
{
    public interface ICollide : IRender
    {
        bool Collides(Block entity);
        bool Collides(ICollide entity);
        void HandleCollision(Block block);
        void HandleCollision(ICollide entity);
    }
}
