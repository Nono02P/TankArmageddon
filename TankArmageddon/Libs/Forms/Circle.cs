using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon
{
    public struct Circle //: IBoundingBox
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
        public Vector2 Location { get; set; }
        public float Radius { get; set; }
        public float Thickness { get; set; }
        public int Sides { get; set; }
        public Color Color { get; set; }

        public Circle(Vector2 pLocation, float pRadius, int pSides, Color pColor, float pThickness = 1, eDrawMode pDrawMode = eDrawMode.Line)
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

        public bool Contains(Point point)
        {
            Vector2 dist = Location - point.ToVector2();
            double distX = Math.Pow(Math.Abs(dist.X), 2);
            double distY = Math.Pow(Math.Abs(dist.Y), 2);
            double sumRadius = Math.Pow(Radius, 2);
            return distX + distY < sumRadius;
        }

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

        public void Draw(GameTime gameTime)
        {
            MainGame.spriteBatch.DrawCircle(Location, Radius, Sides, Color, Thickness);
        }
    }
}
