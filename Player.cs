using System;
using InfiniTK.MonoXna;
using OpenTK;

namespace InfiniTK
{
    public class Player : IMove, ICollide
    {
        #region Constants

        private const float MouseLookSpeed = 0.2f;
        private const float MovementSpeed = 0.01f;
        private const float TurnSpeed = 0.1f;
        
        #endregion

        private readonly Navigator navigator = new Navigator();
        private readonly Starfield starfield = new Starfield();
        private IAction currentAction;

        public Player()
        {
            MoveToStartPosition();
        }

        public void MoveToStartPosition()
        {
            navigator.Pitch = 0;
            navigator.Yaw = 0;
            navigator.Position = new Vector3d(0, 2, 0);
            currentAction = null;
            Controls?.Reset();
        }

        /// <summary>
        /// This method is used to handle movement controls.
        /// </summary>
        private void HandleControls(double timeSinceLastIdle)
        {
            if (Controls == null) return;

            // Calculate move and turn amounts for the given time.
            var moveAmount = MovementSpeed * timeSinceLastIdle;
            var turnAmount = (float) (TurnSpeed * timeSinceLastIdle);

            // Action handling.
            if (currentAction == null)
            {
                if (Controls.JumpState == JumpState.InitiateJump)
                {
                    Controls.JumpState = JumpState.Jumping;
                    currentAction = new Jump(this, Controls);
                }
            }

            // Move up and down. TODO: Restrict this control to a creative game.
            if (Controls.IsMovingUp) navigator.Move(new Vector3d(0, moveAmount, 0));
            else if (Controls.IsMovingDown) navigator.Move(new Vector3d(0, -moveAmount, 0));

            // Move forward and backward.
            if (Controls.IsMovingForward) navigator.MoveForward(moveAmount);
            else if (Controls.IsMovingBackward) navigator.MoveForward(-moveAmount);

            // Move left and right.
            if (Controls.IsMovingLeft) navigator.MoveSideways(-moveAmount);
            else if (Controls.IsMovingRight) navigator.MoveSideways(moveAmount);

            // Turn left and right.
            if (Controls.IsTurningLeft) navigator.Spin(-turnAmount);
            else if (Controls.IsTurningRight) navigator.Spin(turnAmount);

            // Look up and down.
            if (Controls.IsTurningUp) navigator.Tilt(turnAmount);
            else if (Controls.IsTurningDown) navigator.Tilt(-turnAmount);

            // Mouse control.
            if (!Controls.MouseControlEnabled) return;
            var mouseDelta = Controls.GetMouseDelta();
            navigator.Yaw += mouseDelta.X * MouseLookSpeed;
            navigator.Pitch += mouseDelta.Y * -MouseLookSpeed;
        }

        public InputStates Controls { get; set; }

        #region IRender implementation

        public void Load()
        {
            starfield.Load();
        }

        public void Update(double timeSinceLastUpdate)
        {
            HandleControls(timeSinceLastUpdate);
            if (currentAction == null) return;
            if (currentAction.Completed)
            {
                currentAction.Finalise();
                currentAction = null;
            }
            else
            {
                currentAction.Update(timeSinceLastUpdate);
            }
        }

        public void Render()
        {
            starfield.Position = navigator.Position;
            starfield.Render();
        }

        #endregion

        #region ICollide implementation

