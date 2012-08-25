using System.Reflection;
using log4net;
using OpenTK.Input;

namespace InfiniTK
{
    public class InputStates
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public JumpState JumpState { get; set; }

        #region Mouse handling data

        public bool MouseControlEnabled { get; set; }

        private MouseState _previousMouseState;

        #endregion

        #region Movement state properties
        public bool IsMovingForward
        {
            get
            {
                if (_moveForwardKeyDown) 
                    return !_moveBackwardKeyDown || _moveForwardOverBackward;
                return false;
            }
        }
        public bool IsMovingBackward
        {
            get
            {
                if (_moveBackwardKeyDown)
                    return !_moveForwardKeyDown || !_moveForwardOverBackward;
                return false;
            }
        }
        public bool IsMovingLeft
        {
            get
            {
                if (_moveLeftKeyDown)
                    return !_moveRightKeyDown || _moveLeftOverRight;
                return false;
            }
        }
        public bool IsMovingRight
        {
            get
            {
                if (_moveRightKeyDown)
                    return !_moveLeftKeyDown || !_moveLeftOverRight;
                return false;
            }
        }
        public bool IsMovingUp
        {
            get
            {
                if (_moveUpKeyDown)
                    return !_moveDownKeyDown || _moveUpOverDown;
                return false;
            }
        }
        public bool IsMovingDown
        {
            get
            {
                if (_moveDownKeyDown)
                    return !_moveUpKeyDown || !_moveUpOverDown;
                return false;
            }
        }
        public bool IsTurningLeft
        {
            get
            {
                if (_turnLeftKeyDown)
                    return !_turnRightKeyDown || _turnLeftOverRight;
                return false;
            }
        }
        public bool IsTurningRight
        {
            get
            {
                if (_turnRightKeyDown)
                    return !_turnLeftKeyDown || !_turnLeftOverRight;
                return false;
            }
        }
        public bool IsTurningUp
        {
            get
            {
                if (_turnUpKeyDown)
                    return !_turnDownKeyDown || _turnUpOverDown;
                return false;
            }
        }
        public bool IsTurningDown
        {
            get
            {
                if (_turnDownKeyDown)
                    return !_turnUpKeyDown || !_turnUpOverDown;
                return false;
            }
        }
        #endregion

        #region Keyboard state variables

        private bool _jumpKeyDown;

        // Forward and backward movement.
        private bool _moveForwardKeyDown;
        private bool _moveBackwardKeyDown;
        private bool _moveForwardOverBackward = true;

        // Left and right movement.
        private bool _moveLeftKeyDown;
        private bool _moveRightKeyDown;
        private bool _moveLeftOverRight = true;

        // Up and down movement.
        private bool _moveUpKeyDown;
        private bool _moveDownKeyDown;
        private bool _moveUpOverDown = true;

        // Left and right spinning.
        private bool _turnLeftKeyDown;
        private bool _turnRightKeyDown;
        private bool _turnLeftOverRight = true;

        // Up and down tilting.
        private bool _turnUpKeyDown;
        private bool _turnDownKeyDown;
        private bool _turnUpOverDown = true;

        #endregion

        /// <summary>
        /// Do a complete reset of the controls, except mouse controls.
        /// </summary>
        public void Reset()
        {
            ResetKeyStates();
            JumpState = JumpState.NotJumping;
        }

        #region Keyboard handling methods

        /// <summary>
        /// Resets the variables used for key states.
        /// </summary>
        public void ResetKeyStates()
        {
            _jumpKeyDown = false;
            _moveForwardKeyDown = false;
            _moveBackwardKeyDown = false;
            _moveLeftKeyDown = false;
            _moveRightKeyDown = false;
            _moveUpKeyDown = false;
            _moveDownKeyDown = false;
            _turnLeftKeyDown = false;
            _turnRightKeyDown = false;
            _turnUpKeyDown = false;
            _turnDownKeyDown = false;
        }

        public void KeyDown(Key key)
        {
            if (Log.IsDebugEnabled) Log.Debug("KeyDown: " + key);
            ToggleKey(key, true);
        }

        public void KeyUp(Key key)
        {
            if (Log.IsDebugEnabled) Log.Debug("KeyUp: " + key);
            ToggleKey(key, false);
        }

        private void ToggleKey(Key key, bool keyDown)
        {
            switch (key)
            {
                case Key.Space:
                    if (!keyDown) _jumpKeyDown = false;
                    else if (!_jumpKeyDown)
                    {
                        _jumpKeyDown = true;
                        if (JumpState == JumpState.NotJumping)
                            JumpState = JumpState.InitiateJump;
                    }
                    break;
                case Key.Plus:
                    _moveUpKeyDown = keyDown;
                    if (keyDown) _moveUpOverDown = true;
                    break;
                case Key.Minus:
                    _moveDownKeyDown = keyDown;
                    if (keyDown) _moveUpOverDown = false;
                    break;
                case Key.W:
                    _moveForwardKeyDown = keyDown;
                    if (keyDown) _moveForwardOverBackward = true;
                    break;
                case Key.S:
                    _moveBackwardKeyDown = keyDown;
                    if (keyDown) _moveForwardOverBackward = false;
                    break;
                case Key.A:
                    _moveLeftKeyDown = keyDown;
                    if (keyDown) _moveLeftOverRight = true;
                    break;
                case Key.D:
                    _moveRightKeyDown = keyDown;
                    if (keyDown) _moveLeftOverRight = false;
                    break;
                    /*
                case Key.Up:
                    if (!keyDown) _turnUpKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        _turnUpKeyDown = true;
                        _turnUpOverDown = true;
                    }
                    break;
                case Key.Down:
                    if (!keyDown) _turnDownKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        _turnDownKeyDown = true;
                        _turnUpOverDown = false;
                    }
                    break;
                case Key.Left:
                    if (!keyDown) _turnLeftKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        _turnLeftKeyDown = true;
                        _turnLeftOverRight = true;
                    }
                    break;
                case Key.Right:
                    if (!keyDown) _turnRightKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        _turnRightKeyDown = true;
                        _turnLeftOverRight = false;
                    }
                    break;*/
            }
        }

        #endregion

        #region Mouse handling methods

        /// <summary>
        /// This method saves the current mouse state so that deltas are compared
        /// to this snapshot and not one from earlier.
        /// </summary>
        public void SnapshotCurrentMouseState()
        {
            _previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// This method returns the delta to show how much the mouse has moved
        /// since the last time this method was called.
        /// </summary>
        /// <returns></returns>
        public MouseDelta GetMouseDelta()
        {
            MouseState mouse = Mouse.GetState();

            int deltaX = mouse.X - _previousMouseState.X;
            int deltaY = mouse.Y - _previousMouseState.Y;
            int deltaZ = mouse.Wheel - _previousMouseState.Wheel;

            _previousMouseState = mouse;

            return new MouseDelta(deltaX, deltaY, deltaZ);
        }

        #endregion
    }
}
