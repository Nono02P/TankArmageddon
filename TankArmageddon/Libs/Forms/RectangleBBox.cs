using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankArmageddon
{
    public class RectangleBBox : IBoundingBox
    {
        public Rectangle Rectangle;

        public Point Location { get => Rectangle.Location; set => Rectangle.Location = value; }
        public Point Size { get => Rectangle.Size; set => Rectangle.Size = value; }

        public int Bottom => Rectangle.Bottom;

        public int Top => Rectangle.Top;

        public int Right => Rectangle.Right;

        public int Left => Rectangle.Left;

        public int Width => Rectangle.Width;

        public int Height => Rectangle.Height;

        public Point Center => Rectangle.Center;

        public RectangleBBox()
        {
            Rectangle = new Rectangle();
        }

        public RectangleBBox(Point location, Point size)
        {
            Rectangle = new Rectangle(location, size);
        }

        public RectangleBBox(int x, int y, int width, int height)
        {
            Rectangle = new Rectangle(x, y, width, height);
        }
        

        public bool Contains(int x, int y)
        {
            return Rectangle.Contains(x, y);
        }

        public bool Contains(Vector2 pLocation)
        {
            return Rectangle.Contains(pLocation);
        }

        public bool Contains(Point pLocation)
        {
            return Rectangle.Contains(pLocation);
        }

        public bool Equals(IBoundingBox other)
        {
            bool result = false;
            if (other is RectangleBBox)
            {
                RectangleBBox r = (RectangleBBox)other;
                result = Rectangle.Contains(r.Rectangle);
            }
            return result;
        }

        public bool Intersects(IBoundingBox other)
        {
            bool result = false;
            if (other is RectangleBBox)
            {
                RectangleBBox r = (RectangleBBox)other;
                result = Rectangle.Intersects(r.Rectangle);
            }
            else
            {
                result = other.Intersects(this);
            }
            return result;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawRectangle(Rectangle, Color.Red, 2);
        }
    }
}
