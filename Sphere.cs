using OpenTK;
using System;
using System.Drawing;

namespace template
{
    class Sphere : Primitive
    {
        private float _radius;

        public Sphere(Vector3 position, int radius, Vector3 color, Material material) : base(position, color, material)
        {
            _radius = radius;
        }

        public float Radius
        {
            get { return _radius; }
        }

        public override Intersection GetIntersection(VectorMath.Ray ray)
        {
            Vector3 centerToOrigin = ray.origin - Position;
            float a = ray.direction.LengthSquared;
            float b = 2 * Vector3.Dot(ray.direction, centerToOrigin);
            float c = centerToOrigin.LengthSquared - _radius * _radius;

            float d = b * b - 4 * a * c;
            if(d < 0)
            { return null; }
            else
            {
                float sqrtD = (float)Math.Sqrt(d);
                float t0 = (-b + sqrtD) / (2 * a);
                float t1 = (-b - sqrtD) / (2 * a);
                float distance = Math.Min(t0, t1);
                Vector3 intersectionPoint = ray.origin + distance * ray.direction;
                return new Intersection(this, intersectionPoint, (intersectionPoint - Position).Normalized(), ray, distance);
            }
        }

        public override Point GenerateUV(Vector3 worldCoords)
        {
            return new Point((int)(0.5 + Math.Atan2(worldCoords.Z, worldCoords.X) / (2 * Math.PI)), (int)(0.5 + Math.Asin(worldCoords.Y) / Math.PI));
        }
    }
}