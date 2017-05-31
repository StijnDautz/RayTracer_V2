using OpenTK;

namespace template
{
    public abstract class Object
    {
        private Vector3 _position;

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Object(Vector3 position)
        {
            _position = position;
        }
    }
}
