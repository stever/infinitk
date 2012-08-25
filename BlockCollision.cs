namespace InfiniTK
{
    public class BlockCollision
    {
        public ICollide Collider { get; private set; }
        public Block Block { get; private set; }

        public BlockCollision(ICollide collider, Block block)
        {
            Collider = collider;
            Block = block;
        }
    }
}
