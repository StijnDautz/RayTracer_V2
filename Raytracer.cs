using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace template
{
    class Raytracer
    {
        private Scene _scene;
        private Camera _camera;
        private Surface _surface;
        private Point _lowRes, _highRes, _tempResolution;
        public static bool debugMode = true, _renderHigh = false;
        private int _rayCount = 0, REFLECTIONCAP = 3, AACAP = 1;

        public Raytracer(Scene scene, Camera camera, Surface surface)
        {
            _scene = scene;
            _camera = camera;
            _surface = surface;
            _lowRes = new Point(128, 128);
            _highRes = new Point(512, 512);
            _tempResolution = _highRes;
        }

        public void Render()
        {
            Vector3 color = Vector3.Zero;
            int c;
            float offset = 1f / AACAP;
            Point scale = new Point(_highRes.X / _tempResolution.X, _highRes.Y / _tempResolution.Y);
            for (int x = 0; x < _tempResolution.X; x++) //instead of 0 -> resolution.X
            {
                for (int y = 0; y < _tempResolution.Y; y++)
                {
                    //Anti alliasing, trace multiple rays with a constant small offset
                    for (int i = 0; i < AACAP; i++)
                    {
                        for (int j = 0; j < AACAP; j++)
                        {
                            //calculate the color of the ray
                            color += TraceRay(new VectorMath.Ray(_camera.Position, (_camera.Screen.ConvertToWorldCoords(new Vector2((x + i * offset) * scale.X, (y + j * offset)) * scale.Y) - _camera.Position)), 0);
                        }
                    }

                    //compute the average of all rays
                    c = VectorMath.GetColorInt(color / (AACAP * AACAP));

                    //TODO delete?
                    for (int xs = 0; xs < scale.X; xs++)
                    {
                        for (int ys = 0; ys < scale.Y; ys++)
                        {
                            _surface.pixels[x * scale.X + xs + (y * scale.Y + ys) * 1024] = c;
                        }
                    }
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
            //if this ray isn't recursively called more than maxReflection times trace this ray, 
            //else increase rayCount, as the reflections have all been drawn and a new primary ray is going to be shot, instead of a reflective ray
            if (reflectionNum < REFLECTIONCAP)
            {
                //Cast a ray from the camera through a point on the 2D screen and find the primitive in the world it hits first
                Intersection intersection = _scene.GetClosestIntersection(ray);

                //if there was an intersection, compute the color to return
                if (intersection != null)
                {
                    DrawDebugRays(intersection, ray, reflectionNum);
                    return ComputeColor(ray, intersection, reflectionNum);
                }
            }
            //increase rayCount, which is used for debugging, when a new PRIMARY ray is casted
            _rayCount++;
            return Vector3.Zero;
        }

        private Vector3 ComputeColor(VectorMath.Ray ray, Intersection intersection, int reflectionNum)
        {
            //DIFFUSE
            //calculate the color at the intersection
            Vector3 diffuseColor = _scene.GetIntersectionColor(intersection);

            Vector3 reflectionColor = Vector3.Zero;
            float reflectionIndex = intersection.primitive.Material.ReflectionIndex;

            //REFLECTION
            //if reflectionIndex > 0 calculate, compute the color
            if (_renderHigh)
            {
                if (reflectionIndex > 0)
                {
                    //calculate the reflectionRay 
                    Vector3 reflection = VectorMath.Reflect(ray.direction, intersection.Normal).Normalized();

                    //Trace the reflectionRay and increase reflectionNum, so we do not get stuck in an infinite recursive loop
                    //Also at a little offset to the intersectionPoint to prevent shadow acne
                    reflectionColor += TraceRay(new VectorMath.Ray(intersection.IntersectionPoint + 0.01f * reflection, reflection), ++reflectionNum);
                }
            }

            //FINAL COLOR
            return diffuseColor * (1 - reflectionIndex) + reflectionColor * reflectionIndex;
        }

        /*
         * DEBUG
         */
        private void DrawDebugRays(Intersection intersection, VectorMath.Ray ray, int reflectionNum)
        {
            if ((intersection != null && intersection.IntersectionPoint.Y == 0) ||  reflectionNum > 0)
            {
                DrawTracedRay(intersection, ray);
                if (intersection != null)
                { DrawShadowRays(intersection); }
            }
        }

        private void DrawTracedRay(Intersection intersection, VectorMath.Ray ray)
        {
            _surface.DrawRay(ray, _camera.Screen, intersection == null ? ray.magnitude * 10 : intersection.Distance, new Vector3(1, 0, 0));
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

        /*
         * INPUT HANDLING
         */
        int _primitiveWatching;
         public void HandleInput()
        {
            _tempResolution = _highRes;
            if (Game.inputManager.ButtonDown(Key.Left) || Game.inputManager.ButtonDown(Key.A))
            { _camera.Move(new Vector3(-1, 0, 0)); _tempResolution = _lowRes; }
            else if(Game.inputManager.ButtonDown(Key.Right) || Game.inputManager.ButtonDown(Key.D))
            { _camera.Move(new Vector3(1, 0, 0)); _tempResolution = _lowRes; }
            if (Game.inputManager.ButtonDown(Key.Up) || Game.inputManager.ButtonDown(Key.W))
            { _camera.Move(new Vector3(0, 1, 0)); _tempResolution = _lowRes; }
            else if(Game.inputManager.ButtonDown(Key.Down) || Game.inputManager.ButtonDown(Key.S))
            { _camera.Move(new Vector3(0, -1, 0)); _tempResolution = _lowRes; }
            if(Game.inputManager.ButtonDown(Key.Z))
            { _camera.Move(new Vector3(0, 0, 1)); _tempResolution = _lowRes; }
            else if(Game.inputManager.ButtonDown(Key.X))
            { _camera.Move(new Vector3(0, 0, -1)); _tempResolution = _lowRes; }
            if(Game.inputManager.ButtonDown(Key.E))
            {
                if(++_primitiveWatching > _scene.Primitives.Count - 1)
                { _primitiveWatching = 0; }
                _camera.LookAt(_scene.Primitives[_primitiveWatching].Position);
                _tempResolution = _lowRes;
            }
            else if(Game.inputManager.ButtonDown(Key.Q))
            {
                if(--_primitiveWatching < 0)
                { _primitiveWatching = _scene.Primitives.Count - 1; }
                _camera.LookAt(_scene.Primitives[_primitiveWatching].Position);
                _tempResolution = _lowRes;
            }
            if(Game.inputManager.ButtonDown(Key.H))
            { _renderHigh = true; }
            if (Game.inputManager.ButtonDown(Key.L))
            { _renderHigh = false; }
        }
    }
}