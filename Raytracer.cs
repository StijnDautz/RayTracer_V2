using OpenTK;

namespace template
{
    class Raytracer
    {
        private Scene _scene;
        private Camera _camera;
        private Surface _surface;
        private bool debugMode = false;
        private int _rayCount = 0; //So that not EVERY debug ray gets drawn
        private int REFLECTIONCAP = 3;
        private int GLOSSCAP = 4;
        private int AACAP = 1;

        public Raytracer(Scene scene, Camera camera, Surface surface)
        {
            _scene = scene;
            _camera = camera;
            _surface = surface;
        }

        public void Render()
        {
            Vector3 color = Vector3.Zero;
            float offset = 1f / AACAP;
            for (int x = 0; x < 512; x++) //instead of 0 -> resolution.X
            {
                for (int y = 0; y < 512; y++)
                {
                    //Anti alliasing, trace multiple rays with a constant small offset
                    for (int i = 0; i < AACAP; i++)
                    {
                        for(int j = 0; j < AACAP; j++)
                        {
                            //calculate the color of the ray
                            color += TraceRay(new VectorMath.Ray(_camera.Position, (_camera.Screen.ConvertToWorldCoords(new Vector2(x + i * offset, y + j * offset)) - _camera.Position)), 0);
                        }
                    }
                    //compute the average of all rays
                    _surface.pixels[x + y * 1024] = VectorMath.GetColorInt(color / (AACAP * AACAP));

                    //reset color to Vector3.Zero
                    color = Vector3.Zero;
                }
            }

            //draw the primivites in the scene over the casted rays
            if (debugMode)
            { _surface.DrawPrimitives(_scene.Primitives, _scene.Lights, _camera.Screen); }
        }

        private Vector3 TraceRay(VectorMath.Ray ray, int reflectionNum)
        {
            Vector3 color = Vector3.Zero;

            //if this ray isn't recursively called more than maxReflection times trace this ray, 
            //else increase rayCount, as the reflections have all been drawn and a new primary ray is going to be shot, instead of a reflective ray
            if (reflectionNum < REFLECTIONCAP)
            {
                //Cast a ray from the camera through a point on the 2D screen and find the primitive in the world it hits first
                Intersection intersection = _scene.GetClosestIntersection(ray);

                DrawDebugRays(intersection, ray);

                //if there was an intersection, compute the color to return
                if (intersection != null)
                {
                    //DIFFUSE
                    //calculate the color at the intersection
                    color = _scene.GetIntersectionColor(intersection);

                    //REFLECTION
                    //if reflectionIndex > 0 calculate, compute the color
                    Vector3 reflectionColor = Vector3.Zero;
                    float reflectionIndex = intersection.primitive.Material.ReflectionIndex;
                    if (reflectionIndex > 0)
                    {
                        //calculate the reflectionRay 
                        Vector3 reflection = VectorMath.Reflect(ray.direction, intersection.Normal).Normalized();

                        //Trace the reflectionRay and increase reflectionNum, so we do not get stuck in an infinite recursive loop
                        //Also at a little offset to the intersectionPoint to prevent shadow acne
                        for(int i = 0; i < GLOSSCAP; i++)
                        {
                            for(int j = 0; j < GLOSSCAP; j++)
                            {
                                reflectionColor += TraceRay(new VectorMath.Ray(intersection.IntersectionPoint + 0.01f * reflection, reflection), ++reflectionNum);
                            }
                        }
                    }

                    //FINAL COLOR
                    color = color * (1 - reflectionIndex) + reflectionColor * reflectionIndex; 
                }
            }
            //increase rayCount, which is used for debugging, when a new PRIMARY ray is casted
            _rayCount++;
            return color;
        }
        /*
         * DEBUG
         */
        private void DrawDebugRays(Intersection intersection, VectorMath.Ray ray)
        {
            if (_rayCount % 1000 == 0)
            {
                DrawTracedRay(intersection, ray);
                if (intersection != null)
                { DrawShadowRays(intersection); }
            }
        }

        private void DrawTracedRay(Intersection intersection, VectorMath.Ray ray)
        {
            _surface.DrawRay(ray, _camera.Screen, intersection == null ? ray.magnitude : intersection.Distance, new Vector3(1, 0, 0));
        }

        private void DrawShadowRays(Intersection intersection)
        {
            VectorMath.Ray shadowRay;
            foreach (Light l in _scene.Lights)
            {
                //draw a white shadowray to the intersection if there is a shadow, else draw a pink ray with a large magnitude
                shadowRay = new VectorMath.Ray(intersection.IntersectionPoint, l.Position - intersection.IntersectionPoint);
                _surface.DrawRay(shadowRay, _camera.Screen, shadowRay.magnitude, l.InShadow(intersection, l.Position, _scene) ? new Vector3(1, 1, 1) : new Vector3(0, 1, 1));
            }
        }
    }
}