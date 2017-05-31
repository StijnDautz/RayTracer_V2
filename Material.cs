using OpenTK;

namespace template
{
    public class Material
    {
        private Vector3 _color;
        private bool _isReflective;
        private bool _textured;

        public Vector3 Color
        {
            get { return _color; }
        }

        public bool IsReflective
        {
            get { return _isReflective; }
        }

        public Material(Vector3 color, bool isReflective, bool textured)
        {
            _color = color;
            _isReflective = isReflective;
            _textured = textured;
        }

        public Vector3 ComputeColor(Vector3 alpha, Vector3 intersection)
        {
            //Vector3 color = _textured ? (int)intersection.X % 2 == 0 ? Vector3.Zero : new Vector3(1, 1, 1) : _color;
            return _color * alpha;
        }
    }
}
