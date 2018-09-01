using OpenTK.Input;

namespace MoonPad.Utility
{
    public static class Mouse
    {
        private static MouseState previousMouseState;

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

        /// <summary>
        /// This method returns the delta to show how much the mouse has moved
        /// since the last time this method was called.
        /// </summary>
        /// <returns></returns>
        public static MouseDelta GetMouseDelta()
        {
            var mouse = OpenTK.Input.Mouse.GetState();

            var deltaX = mouse.X - previousMouseState.X;
            var deltaY = mouse.Y - previousMouseState.Y;
            var deltaZ = mouse.Wheel - previousMouseState.Wheel;

            previousMouseState = mouse;

            return new MouseDelta(deltaX, deltaY, deltaZ);
        }

        /// <summary>
        /// This method saves the current mouse state so that deltas are compared
        /// to this snapshot and not one from earlier.
        /// </summary>
        public static void SnapshotCurrentMouseState()
        {
            previousMouseState = OpenTK.Input.Mouse.GetState();
        }
    }
}
