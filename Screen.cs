using OpenTK;
using System.Drawing;

namespace template
{
    public class Screen : Object
    {
        private float _scale = 512 / 10; //512x512 pixels in a 10x10 box
        private Point _resolution;
        private Vector3 _normal;
        private Rectangle _dimensions;
        Vector3[] corners = new Vector3[4];

        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }
        public Point Resolution
        {
            get { return _resolution; }
        }

        private Vector3 TopLeft
        {
            get { return corners[0]; }
            set { corners[0] = value; }
        }

        private Vector3 TopRight
        {
            get { return corners[1]; }
            set { corners[1] = value; }

        }

        private Vector3 BottomLeft
        {
            get { return corners[2]; }
            set { corners[2] = value; }
        }

        private Vector3 BottomRight
        {
            get { return corners[3]; }
            set { corners[3] = value; }
        }

        public Screen(Vector3 position, Vector3 normal, Point resolution, Point dimensions) : base(position)
        {
            _resolution = resolution;
            _dimensions = new Rectangle((int)(-dimensions.X / 2 + position.X), (int)(dimensions.Y / 2 + position.Z), dimensions.X, dimensions.Y);
            _normal = normal;
            TopLeft = new Vector3(position.X - dimensions.X / 2, position.Y + dimensions.Y / 2, position.Z);
            TopRight = new Vector3(position.X + dimensions.X / 2, position.Y + dimensions.Y / 2, position.Z);
            BottomLeft = new Vector3(position.X - dimensions.X / 2, position.Y - dimensions.Y / 2, position.Z);
            BottomRight = new Vector3(position.X + dimensions.X / 2, position.Y - dimensions.Y / 2, position.Z);
        }

        public Vector3 ConvertToWorldCoords(Vector2 p)
        {
            //given a positional vector v1 and two directional vectors v2 and v3, which determine a plane, any point on this plane can be defined by v1 + t1 * v2 + t2 * v3
            return TopLeft + ((TopRight - TopLeft) / _resolution.X) * p.X + ((BottomLeft - TopLeft) / _resolution.Y) * p.Y;
        }

        //TODO make world dimension variable
        public Point ConvertToScreenCoords(Vector3 v)
        {
            int x = (int)((_dimensions.Right + v.X) * (_resolution.X / 10));
            int y = (int)((_dimensions.Top - v.Z) * (_resolution.Y / 10));
            return new Point(x, y);
        }
    }
}
