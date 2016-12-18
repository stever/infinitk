using System;
using OpenTK;

namespace InfiniTK
{
    public class Jump : IAction
    {
        private readonly IMove entity;
        private readonly InputStates controls;
        private readonly double startingHeight;
        private double timeSinceStart;

        public Jump(IMove entity, InputStates controls)
        {
            this.entity = entity;
            this.controls = controls;
            startingHeight = this.entity.Position.Y;
        }

        #region IAction implementation

        public void Update(double timeSinceLastUpdate)
        {
            if (Completed) return;

            timeSinceStart += timeSinceLastUpdate;

            // Jump initially follows a curve, from upward to downward.
            double moveAmount; // Falling motion does not slow down, then using constant speed.
            if (timeSinceStart < Math.PI * 100) moveAmount = Math.Sin(timeSinceStart / 100) * 0.125;
            else moveAmount = (-(timeSinceStart / 100) + Math.PI) * 0.125 / 2;
            entity.Move(new Vector3d(0, moveAmount, 0));

            // If after falling at constant speed we reach starting height, stop and
            // end the jump action at the same height we started with.
            if (entity.Position.Y > startingHeight) return;
            entity.Move(new Vector3d(0, startingHeight - entity.Position.Y, 0));
            Completed = true;

            /* TODO: Use real gravity/acceleration calculation in downward motion.
            Gravity is just constant acceleration downwards.
            http://stackoverflow.com/questions/3966188/c-sharp-xna-simulate-gravity
            const Vector3D Gravity=(0, 0, -9.8 m/s^2);
            Vector3D Acceleration=Gravity;//insert other forces here
            Vector3D Position+=Speed*DeltaT+0.5*Acceleration*DeltaT*DeltaT.
            Vector3D Speed+=Acceleration*DeltaT;*/
        }

        public bool Completed { get; private set; }

        public void Finalise()
        {
            controls.JumpState = JumpState.NotJumping;
        }

        #endregion
    }
}
