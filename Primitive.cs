using OpenTK;
using System.Drawing;

namespace template
{
    public abstract class Primitive : Object
    {
        Material _material;
        private float _refractionIndex;

        public Material Material
        {
            get { return _material; }
        }

        public bool IsReflective
        {
            get { return _material.IsReflective; }
        }

        public float RefractionIndex
        {
            get { return _refractionIndex; }
            set { _refractionIndex = value; }
        }

        public Primitive(Vector3 position, Vector3 color, Material material) : base(position)
        {
            _material = material;
        }

        public virtual Intersection GetIntersection(VectorMath.Ray ray)
        {
            return null;
        }

        public virtual Point GenerateUV(Vector3 worldCoords)
        {
            return new Point(0, 0);
        }

        public Vector3 ComputeColor(Intersection intersection, Light l, Scene scene)
        {
            //convert the 3d intersectionpoint to a 2d point on the texture of the material
            return Material.ComputeColor(l.ComputeIllumination(intersection, scene), intersection.IntersectionPoint);
        }
    }
}
