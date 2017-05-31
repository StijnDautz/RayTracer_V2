using OpenTK;

namespace template
{
    public static class VectorMath
    {
        public struct Ray
        {
            public Vector3 origin;
            public Vector3 direction;
            public float magnitude;

            public Ray(Vector3 o, Vector3 d)
            {
                origin = o;
                magnitude = d.Length;
                direction = d.Normalized();
            }

            public Vector3 Position
            {
                get { return origin + magnitude * direction; }
            }
        }

        public static Vector3 Reflect(Vector3 incoming, Vector3 normal)
        {
            return incoming - normal * (2 * Vector3.Dot(incoming, normal));
        }

        public static int GetColorInt(Vector3 color)
        {
            color = Vector3.Clamp(color, Vector3.Zero, new Vector3(1,1,1));
            color *= 255;
            return ((int)color.Z << 0) | ((int)color.Y << 8) | ((int)color.X << 16);
        }

        public static Vector3 GetColorVector3(int color)
        {
            Vector3 c = new Vector3(color >> 16 / 255, color >> 8 / 255, color / 255);
            return c;
        }
    }
}