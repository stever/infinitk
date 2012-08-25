using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK
{
    /// <summary>
    /// Move a bunch of postions at the same time.
    /// </summary>
    public class Navigator : IMove
    {
        #region Pitch property

        private const float MAX_PITCH = 89.99f; // NOTE: Can't be >= 90.
        private const float MIN_PITCH = -MAX_PITCH;

        private float _pitch;

        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = GetLimitedPitch(value); }
        }

        /// <summary>
        /// This method limits the value of the pitch variable.
        /// </summary>
        private static float GetLimitedPitch(float pitch)
        {
            if (pitch > MAX_PITCH) pitch = MAX_PITCH;
            else if (pitch < MIN_PITCH) pitch = MIN_PITCH;
            return pitch;
        }

        #endregion

        #region Yaw property

        private float _yaw;

        public float Yaw
        {
            get { return _yaw; }
            set { _yaw = GetNormalYaw(value); }
        }

        /// <summary>
        /// This method provides a normal yaw value between 0 and 360 degrees.
        /// </summary>
        private static float GetNormalYaw(float yaw)
        {
            while (yaw >= 360.0f) yaw -= 360.0f;
            while (yaw < 0.0f) yaw += 360.0f;
            return yaw;
        }

        #endregion

        #region IPosition implementation

        // TODO: Check if property stil has a significant performance hit.
        public Vector3d Position { get; set; }

        #endregion

        #region IRender implementation

        /// <summary>
        /// Apply the camera from the given position and orientation.
        /// </summary>
        public void ApplyCamera()
        {
            double cosYaw = Math.Cos(MathHelper.DegreesToRadians(_yaw));
            double sinYaw = Math.Sin(MathHelper.DegreesToRadians(_yaw));
            double tanPitch = Math.Tan(MathHelper.DegreesToRadians(_pitch));

            // Calculate view target based on new position.
            Vector3d viewTarget = new Vector3d();
            viewTarget.X = Position.X + cosYaw;
            viewTarget.Y = Position.Y + tanPitch;
            viewTarget.Z = Position.Z + sinYaw;

            Matrix4d camera = Matrix4d.LookAt(Position, viewTarget, Vector3d.UnitY);
            GL.LoadMatrix(ref camera);
        }

        /// <summary>
        /// This method moves the entity in the direction it is facing.
        /// </summary>
        public void MoveForward(double amount)
        {
            Vector3d fwd = new Vector3d();
            double cosYaw = Math.Cos(MathHelper.DegreesToRadians(Yaw));
            double sinYaw = Math.Sin(MathHelper.DegreesToRadians(Yaw));
            fwd.X += cosYaw * amount;
            fwd.Z += sinYaw * amount;
            Move(fwd);
        }

        /// <summary>
        /// This method moves the entity sideways.
        /// </summary>
        public void MoveSideways(double amount)
        {
            Vector3d direction = new Vector3d();
            float angle = Yaw + 90;
            double cosYaw = Math.Cos(MathHelper.DegreesToRadians(GetNormalYaw(angle)));
            double sinYaw = Math.Sin(MathHelper.DegreesToRadians(GetNormalYaw(angle)));
            direction.X += cosYaw * amount;
            direction.Z += sinYaw * amount;
            Move(direction);
        }

        /// <summary>
        /// This method moves the entity using the given vector.
        /// </summary>
        public void Move(Vector3d amount)
        {
            Position += amount;
        }

        /// <summary>
        /// Turn to the side without moving up or down and without doing a roll.
        /// Also known as Yaw.
        /// </summary>
        /// <param name="degrees">The angle of the turn.</param>
        public void Spin(float degrees)
        {
            Yaw += degrees;
        }

        /// <summary>
        /// Tilt doesn't affect the direction of movement. This provides the
        /// ability to look up or down while moving, or be at an inclination 
        /// when walking up or down a slope.
        /// </summary>
        /// <param name="degrees">The angle to tilt.</param>
        public void Tilt(float degrees)
        {
            Pitch += degrees;
        }

        #endregion
    }
}
