namespace InfiniTK
{
    public class BlockCollision
    {
        public ICollide Collider { get; }
        public Block Block { get; }

        public BlockCollision(ICollide collider, Block block)
        {
            Collider = collider;
            Block = block;
        }
    }
}
