using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace TankArmageddon.GUI
{
    // TODO Gérer des évènements sur changement de taille, afin d'affecter l'origine dans le button.

    public class Textbox : Element, IIntegrableMenu
    {
        #region Variables privées
        private SpriteFont _font;
        private string _strText = "";
        private StringBuilder _strBderText;
        private Vector2 _usedArea = Vector2.One;
        private bool _useStringBuilder = false;
        #endregion

        #region Propriétés

        public bool Selected { get; set; }

        #region Police et texte
        /// <summary>
        /// Police de caractères à utiliser.
        /// </summary>
        public SpriteFont Font { get { return _font; } set { _font = value; RefreshSize(); } }

        /// <summary>
        /// Texte affiché.
        /// </summary>
        public string Text
        {
            get
            {
                // Retourne le texte en fonction du format utilisé (StringBuilder ou String normal)
                if (UseStringBuilder)
                {
                    return _strBderText.ToString();
                }
                else
                {
                    return _strText;
                }
            }
            set
            {
                if (UseStringBuilder)
                {
                    // _strBderText. = value;
                }
                else
                {
                    _strText = value;
                }
                RefreshSize();
            }
        }

        /// <summary>
        /// Permet de basculer en mode StringBuilder (à utiliser quand le texte est long et/ou d'une longueur qui va varier souvent).
        /// </summary>
        public bool UseStringBuilder
        {
            get { return _useStringBuilder; }
            set
            {
                // Au changement de mode du text (String ou StringBuilder), réécrit le précédent texte dans le nouveau format.
                if (_useStringBuilder)
                {
                    _strText = Text;
                }
                else
                {
                    _strBderText = new StringBuilder(Text);
                }
                _useStringBuilder = value;
            }
        }
        #endregion

        #region Position et taille
        /// <summary>
        /// Sert à redéfinir la taille utile afin de supprimer la zone morte que peuvent avoir certaines polices.
        /// Par défaut : 1.0f = 100% de la taille en pixels utilisé.
        /// </summary>
        public Vector2 UsedArea { get { return _usedArea; } set { _usedArea = value; RefreshSize(); } }

        /// <summary>
        /// Taille de la police en pixels x UsedArea (pour redéfinir la zone morte en %)
        /// </summary>
        public new Vector2 Size { get => base.Size; private set { base.Size = value; RefreshBoundingBox(); } }

        /// <summary>
        /// Position du texte
        /// </summary>
        public new Vector2 Position { get { return _position + (Size * (Vector2.One - UsedArea)) / 2; } set { _position = value - (Size * (Vector2.One - UsedArea)) / 2; RefreshBoundingBox(); } }

        /// <summary>
        /// Origine du texte
        /// </summary>
        public new Vector2 Origin {
            get
            {
                if (Equals(_origin, Vector2.Zero))
                {
                    return _origin;
                }else
                {
                    return _origin + (Size * (Vector2.One - UsedArea)) / 2;
                }
            }
            set
            {
                if (Equals(value, Vector2.Zero))
                {
                    _origin = Vector2.Zero;
                }else
                {
                    _origin = value + (Size * (Vector2.One - UsedArea)) / 2; RefreshBoundingBox();
                }
            } }

        /// <summary>
        /// Décalage du texte au second plan par rapport à celui du premier plan (par défaut 1 px en X et en Y)
        /// </summary>
        public Vector2 PositionBck { get; set; } = Vector2.One;
        #endregion

        #region Couleurs
        /// <summary>
        /// Couleur du texte au premier plan sans clic ou survol de souris (par défaut noir).
        /// </summary>
        public Color Color_Default { get; set; } = Color.White;
        /// <summary>
        /// Couleur du texte au second plan sans clic ou survol de souris (par défaut blanc).
        /// </summary>
        public Color ColorBck_Default { get; set; } = Color.Black;
        /// <summary>
        /// Couleur du texte au premier plan en cas de survol de souris (par défaut noir).
        /// </summary>
        public Color Color_Hover { get; set; } = Color.White;
        /// <summary>
        /// Couleur du texte au second plan en cas de survol de souris (par défaut blanc).
        /// </summary>
        public Color ColorBck_Hover { get; set; } = Color.Black;
        /// <summary>
        /// Couleur du texte au premier plan en cas de sélection dans un menu (par défaut noir).
        /// </summary>
        public Color Color_Selected { get; set; } = Color.White;
        /// <summary>
        /// Couleur du texte au second plan en cas de sélection dans un menu (par défaut blanc).
        /// </summary>
        public Color ColorBck_Selected { get; set; } = Color.Black;
        /// <summary>
        /// Couleur du texte au premier plan en cas de clic (par défaut noir).
        /// </summary>
        public Color Color_Clicked { get; set; } = Color.White;
        /// <summary>
        /// Couleur du texte au second plan en cas de clic (par défaut blanc).
        /// </summary>
        public Color ColorBck_Clicked { get; set; } = Color.Black;
        #endregion

        #endregion

        #region Constructeur
        public Textbox(Vector2 pPosition, Vector2 pOrigin, SpriteFont pFont, string pText, bool pShowRectangle = false, bool pVisible = true) 
            : base(pPosition, pOrigin, pFont.MeasureString(pText), pVisible)
        {
            Position = pPosition;
            Font = pFont;
            Text = pText;
            Origin = pOrigin;
        }
        public Textbox(Vector2 pPosition, SpriteFont pFont, string pText) 
            : base(pPosition, new Vector2(0), pFont.MeasureString(pText), true)
        {
            Position = pPosition;
            Font = pFont;
            Text = pText;
        }
        #endregion

        #region Méthodes

        /// <summary>
        /// Recalcule la taille du texte (en prenant en compte la "zone morte" définie dans UsedArea)
        /// </summary>
        private void RefreshSize()
        {
            if (Font != null)
                Size = Font.MeasureString(Text) * UsedArea;
            RefreshBoundingBox();
        }

        /// <summary>
        /// Raffraichit la BoundingBox (en prenant en compte la "zone morte" définie dans UsedArea)
        /// </summary>
        public override void RefreshBoundingBox()
        {
            Vector2 location = Position - Origin + Size * (Vector2.One - UsedArea);
            BoundingBox = new RectangleBBox(location.ToPoint(), Size.ToPoint());
        }

        /// <summary>
        /// Permet d'insérer une chaine de caractères à l'emplacement pointé en startIndex.
        /// </summary>
        /// <param name="startIndex">Emplacement où insérer la chaine de caractères</param>
        /// <param name="value">Chaine de caractères à insérer</param>
        /// <returns></returns>
        public string Insert(int startIndex, string value)
        {
            if (UseStringBuilder)
            {
                _strBderText.Insert(startIndex, value);
            }
            else
            {
                _strText.Insert(startIndex, value);
            }
            return Text;
        }

        public void ApplyColor(Color pFrontColor, Color pBackColor)
        {
            Color_Clicked = pFrontColor;
            Color_Default = pFrontColor;
            Color_Hover = pFrontColor;
            Color_Selected = pFrontColor;

            ColorBck_Clicked = pBackColor;
            ColorBck_Default = pBackColor;
            ColorBck_Hover = pBackColor;
            ColorBck_Selected = pBackColor;
        }

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (Visible)
            {
                // Détermine les couleurs à afficher (Par défaut ---> Cliqué --> Selectionné --> Survolé)
                Color colorBck = ColorBck_Default;
                Color color = Color_Default;
                if (Clicked)
                {
                    colorBck = ColorBck_Clicked;
                    color = Color_Clicked;
                }
                else if (Selected)
                {
                    colorBck = ColorBck_Selected;
                    color = Color_Selected;
                }
                else if (Hover)
                {
                    colorBck = ColorBck_Hover;
                    color = Color_Hover;
                }
                MainGame.spriteBatch.DrawString(Font, Text, Position + PositionBck, colorBck, Angle, Origin, Scale, SpriteEffects.None, 1);
                MainGame.spriteBatch.DrawString(Font, Text, Position, color, Angle, Origin, Scale, SpriteEffects.None, 0);
            }
        }
        #endregion
        
        #endregion
    }
}