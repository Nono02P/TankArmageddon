using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon.GUI
{
    #region Delegates
    public delegate void onHover(Element pSender);
    public delegate void onClick(Element pSender, ClickType Clicks);
    public delegate void onDblClick(Element pSender, ClickType Clicks);
    public delegate void onReleased(Element pSender, ClickType Clicks);
    #endregion

    public abstract class Element : IActor
    {
        #region Evènements
        /// <summary>
        /// Evènement apparaissant quand l'élément est survolé.
        /// </summary>
        public event onHover OnHover;
        /// <summary>
        /// Evènement apparaissant quand l'élément est cliqué.
        /// </summary>
        public event onClick OnClick;
        /// <summary>
        /// [Pas encore implémenté] 
        /// TODO
        /// Evènement apparaissant quand l'élément est double cliqué.
        /// </summary>
        public event onDblClick OnDblClick;
        /// <summary>
        /// Evènement apparaissant quand l'élément est relaché.
        /// </summary>
        public event onReleased OnReleased;

        /// <summary>
        /// Evènement apparaissant quand la position change.
        /// </summary>
        public event onVector2Change OnPositionChange;

        /// <summary>
        /// Evènement apparaissant quand l'origine change.
        /// </summary>
        public event onVector2Change OnOriginChange;

        /// <summary>
        /// Evènement apparaissant quand la taille change.
        /// </summary>
        public event onVector2Change OnSizeChange;
        #endregion

        #region Variables privées
        private bool _dblClick = false; // TODO gérer l'évènement double click !
        private float _scale = 1;
        #endregion

        #region Variables protected
        protected Vector2 _position;
        protected Vector2 _origin;
        protected Vector2 _size;
        #endregion

        #region Propriétés
        /// <summary>
        /// Zone de l'élément qui permet de gérer les collisions avec la souris
        /// </summary>
        public IBoundingBox BoundingBox { get; protected set; } = new RectangleBBox();
        /// <summary>
        /// Afficher le rectangle de collisions
        /// </summary>
        public bool ShowBoundingBox { get; set; } = false;

        public Vector2 Position { get => _position; set { if (_position != value) { Vector2 before = _position; _position = value; OnPositionChange?.Invoke(this, before, value); RefreshBoundingBox(); } } }
        public Vector2 Origin { get => _origin; set { if (_origin != value) { Vector2 before = _origin; _origin = value; OnOriginChange?.Invoke(this, before, value); RefreshBoundingBox(); } } }
        public Vector2 Size { get => _size; set { if (_size != value) { Vector2 before = _size;  _size = value; OnSizeChange?.Invoke(this, before, value); RefreshBoundingBox(); } } }
        public float Angle { get; set; }
        public float Scale { get => _scale; set { _scale = value; RefreshBoundingBox(); } }
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
            MainGame.CurrentScene.AddActor(this);
        }
        #endregion

        #region Méthodes

        public virtual void RefreshBoundingBox()
        {
            Vector2 location = Position - Origin;
            BoundingBox = new RectangleBBox(location.ToPoint(), Size.ToPoint());
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
            Point mousePosition = new Point((int)(mouseState.Position.X + MainGame.Camera.Position.X), (int)(mouseState.Position.Y + MainGame.Camera.Position.Y));
            if (BoundingBox.Contains(mousePosition) && !Hover)
            {
                OnHover?.Invoke(this);
            }
            Hover = BoundingBox.Contains(mousePosition);
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
                    OnClick?.Invoke(this, ClickType.Left); // Cliqué
                    Clicked = true;
                }
                else if (Input.OnReleased(ClickType.Left))
                {
                    OnReleased?.Invoke(this, ClickType.Left); // Relaché
                }
                #endregion

                #region Click mollette
                if (Input.OnPressed(ClickType.Middle))
                {
                    OnClick?.Invoke(this, ClickType.Middle); // Cliqué
                }
                else if (Input.OnReleased(ClickType.Middle))
                {
                    OnReleased?.Invoke(this, ClickType.Middle); // Relaché
                }
                #endregion

                #region Click droit
                if (Input.OnPressed(ClickType.Right))
                {
                    OnClick?.Invoke(this, ClickType.Right); // Cliqué
                }
                else if (Input.OnReleased(ClickType.Right))
                {
                    OnReleased?.Invoke(this, ClickType.Right); // Relaché
                }
                #endregion

                #region Click sur X1
                if (Input.OnPressed(ClickType.X1))
                {
                    OnClick?.Invoke(this, ClickType.X1); // Cliqué
                }
                else if (Input.OnReleased(ClickType.X1))
                {
                    OnReleased?.Invoke(this, ClickType.X1); // Relaché
                }
                #endregion

                #region Click sur X2
                if (Input.OnPressed(ClickType.X2))
                {
                    OnClick?.Invoke(this, ClickType.X2); // Cliqué
                }
                else if (Input.OnReleased(ClickType.X2))
                {
                    OnReleased?.Invoke(this, ClickType.X2); // Relaché
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ShowBoundingBox && Visible)
            {
                Primitives2D.DrawRectangle(spriteBatch, ((RectangleBBox)BoundingBox).Rectangle, Color.Aqua);
            }
        }

        public void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime) { }
        #endregion

        #endregion
    }
}
