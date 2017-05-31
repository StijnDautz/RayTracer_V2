using OpenTK;
using OpenTK.Input;

namespace template
{
    class InputManager
    {
        private KeyboardState _previousKeyBoardState;
        private KeyboardState _keyBoardState;
        private MouseState _previousMouseState;
        private MouseState _mouseState;

        public Vector2 MousePosition
        {
            get { return new Vector2(_mouseState.X, _mouseState.Y); }
        }

        public float ScollWheelMovement
        {
            get { return _mouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue; }
        }

        public bool LeftMouseButtonDown
        {
            get { return _mouseState.LeftButton == ButtonState.Pressed; }
        }

        public void Update()
        {
            _previousKeyBoardState = _keyBoardState;
            _keyBoardState = Keyboard.GetState();
            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public bool ButtonDown(Key k)
        {
            return _keyBoardState.IsKeyDown(k);
        }
    }
}