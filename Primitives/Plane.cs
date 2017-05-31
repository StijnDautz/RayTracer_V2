using OpenTK;

namespace template
{
    public class Plane : Primitive
    {
        private Vector3 _normal;
        
        public Plane(Vector3 position, Vector3 normal, Vector3 color, Material material) : base(position, color, material)
        {
            _normal = normal;
        }

        public override Intersection GetIntersection(VectorMath.Ray ray)
        {
            //if the ray is not parralel to the plane, calculate the intersection point
            float dotN_R = Vector3.Dot(_normal, ray.direction);
            if(dotN_R < 0)
            {
                Vector3 p = ray.origin + (-(Vector3.Dot(ray.origin, _normal) + Position.Length) / dotN_R) * ray.direction;
                return new Intersection(this, p, _normal, ray, (p - ray.origin).Length);
            }
            else { return null; }
        }
    }
}