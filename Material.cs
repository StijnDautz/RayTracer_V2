using OpenTK;
using System;

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
            { return (((int)(Math.Floor(intersection.X) + Math.Floor(intersection.Y) + Math.Floor(intersection.Z)) & 1) == 0) ? Vector3.Zero : new Vector3(1, 1, 1); }
            else
            { return _color * alpha; }
        }
    }
}
