using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class RectangleBBox : IBoundingBox
    {
        #region Champs
        public Rectangle Rectangle;
        #endregion

        #region Propriétés
        public Point Location { get => Rectangle.Location; set => Rectangle.Location = value; }
        public Point Size { get => Rectangle.Size; set => Rectangle.Size = value; }

        public int Bottom => Rectangle.Bottom;

        public int Top => Rectangle.Top;

        public int Right => Rectangle.Right;

        public int Left => Rectangle.Left;

        public int Width => Rectangle.Width;

        public int Height => Rectangle.Height;

        public Point Center => Rectangle.Center;
        #endregion

        #region Constructeur
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
        #endregion

        #region Contains
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
        #endregion

        #region Equals
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
        #endregion

        #region Intersects
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
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawRectangle(Rectangle, Color.Red, 2);
        }
        #endregion
    }
}