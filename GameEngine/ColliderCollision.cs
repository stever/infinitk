using InfiniTK.Engine;

namespace InfiniTK.GameEngine
{
    public class ColliderCollision
    {
        public ICollide Collider { get; }
        public ICollide Other { get; }

        public ColliderCollision(ICollide collider, ICollide other)
        {
            Collider = collider;
            Other = other;
        }
    }
}
