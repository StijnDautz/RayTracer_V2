using OpenTK;

namespace template
{
    class Camera
    {
        private Vector3 _position;
        private Vector3 _direction;
        private float _offset;
        private Screen _screen;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                //TODO change _screen corners
            }
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                //TODO change _screen corners
            }
        }

        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public Screen Screen
        {
            get { return _screen; }
        }

        public Camera(Vector3 position, Vector3 direction, Screen screen)
        {
            _position = position;
            _direction = direction;
            _screen = screen;
        }
    }
}
