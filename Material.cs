using OpenTK;

namespace template
{
    public class Material
    {
        private Vector3 _color;
        private float _reflectionIndex;
        private bool _textured;

        public Vector3 Color
        {
            get { return _color; }
        }

        public float ReflectionIndex
        {
            get { return _reflectionIndex; }
        }

        public Material(Vector3 color, float reflectionIndex, bool textured)
        {
            _color = color;
            _reflectionIndex = reflectionIndex;
            _textured = textured;
        }

        public Vector3 ComputeColor(Vector3 alpha, Vector3 intersection)
        {
            if(_textured)
            {
                int c = (int)intersection.X % 2;
                return new Vector3(c, c, c) * alpha;
            }
            else
            { return _color * alpha; }
        }
    }
}
