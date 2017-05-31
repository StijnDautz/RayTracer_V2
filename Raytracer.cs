using OpenTK;
using System.Drawing;

namespace template
{
    class Raytracer
    {
        private Scene _scene;
        private Camera _camera;
        private Surface _surface;
        private int _rayCount = 0; //So that not EVERY debug ray gets drawn
        private int maxReflection = 1;
        private int antialliasingRayCap = 1;

        public Raytracer(Scene scene, Camera camera, Surface surface)
        {
            _scene = scene;
            _camera = camera;
            _surface = surface;
        }

        public void Render()
        {
            Vector3 color = Vector3.Zero;
            float offset = 1f / antialliasingRayCap;
            for (int x = 0; x < 512; x++) //instead of 0 -> resolution.X
            {
                for (int y = 0; y < 512; y++)
                {
                    for (int i = 0; i < antialliasingRayCap; i++)
                    {
                        for(int j = 0; j < antialliasingRayCap; j++)
                        {
                            color += TraceRay(new VectorMath.Ray(_camera.Position, (_camera.Screen.ConvertToWorldCoords(new Vector2(x + i * offset, y + j * offset)) - _camera.Position)), 0);
                        }
                    }
                    _surface.pixels[x + y * 1024] = VectorMath.GetColorInt(color / (antialliasingRayCap * antialliasingRayCap));
                    color = Vector3.Zero;
                }
            }

            //draw the primivites in the scene over the casted rays
            _surface.DrawPrimitives(_scene.Primitives, _scene.Lights, _camera.Screen);
        }

        private Vector3 TraceRay(VectorMath.Ray ray, int reflectionNum)
        {
            Vector3 color = Vector3.Zero;

            //if this ray isn't recursively called more than maxReflection times trace this ray, 
            //else increase rayCount, as the reflections have all been drawn and a new primary ray is going to be shot, instead of a reflective ray
            if (reflectionNum < maxReflection)
            {
                //Cast a ray from the camera through a point on the 2D screen and find the primitive in the world it hits first
                Intersection intersection = _scene.GetClosestIntersection(ray);

                DrawDebugRays(intersection, ray);

                //if there was an intersection, compute the color to return
                if (intersection != null)
                {
                    //calculate the color at the intersection
                    color = _scene.GetIntersectionColor(intersection);

                    //increase reflectionNum, so we do not get stuck in an infinite recursive loop
                    reflectionNum++;

                    //trace the reflective ray
                    Vector3 reflection = VectorMath.Reflect(ray.direction, intersection.Normal);
                    if (intersection.primitive.IsReflective)
                    { color += TraceRay(new VectorMath.Ray(intersection.IntersectionPoint, reflection), reflectionNum); }
                }
            }
            _rayCount++;
            return color;
        }

        /*
         * DEBUG
         */
        private void DrawDebugRays(Intersection intersection, VectorMath.Ray ray)
        {
            if (intersection != null && intersection.IntersectionPoint.Y == 0 && _rayCount % 1000 == 0)
            {
                DrawTracedRay(intersection, ray);
                if (intersection != null)
                { DrawShadowRays(intersection); }
            }
        }

        private void DrawTracedRay(Intersection intersection, VectorMath.Ray ray)
        {
            float drawLength = intersection == null ? ray.magnitude : intersection.Distance;
            _surface.DrawRay(ray, _camera.Screen, drawLength, new Vector3(1, 0, 0));
        }

        private void DrawShadowRays(Intersection intersection)
        {
            VectorMath.Ray shadowRay;
            Vector3 color;
            foreach (Light l in _scene.Lights)
            {
                bool inShadow = l.InShadow(intersection, l.Position, _scene);
                color = inShadow ? new Vector3(1, 1, 1) : new Vector3(0, 1, 1);
                shadowRay = new VectorMath.Ray(intersection.IntersectionPoint, l.Position - intersection.IntersectionPoint);
                _surface.DrawRay(shadowRay, _camera.Screen, shadowRay.magnitude, color);
            }
        }
    }
}