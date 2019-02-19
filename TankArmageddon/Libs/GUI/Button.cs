using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon.GUI
{
    public class Button : Element, IIntegrableMenu
    {
        #region Propriétés
        /// <summary>
        /// Image utilisé par défaut (sans survol ni click)
        /// </summary>
        public Texture2D ImageDefault { get; protected set; } = null;
        public Rectangle? ImageBoxDefault { get; set; } = null;
        /// <summary>
        /// Image utilisé en cas de survol
        /// </summary>
        public Texture2D ImageHover { get; protected set; } = null;
        public Rectangle? ImageBoxHover { get; set; } = null;
        /// <summary>
        /// Image utilisé en cas de click
        /// </summary>
        public Texture2D ImagePressed { get; protected set; } = null;
        public Rectangle? ImageBoxPressed { get; set; } = null;
        /// <summary>
        /// Image utilisé par défaut (sans survol ni click)
        /// </summary>
        public Texture2D ImageSelected { get; protected set; } = null;
        public Rectangle? ImageBoxSelected { get; set; } = null;
        /// <summary>
        /// Objet TextBox intégré au bouton.
        /// </summary>
        public Textbox TextBox { get; private set; }
        public bool Selected { get; set; }
        public Color Color_Selected { get => TextBox.Color_Selected; set { if (TextBox != null) TextBox.Color_Selected = value; }  }
        public Color ColorBck_Selected { get => TextBox.ColorBck_Selected; set { if (TextBox != null) TextBox.ColorBck_Selected = value; } }
        public Color Color_Default { get => TextBox.Color_Default; set { if (TextBox != null) TextBox.Color_Default = value; } }
        public Color ColorBck_Default { get => TextBox.ColorBck_Default; set { if (TextBox != null) TextBox.ColorBck_Default = value; } }
        #endregion

        #region Constructeur
        public Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible, Texture2D pImageDefault, Texture2D pImageHover, Texture2D pImagePressed, SpriteFont pFont, string pText)
            : base(pPosition, pOrigin, new Vector2(pImageDefault.Width, pImageDefault.Height), pVisible, pScale)
        {
            ImageDefault = pImageDefault;
            ImageHover = pImageHover;
            ImagePressed = pImagePressed;
            TextBox = new Textbox(pPosition, Vector2.Zero, pFont, pText, false, pVisible);
            AlignText(HAlign.Center, VAlign.Middle);
        }

        public Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible, Texture2D pImageDefault, Texture2D pImageHover, Texture2D pImagePressed)
            : base(pPosition, pOrigin, new Vector2(pImageDefault.Width, pImageDefault.Height), pVisible, pScale)
        {
            ImageDefault = pImageDefault;
            ImageHover = pImageHover;
            ImagePressed = pImagePressed;
        }

        protected Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible) : base(pPosition, pOrigin, Vector2.Zero, pVisible, pScale) { }

        protected Button(Vector2 pPosition, Vector2 pOrigin, float pScale, bool pVisible, SpriteFont pFont, string pText) : base(pPosition, pOrigin, Vector2.Zero, pVisible, pScale)
        {
            TextBox = new Textbox(pPosition, Vector2.Zero, pFont, pText, false, pVisible);
            AlignText(HAlign.Center, VAlign.Middle);
        }
        #endregion

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
            if (TextBox != null)
                TextBox.Update(gameTime);
            base.Update(gameTime);
        }
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (Visible)
            {
                // Détermine l'image à afficher (Par défaut ---> Cliqué --> Survolé --> Sélectionné)
                Texture2D img = ImageDefault;
                Rectangle? imgBox = ImageBoxDefault;
                if (Clicked && ImagePressed != null)
                {
                    img = ImagePressed;
                    imgBox = ImageBoxPressed;
                }
                else if (Hover && ImageHover != null)
                {
                    img = ImageHover;
                    imgBox = ImageBoxHover;
                }
                else if (Selected && ImageSelected != null)
                {
                    img = ImageSelected;
                    imgBox = ImageBoxSelected;
                }
                spriteBatch.Draw(img, Position, imgBox, Color.White, Angle, Origin, Scale, SpriteEffects.None, 0);
                if (TextBox != null)
                    TextBox.Draw(spriteBatch, gameTime);
            }
        }
        #endregion
    }
}
