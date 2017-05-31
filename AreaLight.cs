using OpenTK;
using System.Drawing;

namespace template
{
    class AreaLight : Light
    {
        private Point _size;
        private int _lightRayCap = 1;
        Vector3 _v1;
        Vector3 _v2;

        public AreaLight(Vector3 position, Vector3 color, Vector3 intensity, Vector3 v1, Vector3 v2, Point size) : base(position, color, intensity)
        {
            _size = size;
            _v1 = v1;
            _v2 = v2;
        }

        protected override float GetAttenuation(Intersection intersection, Scene scene)
        {
            if (Raytracer._renderHigh)
            {
                float attentuation = 0;
                Vector3 rndV;
                int xm = _size.X * _lightRayCap;
                int ym = _size.Y * _lightRayCap;
                float xf = 1f / xm;
                float yf = 1f / xm;
                for (int x = 0; x < xm; x++)
                {
                    for (int y = 0; y < ym; y++)
                    {
                        rndV = Position + _v1 * x * xf + _v2 * y * yf;
                        if (!InShadow(intersection, rndV, scene))
                        { attentuation += GetAttenuation(intersection, rndV, scene); }
                    }
                }
                return attentuation / (xm * ym);
            }
            return 1;
        }
    }
}
