using OpenTK;
using System.Collections.Generic;

namespace template
{
    public class Scene
    {
        private List<Light> _lights;
        private List<Primitive> _primitives;

        public List<Primitive> Primitives
        {
            get { return _primitives; }
        }

        public List<Light> Lights
        {
            get { return _lights; }
        }

        public Scene()
        {
            _lights = new List<Light>();
            _primitives = new List<Primitive>();
        }

        public void AddLight(Light l)
        {
            _lights.Add(l);
        }

        public void AddPrimitive(Primitive p)
        {
            _primitives.Add(p);
        }

        public Intersection GetClosestIntersection(VectorMath.Ray ray)
        {
            float minD = float.MaxValue;
            Intersection intersection = null;
            Intersection closest = null;
            foreach (Primitive p in _primitives)
            {
                intersection = p.GetIntersection(ray);
                if(intersection != null && intersection.Distance > 0 && intersection.Distance < minD)
                {
                    minD = intersection.Distance;
                    closest = intersection;
                }
            }
            return closest;
        }

        public Vector3 GetIntersectionColor(Intersection intersection)
        {
            Vector3 color = Vector3.Zero;
            foreach (Light l in _lights)
            {
                color += intersection.primitive.ComputeColor(intersection, l, this);
            }
            return color;
        }
    }
}