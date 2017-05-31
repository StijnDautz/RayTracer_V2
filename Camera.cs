using OpenTK;
using OpenTK.Graphics.ES10;
using System;

namespace template
{
    class Camera
    {
        private Vector3 _position, _direction;
        private float _offset;
        private Screen _screen;

        public Vector3 Position
        {
            get { return _position; }
        }

        public Vector3 Direction
        {
            get { return _direction; }
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
            _offset = (_screen.Position - _position).Length;
        }

        public void Move(Vector3 direction)
        {
            _position += direction;
            _screen.Move(direction);
        }

        public void LookAt(Vector3 target)
        {
            _direction = target;
            _screen.Rotate(target, _offset);
        }
    }
}
