using OpenTK;
using System;
using System.Drawing;

namespace template {

    class Game
    {
	    // member variables
	    public Surface surface;
        private Raytracer _raytracer;
        public static Random rnd;

	    // initialize
	    public void Init()
	    {
            rnd = new Random();

            Material mirror = new Material(new Vector3(1, 1, 1), 1, false);
            Material wall = new Material(new Vector3(1, 1f, 1f), 0, false);
            Material red = new Material(new Vector3(1, 0.8f, 0.9f), 0.2f, false);
            Material textured = new Material(Vector3.Zero, 0, true);

            Scene scene = new Scene();
            scene.AddPrimitive(new Sphere(new Vector3(0, 1, 0.5f), 1, new Vector3(1.0f, 0.5f, 0.5f), red));      
            //scene.AddPrimitive(new Sphere(new Vector3(4, 0, 2), 1, new Vector3(0.5f, 1.0f, 0.5f), mirror));
            //scene.AddPrimitive(new Sphere(new Vector3(-1, 0, 1), 1, new Vector3(0.5f, 0.5f, 1.0f), mirror));
            scene.AddPrimitive(new Plane(new Vector3(0, -0.9f, 0), new Vector3(0, 1, 0), new Vector3(1f, 1f, 1f), mirror));
            scene.AddPrimitive(new Plane(new Vector3(0, 5, 0), new Vector3(0, -1, 0), new Vector3(1f, 1f, 1f), wall));
            scene.AddPrimitive(new Plane(new Vector3(-4, 0, 0), new Vector3(1, 0, 0), new Vector3(1f, 1f, 1f), textured));
            scene.AddPrimitive(new Plane(new Vector3(6, 0, 0), new Vector3(-1, 0, 0), new Vector3(1f, 1f, 1f), wall));
            scene.AddPrimitive(new Plane(new Vector3(0, 0, 5f), new Vector3(0, 0, -1), new Vector3(2f, 2f, 2f), wall));
            //scene.AddLight(new Light(new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f), new Vector3(3f,3f,3f)));
            //scene.AddLight(new Light(new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f)));
            scene.AddLight(new AreaLight(new Vector3(-2, 1, -2), new Vector3(1, 1, 1), new Vector3(1f, 1f, 1f), new Vector3(-1, 0, 0), new Vector3(0, 2, 0.5f), new Point(1, 1)));

            Screen scr = new Screen(new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Point(512, 512), new Point(8, 8));
            Camera camera = new Camera(new Vector3(0, 0, -2), new Vector3(0, 0, 1), scr);
            _raytracer = new Raytracer(scene, camera, surface);
        }

	    // tick: renders one frame
	    public void Tick()
	    {
		    surface.Clear( 0 );
            _raytracer.Render();
	    }
    }
}