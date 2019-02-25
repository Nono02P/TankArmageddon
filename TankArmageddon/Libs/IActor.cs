using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public interface IActor
    {
        #region Propriétés
        Vector2 Position { get; }
        IBoundingBox BoundingBox { get; }
        bool Remove { get; set; }
        #endregion

        #region Méthodes
        void RefreshBoundingBox();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime);
        #endregion
    }
}
