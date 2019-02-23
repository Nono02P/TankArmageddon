using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon
{
    public struct Circle : IBoundingBox
    {
        public enum eDrawMode
        {
            Fill,
            Line,
        }


        private eDrawMode _drawMode;

        public eDrawMode DrawMode
        {
            get { return _drawMode; }
            set
            {
                _drawMode = value;
                switch (value)
                {
                    case eDrawMode.Fill:
                        Thickness = Radius;
                        break;
                    default:
                        break;
                }
            }
        }
        public Point Location { get; set; }
        public float Radius { get; set; }
        public float Thickness { get; set; }
        public int Sides { get; set; }
        public Color Color { get; set; }
        public Point Size { get => new Point(Width, Height); }

        public int Bottom => (int)(Location.Y + Radius);

        public int Top => (int)(Location.Y - Radius);

        public int Right => (int)(Location.X + Radius);

        public int Left => (int)(Location.X - Radius);

        public int Width => (int)Radius * 2;

        public int Height => (int)Radius * 2;

        public Point Center => Location;

        public Circle(Point pLocation, float pRadius, int pSides, Color pColor, float pThickness = 1, eDrawMode pDrawMode = eDrawMode.Line)
        {
            Location = pLocation;
            Radius = pRadius;
            Sides = pSides;
            Color = pColor;
            _drawMode = pDrawMode;
            switch (_drawMode)
            {
                case eDrawMode.Fill:
                    Thickness = pRadius;
                    break;
                case eDrawMode.Line:
                    Thickness = pThickness;
                    break;
                default:
                    Thickness = pThickness;
                    break;
            }
        }

        #region Contains
        public bool Contains(Point point)
        {
            Point dist = Location - point;
            double distX = Math.Pow(Math.Abs(dist.X), 2);
            double distY = Math.Pow(Math.Abs(dist.Y), 2);
            double sumRadius = Math.Pow(Radius, 2);
            return distX + distY < sumRadius;
        }

        public bool Contains(int x, int y)
        {
            return Contains(new Point(x, y));
        }

        public bool Contains(Vector2 pLocation)
        {
            return Contains(pLocation.ToPoint());
        }
        #endregion

        #region Intersects
        public bool Intersects(Circle circle)
        {
            double distX = Math.Pow(Math.Abs((Location - circle.Location).X), 2);
            double distY = Math.Pow(Math.Abs((Location - circle.Location).Y), 2);
            double sumRadius = Math.Pow(Radius + circle.Radius, 2);
            return distX + distY < sumRadius;
        }

        public bool Intersects(Rectangle rectangle)
        {
            return  Location.X + Radius >= rectangle.Location.X && 
                    Location.X - Radius <= rectangle.Location.X + rectangle.Size.X && 
                    Location.Y + Radius >= rectangle.Location.Y && 
                    Location.Y - Radius <= rectangle.Location.Y + rectangle.Size.Y;
        }

        public bool Intersects(IBoundingBox other)
        {
            bool result = false;
            if (other is Circle)
            {
                Circle c = (Circle)other;
                result = Intersects(c);
            }
            if (other is RectangleBBox)
            {
                RectangleBBox r = (RectangleBBox)other;
                result = Intersects(r.Rectangle);
            }
            return result;
        }
        #endregion

        public bool Equals(IBoundingBox other)
        {
            bool result = false;
            if (other is Circle)
            {
                Circle c = (Circle)other;
                result = Location == c.Location &&
                        Radius == c.Radius;
            }
            return result;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawCircle(Location.ToVector2(), Radius, Sides, Color, Thickness);
        }
    }
}
