namespace InfiniTK
{
    public class ColliderCollision
    {
        public ICollide Collider { get; private set; }
        public ICollide Other { get; private set; }

        public ColliderCollision(ICollide collider, ICollide other)
        {
            Collider = collider;
            Other = other;
        }
    }
}
