using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Sprite : ICollisionnable
    {
        #region Evènements
        public event onSpriteEffectsChange OnSpriteEffectsChange;
        #endregion

        #region Variables privées
        private SpriteEffects _effects;
        private Vector2 _position = Vector2.Zero;
        private Rectangle? _imgBox = null;
        private Vector2 _origin = Vector2.Zero;
        private Vector2 _scale = Vector2.One;
        private Texture2D _image;
        #endregion

        #region Propriétés
        public Vector2 Position { get => _position; set { if (_position != value) { _position = value; RefreshBoundingBox(); } } }
        public Vector2 Velocity { get; set; }
        public IBoundingBox BoundingBox { get; protected set; } = new RectangleBBox();
        public Rectangle? ImgBox { get => _imgBox; protected set { if (_imgBox != value) { _imgBox = value; RefreshBoundingBox(); } } }
        public Vector2 Origin { get => _origin; set { if (_origin != value) { _origin = value; RefreshBoundingBox(); } } }
        public Vector2 Scale { get => _scale; set { if (_scale != value) { _scale = value; RefreshBoundingBox(); } } }
        public Texture2D Image { get => _image; protected set { _image = value; RefreshBoundingBox(); } }
        public float Angle { get; set; }
        public bool Remove { get; set; }
        public SpriteEffects Effects { get { return _effects; } protected set { if (_effects != value) { OnSpriteEffectsChange?.Invoke(this, _effects, value); _effects = value; } } }
        public bool ShowBoundingBox { get; set; }
        #endregion

        #region Constructeur
        public Sprite(Texture2D pImage, Rectangle? pImgBox, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale)
        {
            Image = pImage;
            ImgBox = pImgBox;
            Position = pPosition;
            Origin = pOrigin;
            Scale = pScale;
            Effects = SpriteEffects.None;
            MainGame.CurrentScene.AddActor(this);
        }

        protected Sprite(Texture2D pImage)
        {
            Image = pImage;
            MainGame.CurrentScene.AddActor(this);
        }

        protected Sprite()
        {
            MainGame.CurrentScene.AddActor(this);
        }
        #endregion
        
        #region Collisions
        public virtual void TouchedBy(ICollisionnable collisionnable) { }
        #endregion

        #region BoundingBox
        public virtual void RefreshBoundingBox()
        {
            RectangleBBox r = (RectangleBBox)BoundingBox;
            if (ImgBox == null)
            {
                r.Rectangle = new Rectangle((int)(Position.X - Origin.X * Scale.X), (int)(Position.Y - Origin.Y * Scale.Y), (int)(Image.Width * Scale.X), (int)(Image.Height * Scale.Y));
            }
            else
            {
                r.Rectangle = new Rectangle((int)(Position.X - Origin.X * Scale.X), (int)(Position.Y - Origin.Y * Scale.Y), (int)(ImgBox.Value.Width * Scale.X), (int)(ImgBox.Value.Height * Scale.Y));
            }
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            Position += Velocity;
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, ImgBox, Color.White, Angle, Origin, Scale, Effects, 0);
            if (ShowBoundingBox)
                BoundingBox.Draw(spriteBatch, gameTime);
        }

        public void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime) { }
        #endregion
    }
}
