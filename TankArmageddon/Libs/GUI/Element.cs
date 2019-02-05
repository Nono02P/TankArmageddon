using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon.GUI
{
    #region Delegates
    public delegate void OnHover(Element pSender);
    public delegate void OnClick(Element pSender, ClickType Clicks);
    public delegate void OnDblClick(Element pSender, ClickType Clicks);
    public delegate void OnReleased(Element pSender, ClickType Clicks);
    #endregion

    public abstract class Element : IActor
    {
        #region Variables privées
        private bool _dblClick = false; // TODO gérer l'évènement double click !
        #endregion

        #region Variables protected
        protected Vector2 _position;
        protected Vector2 _origin;
        protected Vector2 _size;
        #endregion

        #region Propriétés
        /// <summary>
        /// Delegate de fonction quand l'élément est survolé.
        /// </summary>
        public OnHover onHover { get; set; }
        /// <summary>
        /// Delegate de fonction quand l'élément est cliqué.
        /// </summary>
        public OnClick onClick { get; set; }
        /// <summary>
        /// [Pas encore implémenté] 
        /// TODO
        /// Delegate de fonction quand l'élément est double cliqué.
        /// </summary>
        public OnDblClick onDblClick { get; set; }
        /// <summary>
        /// Delegate de fonction quand l'élément est relaché.
        /// </summary>
        public OnReleased onReleased { get; set; }

        /// <summary>
        /// Zone de l'élément qui permet de gérer les collisions avec la souris
        /// </summary>
        public Rectangle BoundingBox { get; protected set; }
        /// <summary>
        /// Afficher le rectangle de collisions
        /// </summary>
        public bool ShowBoundingBox { get; set; } = false;

        public Vector2 Position { get => _position; set { _position = value; RefreshBoundingBox(); } }
        public Vector2 Origin { get => _origin; set { _origin = value; } }
        public Vector2 Size { get => _size;  set { _size = value; RefreshBoundingBox(); } }
        public float Angle { get; set; }
        public float Scale { get; set; }
        public bool Visible { get; set; } = true;
        public bool Remove { get; set; } = false;
        public bool Hover { get; protected set; }
        public bool Clicked { get; protected set; }
        #endregion

        #region Constructeur
        public Element(Vector2 pPosition, Vector2 pOrigin, Vector2 pSize, bool pVisible, float pScale = 1.0f)
        {
            Position = pPosition;
            Origin = pOrigin;
            Size = pSize;
            Scale = pScale;
            Visible = pVisible;
            MainGame.gameState.CurrentScene.AddActor(this);
        }
        #endregion

        #region Méthodes

        protected virtual void RefreshBoundingBox()
        {
            Vector2 location = Position - Origin;
            BoundingBox = new Rectangle(location.ToPoint(), Size.ToPoint());
        }

        public virtual void SetOriginToCenter()
        {
            Origin = Size / 2;
        }

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            #region Vérifie si la souris est en survol
            MouseState mouseState = Mouse.GetState();
            if (BoundingBox.Contains(mouseState.Position) && !Hover)
            {
                onHover?.Invoke(this);
            }
            Hover = BoundingBox.Contains(mouseState.Position);
            #endregion

            #region Evènements sur click

            if (Input.OnReleased(ClickType.Left))
            {
                Clicked = false;
            }

            if (Hover)
            {
                #region Click gauche
                if (Input.OnPressed(ClickType.Left))
                {
                    onClick?.Invoke(this, ClickType.Left); // Cliqué
                    Clicked = true;
                }
                else if (Input.OnReleased(ClickType.Left))
                {
                    onReleased?.Invoke(this, ClickType.Left); // Relaché
                }
                #endregion

                #region Click mollette
                if (Input.OnPressed(ClickType.Middle))
                {
                    onClick?.Invoke(this, ClickType.Middle); // Cliqué
                }
                else if (Input.OnReleased(ClickType.Middle))
                {
                    onReleased?.Invoke(this, ClickType.Middle); // Relaché
                }
                #endregion

                #region Click droit
                if (Input.OnPressed(ClickType.Right))
                {
                    onClick?.Invoke(this, ClickType.Right); // Cliqué
                }
                else if (Input.OnReleased(ClickType.Right))
                {
                    onReleased?.Invoke(this, ClickType.Right); // Relaché
                }
                #endregion

                #region Click sur X1
                if (Input.OnPressed(ClickType.X1))
                {
                    onClick?.Invoke(this, ClickType.X1); // Cliqué
                }
                else if (Input.OnReleased(ClickType.X1))
                {
                    onReleased?.Invoke(this, ClickType.X1); // Relaché
                }
                #endregion

                #region Click sur X2
                if (Input.OnPressed(ClickType.X2))
                {
                    onClick?.Invoke(this, ClickType.X2); // Cliqué
                }
                else if (Input.OnReleased(ClickType.X2))
                {
                    onReleased?.Invoke(this, ClickType.X2); // Relaché
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ShowBoundingBox)
            {
                Primitives2D.DrawRectangle(spriteBatch, BoundingBox, Color.Aqua);
            }
        }
        #endregion

        #endregion
    }
}