        public bool Collides(Block block)
        {
            // TODO: Player is 2 blocks tall. Collisons have to take both blocks into account!

            /*
            Vector3d playerPosition = navigator.Position;
            double pX = playerPosition.X;
            double pY = playerPosition.Y;
            double pZ = playerPosition.Z;

            Vector3d blockPosition = block.Position;
            int ibX = (int) Math.Round(blockPosition.X);
            int ibY = (int) Math.Round(blockPosition.Y);
            int ibZ = (int) Math.Round(blockPosition.Z);

            bool inX = pX <= ibX + 1 && pX >= ibX - 1;
            bool inY = pY <= ibY + 1 && pY >= ibY - 1;
            bool inZ = pZ <= ibZ + 1 && pZ >= ibZ - 1;

            if (!inX || !inY || !inZ) return false;

            int ipX = (int) Math.Round(playerPosition.X);
            int ipY = (int) Math.Round(playerPosition.Y);
            int ipZ = (int) Math.Round(playerPosition.Z);

            return ipX == ibX || ipY == ibY || ipZ == ibZ;
            */

            /*
            Vector3d legsPosition = Vector3d.Add(navigator.Position, -Vector3d.UnitY);
            BoundingBox head = BoundingBox.CreateFromSphere(navigator.Position, 0.5f);
            BoundingBox legs = BoundingBox.CreateFromSphere(legsPosition, 0.5f);
            BoundingBox p = BoundingBox.CreateMerged(head, legs);
            Log.DebugFormat("Player bounding box = {0}", p);
            BoundingBox b = block.BoundingBox;
            return p.Intersects(b);
            */

            var p = BoundingBox.CreateFromSphere(navigator.Position, 0.5f);
            var b = block.BoundingBox;
            return p.Intersects(b);
        }

        public bool Collides(ICollide entity)
        {
            throw new NotImplementedException();
        }

        /*
        /// <summary>
        /// This method is used to determine if two spheres collide.
        /// </summary>
        private static bool SpheresCollide(double diameter, Vector3d a, Vector3d b)
        {
            double x = b.X - a.X;
            double y = b.Y - a.Y;
            double z = b.Z - a.Z;
            return (x * x + y * y + z * z) < (diameter * diameter);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        private static double DistanceBetween(Vector3d a, Vector3d b)
        {
            double x = b.X - a.X;
            double y = b.Y - a.Y;
            double z = b.Z - a.Z;
            return Math.Sqrt(x * x + y * y + z * z); // NOTE: Sqrt may be slow!
        }
        */

        public void HandleCollision(Block block)
        {
            // The distance between the player and the block.
            var playerPosition = navigator.Position;
            var distance = Vector3d.Subtract(playerPosition, block.Position);

            // Determine the axis with the largest difference.
            var furthest = 1; // 1=x, 2=y, 3=z
            var x = Math.Abs(distance.X);
            var y = Math.Abs(distance.Y);
            var z = Math.Abs(distance.Z);
            var highest = x;
            if (y > highest) { highest = y; furthest = 2; }
            if (z > highest) { furthest = 3; }

            /*
            Vector3d blocker = Vector3d.One;
            switch (furthest)
            {
                case 1: blocker = new Vector3d(0, 1, 1); break;
                case 2: blocker = new Vector3d(1, 0, 1); break;
                case 3: blocker = new Vector3d(1, 1, 0); break;
            }
            navigator.Position = _lastPosition;            
            Vector3d move = Vector3d.Subtract(playerPosition, _lastPosition);
            navigator.Move(Vector3d.Multiply(move, blocker));*/

            // Push player flat back on blocking face.
            x = navigator.Position.X;
            y = navigator.Position.Y;
            z = navigator.Position.Z;
            switch (furthest)
            {
                case 1:
                    if (distance.X < 0) x = block.Position.X - 1;
                    else x = block.Position.X + 1;
                    break;
                case 2: 
                    if (distance.Y < 0) y = block.Position.Y - 1;
                    else y = block.Position.Y + 1;
                    break;
                case 3: 
                    if (distance.Z < 0) z = block.Position.Z - 1;
                    else z = block.Position.Z + 1;
                    break;
            }
            navigator.Position = new Vector3d(x, y, z);
        }

        public void HandleCollision(ICollide entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPosition implementation

        public Vector3d Position
        {
            get { return navigator.Position; }
            set { navigator.Position = value; }
        }

        #endregion

        #region IMove implementation

        public void ApplyCamera()
        {
            navigator.ApplyCamera();
        }

        public void MoveForward(double amount)
        {
            navigator.MoveForward(amount);
        }

        public void MoveSideways(double amount)
        {
            navigator.MoveSideways(amount);
        }

        public void Move(Vector3d amount)
        {
            navigator.Move(amount);
        }

        public void Spin(float degrees)
        {
            navigator.Spin(degrees);
        }

        public void Tilt(float degrees)
        {
            navigator.Tilt(degrees);
        }

        #endregion
    }
}
