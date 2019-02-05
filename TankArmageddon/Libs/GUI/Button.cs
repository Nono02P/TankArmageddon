using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon.GUI
{
    public class Button : Element
    {
        #region Propriétés
        /// <summary>
        /// Image utilisé par défaut (sans survol ni click)
        /// </summary>
        public Texture2D ImageDefault { get; private set; } = null;
        /// <summary>
        /// Image utilisé en cas de survol
        /// </summary>
        public Texture2D ImageHover { get; private set; } = null;
        /// <summary>
        /// Image utilisé en cas de click
        /// </summary>
        public Texture2D ImagePressed { get; private set; } = null;
        /// <summary>
        /// Objet TextBox intégré au bouton.
        /// </summary>
        public Textbox TextBox { get; private set; }
        #endregion

        #region Constructeur
        public Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible, Texture2D pImageDefault, Texture2D pImageHover, Texture2D pImagePressed, SpriteFont pFont, string pText)
            : base(pPosition, pOrigin, new Vector2(pImageDefault.Width, pImageDefault.Height), pVisible, pScale)
        {
            Scale = pScale;
            ImageDefault = pImageDefault;
            ImageHover = pImageHover;
            ImagePressed = pImagePressed;
            TextBox = new Textbox(pPosition, Vector2.Zero, pFont, pText, false, pVisible);
            AlignText(HAlign.Center, VAlign.Middle);
        }

        public Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible, Texture2D pImageDefault, Texture2D pImageHover, Texture2D pImagePressed)
            : base(pPosition, pOrigin, new Vector2(pImageDefault.Width, pImageDefault.Height), pVisible, pScale)
        {
            Scale = pScale;
            ImageDefault = pImageDefault;
            ImageHover = pImageHover;
            ImagePressed = pImagePressed;
        }
        #endregion

        #region Méthodes

        #region Alignement de texte
        public void AlignText(HAlign pHAlign, VAlign pVAlign)
        {
            float x = 0;
            float y = 0;
            switch (pHAlign)
            {
                case HAlign.Left:
                    x = Position.X - Origin.X + TextBox.Origin.X;
                    break;
                case HAlign.Center:
                    x = Position.X - Origin.X + TextBox.Origin.X + (Size.X - TextBox.Size.X) / 2;
                    break;
                case HAlign.Right:
                    x = Position.X - Origin.X + TextBox.Origin.X + Size.X - TextBox.Size.X;
                    break;
                default:
                    break;
            }

            switch (pVAlign)
            {
                case VAlign.Top:
                    y = Position.Y - Origin.Y - TextBox.Origin.Y;
                    break;
                case VAlign.Middle:
                    y = Position.Y - Origin.Y + TextBox.Origin.Y + (Size.Y - TextBox.Size.Y) / 2;
                    break;
                case VAlign.Bottom:
                    y = Position.Y - Origin.Y + TextBox.Origin.Y + Size.Y - TextBox.Size.Y;
                    break;
                default:
                    break;
            }
            TextBox.Position = new Vector2(x, y);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            TextBox.Update(gameTime);
            base.Update(gameTime);
        }
        #endregion


        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            // Détermine l'image à afficher (Par défaut ---> Cliqué --> Survolé)
            Texture2D img = ImageDefault;
            if (Clicked && ImagePressed != null)
            {
                img = ImagePressed;
            }
            else if (Hover && ImageHover != null)
            {
                img = ImageHover;
            }
            spriteBatch.Draw(img, Position, null, Color.White, Angle, Origin, Scale, SpriteEffects.None, 0);
            TextBox.Draw(spriteBatch, gameTime);
        }
        #endregion

        #endregion
    }
}
