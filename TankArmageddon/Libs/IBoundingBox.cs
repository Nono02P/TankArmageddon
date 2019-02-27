using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon
{
    public interface IBoundingBox : IEquatable<IBoundingBox>
    {
        #region Propriétés
        Point Location { get; set; }
        int Bottom { get; }
        int Top { get; }
        int Right { get; }
        int Left { get; }
        int Width { get; }
        int Height { get; }
        Point Size { get; }
        Point Center { get; }
        #endregion

        #region Méthodes
        bool Contains(int x, int y);
        bool Contains(Vector2 pLocation);
        bool Contains(Point pLocation);
        bool Intersects(IBoundingBox other);
        string ToString();
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        #endregion
    }
}