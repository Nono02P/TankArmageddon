using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon.GUI
{
    public class Image : Element
    {
        #region Propriétés
        public Texture2D Texture { get; set; }
        #endregion

        #region Constructeur
        public Image(Texture2D pTexture, Vector2 pPosition, Vector2 pOrigin, bool pVisible = true, float pScale = 1) : base(pPosition, pOrigin, new Vector2(pTexture.Width, pTexture.Height), pVisible, pScale)
        {
            Texture = pTexture;
        }

        public Image(Texture2D pTexture, Vector2 pPosition, bool pVisible = true, float pScale = 1) : base(pPosition, Vector2.Zero, new Vector2(pTexture.Width, pTexture.Height), pVisible, pScale)
        {
            Texture = pTexture;
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Angle, Origin, Scale, SpriteEffects.None, 0);
            base.Draw(spriteBatch, gameTime);
        }
        #endregion
    }
}
