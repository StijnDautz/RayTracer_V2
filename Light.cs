using OpenTK;

namespace template
{
    public class Light : Object
    {
        private Vector3 _color;
        private Vector3 _brightness;

        public Vector3 Color
        {
            get { return _color; }
        }

        public Vector3 Brightness
        {
            get { return _brightness; }
        }
        public Light(Vector3 position, Vector3 color, Vector3 intensity) : base(position)
        {
            _color = color;
            _brightness = intensity;
        }

        /*
         * Gets the the light attenuation depending on the type of light source
         */
        protected virtual float GetAttenuation(Intersection intersection, Scene scene)
        {
            return !InShadow(intersection, Position, scene) ? GetAttenuation(intersection, Position, scene) : 0;
        }

        /*
         * Calculates the light attenuation from a position to an intersection
         */
        protected float GetAttenuation(Intersection intersection, Vector3 position, Scene scene)
        {
            float dist = (position - intersection.IntersectionPoint).Length;
            return 1 - (1 / (dist * dist * 2));
        }

        /*
         * Computes the illumination using the formula: lightColor * brightness * lightIntensity *  Dot(normal, intersectionTolight)
         *      - Dot(normal, intersectionTolight) : the bigger, the more the light is "above" the intersection.
         */
        public Vector3 ComputeIllumination(Intersection intersection, Scene scene)
        {
            return _color * GetAttenuation(intersection, scene) * _brightness * Vector3.Dot(intersection.Normal, (Position - intersection.IntersectionPoint).Normalized());
        }

        public bool InShadow(Intersection intersection, Vector3 position, Scene scene)
        {
            Vector3 dir = position - intersection.IntersectionPoint;
            //if the dot product is smaller then one, then the light source is behind the primitive itself and so we immediately return true,
            //to prevent looping through all primitives in the scene
            if (Vector3.Dot(dir.Normalized(), intersection.Normal) < 0)
            { return true; }

            //if there is an intersection with a primitive and this primitive is closer to the intersection then the light source, then the lightRay can not hit the intersection and we return true
            Intersection i = scene.GetClosestIntersection(new VectorMath.Ray(intersection.IntersectionPoint + 0.001f * dir.Normalized(), dir));
            return i != null && dir.Length > i.Distance;
        }
    }
}