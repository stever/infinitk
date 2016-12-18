using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK.Engine
{
    public abstract class SceneObject
    {
        public Vector3d Position { get; set; }

        public abstract void Load();

        public abstract void Update(double timeSinceLastUpdate);

        public abstract void Render();
        private const float MaxPitch = 89.99f; // NOTE: Can't be >= 90.
        private const float MinPitch = -MaxPitch;

        private float pitch;
        private float yaw;

        public float Pitch
        {
            get { return pitch; }
            set { pitch = GetLimitedPitch(value); }
        }

        public float Yaw
        {
            get { return yaw; }
            set { yaw = GetNormalYaw(value); }
        }

        /// <summary>
        /// This method limits the value of the pitch variable.
        /// </summary>
        private static float GetLimitedPitch(float pitch)
        {
            if (pitch > MaxPitch) pitch = MaxPitch;
            else if (pitch < MinPitch) pitch = MinPitch;
            return pitch;
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

        /// <summary>
        /// Apply the camera from the given position and orientation.
        /// </summary>
        public void ApplyCamera()
        {
            var cosYaw = Math.Cos(MathHelper.DegreesToRadians(yaw));
            var sinYaw = Math.Sin(MathHelper.DegreesToRadians(yaw));
            var tanPitch = Math.Tan(MathHelper.DegreesToRadians(pitch));

            // Calculate view target based on new position.
            var viewTarget = new Vector3d
            {
                X = Position.X + cosYaw,
                Y = Position.Y + tanPitch,
                Z = Position.Z + sinYaw
            };

            var camera = Matrix4d.LookAt(Position, viewTarget, Vector3d.UnitY);
            GL.LoadMatrix(ref camera);
        }

        /// <summary>
        /// This method moves the entity in the direction it is facing.
        /// </summary>
        public void MoveForward(double amount)
        {
            var fwd = new Vector3d();
            var cosYaw = Math.Cos(MathHelper.DegreesToRadians(Yaw));
            var sinYaw = Math.Sin(MathHelper.DegreesToRadians(Yaw));
            fwd.X += cosYaw * amount;
            fwd.Z += sinYaw * amount;
            Move(fwd);
        }

        /// <summary>
        /// This method moves the entity sideways.
        /// </summary>
        public void MoveSideways(double amount)
        {
            var direction = new Vector3d();
            var angle = Yaw + 90;
            var cosYaw = Math.Cos(MathHelper.DegreesToRadians(GetNormalYaw(angle)));
            var sinYaw = Math.Sin(MathHelper.DegreesToRadians(GetNormalYaw(angle)));
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
    }
}
