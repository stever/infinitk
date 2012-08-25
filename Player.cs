using System;
using OpenTK;

namespace InfiniTK
{
    public class Player : IMove, ICollide
    {
        #region Constants
        private const float MOUSE_LOOK_SPEED = 0.2f;
        private const float MOVEMENT_SPEED = 0.01f;
        private const float TURN_SPEED = 0.1f;
        #endregion

        private readonly Navigator _navigator = new Navigator();
        private readonly Starfield _starfield = new Starfield();
        private IAction _currentAction;

        public Player()
        {
            MoveToStartPosition();
        }

        public void MoveToStartPosition()
        {
            _navigator.Pitch = 0;
            _navigator.Yaw = 0;
            _navigator.Position = new Vector3d(0, 2, 0);
            _currentAction = null;
            if (Controls != null) Controls.Reset();
        }

        /// <summary>
        /// This method is used to handle movement controls.
        /// </summary>
        private void HandleControls(double timeSinceLastIdle)
        {
            if (Controls == null) return;

            // Calculate move and turn amounts for the given time.
            double moveAmount = MOVEMENT_SPEED * timeSinceLastIdle;
            float turnAmount = (float) (TURN_SPEED * timeSinceLastIdle);

            // Action handling.
            if (_currentAction == null)
            {
                if (Controls.JumpState == JumpState.InitiateJump)
                {
                    Controls.JumpState = JumpState.Jumping;
                    _currentAction = new Jump(this, Controls);
                }
            }

            // Move up and down. TODO: Restrict this control to a creative game.
            if (Controls.IsMovingUp) _navigator.Move(new Vector3d(0, moveAmount, 0));
            else if (Controls.IsMovingDown) _navigator.Move(new Vector3d(0, -moveAmount, 0));

            // Move forward and backward.
            if (Controls.IsMovingForward) _navigator.MoveForward(moveAmount);
            else if (Controls.IsMovingBackward) _navigator.MoveForward(-moveAmount);

            // Move left and right.
            if (Controls.IsMovingLeft) _navigator.MoveSideways(-moveAmount);
            else if (Controls.IsMovingRight) _navigator.MoveSideways(moveAmount);

            // Turn left and right.
            if (Controls.IsTurningLeft) _navigator.Spin(-turnAmount);
            else if (Controls.IsTurningRight) _navigator.Spin(turnAmount);

            // Look up and down.
            if (Controls.IsTurningUp) _navigator.Tilt(turnAmount);
            else if (Controls.IsTurningDown) _navigator.Tilt(-turnAmount);

            // Mouse control.
            if (!Controls.MouseControlEnabled) return;
            MouseDelta mouseDelta = Controls.GetMouseDelta();
            _navigator.Yaw += mouseDelta.X * MOUSE_LOOK_SPEED;
            _navigator.Pitch += mouseDelta.Y * -MOUSE_LOOK_SPEED;
        }

        public InputStates Controls { get; set; }

        #region IRender implementation

        public void Load()
        {
            _starfield.Load();
        }

        public void Update(double timeSinceLastUpdate)
        {
            HandleControls(timeSinceLastUpdate);
            if (_currentAction == null) return;
            if (_currentAction.Completed)
            {
                _currentAction.Finalise();
                _currentAction = null;
            }
            else _currentAction.Update(timeSinceLastUpdate);
        }

        public void Render()
        {
            _starfield.Position = _navigator.Position;
            _starfield.Render();
        }

        #endregion

        #region ICollide implementation

        public bool Collides(Block block)
        {
            // TODO: Player is 2 blocks tall. Collisons have to take both blocks into account!

            /*
            Vector3d playerPosition = _navigator.Position;
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
            Vector3d legsPosition = Vector3d.Add(_navigator.Position, -Vector3d.UnitY);
            BoundingBox head = BoundingBox.CreateFromSphere(_navigator.Position, 0.5f);
            BoundingBox legs = BoundingBox.CreateFromSphere(legsPosition, 0.5f);
            BoundingBox p = BoundingBox.CreateMerged(head, legs);
            Log.DebugFormat("Player bounding box = {0}", p);
            BoundingBox b = block.BoundingBox;
            return p.Intersects(b);
            */

            BoundingBox p = BoundingBox.CreateFromSphere(_navigator.Position, 0.5f);
            BoundingBox b = block.BoundingBox;
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
            Vector3d playerPosition = _navigator.Position;
            Vector3d distance = Vector3d.Subtract(playerPosition, block.Position);

            // Determine the axis with the largest difference.
            int furthest = 1; // 1=x, 2=y, 3=z
            double x = Math.Abs(distance.X);
            double y = Math.Abs(distance.Y);
            double z = Math.Abs(distance.Z);
            double highest = x;
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
            _navigator.Position = _lastPosition;            
            Vector3d move = Vector3d.Subtract(playerPosition, _lastPosition);
            _navigator.Move(Vector3d.Multiply(move, blocker));*/

            // Push player flat back on blocking face.
            x = _navigator.Position.X;
            y = _navigator.Position.Y;
            z = _navigator.Position.Z;
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
            _navigator.Position = new Vector3d(x, y, z);
        }

        public void HandleCollision(ICollide entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPosition implementation

        public Vector3d Position
        {
            get { return _navigator.Position; }
            set { _navigator.Position = value; }
        }

        #endregion

        #region IMove implementation

        public void ApplyCamera()
        {
            _navigator.ApplyCamera();
        }

        public void MoveForward(double amount)
        {
            _navigator.MoveForward(amount);
        }

        public void MoveSideways(double amount)
        {
            _navigator.MoveSideways(amount);
        }

        public void Move(Vector3d amount)
        {
            _navigator.Move(amount);
        }

        public void Spin(float degrees)
        {
            _navigator.Spin(degrees);
        }

        public void Tilt(float degrees)
        {
            _navigator.Tilt(degrees);
        }

        #endregion
    }
}
