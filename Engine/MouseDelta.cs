namespace InfiniTK.Engine
{
    public struct MouseDelta
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Wheel;

        public MouseDelta(int x, int y, int wheel)
        {
            X = x;
            Y = y;
            Wheel = wheel;
        }
    }
}
