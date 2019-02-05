using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    #region Delegate
    public delegate void onBoolChange(object sender, bool previous, bool actual);
    public delegate void onByteChange(object sender, byte previous, byte actual);
    public delegate void onSbyteChange(object sender, sbyte previous, sbyte actual);
    public delegate void onUshortChange(object sender, ushort previous, ushort actual);
    public delegate void onShortChange(object sender, short previous, short actual);
    public delegate void onUintChange(object sender, uint previous, uint actual);
    public delegate void onIntChange(object sender, int previous, int actual);
    public delegate void onUlongChange(object sender, ulong previous, ulong actual);
    public delegate void onLongChange(object sender, long previous, long actual);
    public delegate void onFloatChange(object sender, float previous, float actual);
    public delegate void onDoubleChange(object sender, double previous, double actual);
    public delegate void onDecimalChange(object sender, decimal previous, decimal actual);
    public delegate void onCharChange(object sender, char previous, char actual);
    public delegate void onStringChange(object sender, string previous, string actual);
    public delegate void onVector2Change(object sender, Vector2 previous, Vector2 actual);
    public delegate void onVector3Change(object sender, Vector3 previous, Vector3 actual);
    public delegate void onVector4Change(object sender, Vector4 previous, Vector4 actual);
    public delegate void onRectangleChange(object sender, Rectangle previous, Rectangle actual);
    public delegate void onTexture2DChange(object sender, Texture2D previous, Texture2D actual);
    public delegate void onSpriteEffectsChange(object sender, SpriteEffects previous, SpriteEffects actual);
    #endregion

    public class Sprite : IActor , ICollisionnable
    {
        #region Evènements
        public event onSpriteEffectsChange OnSpriteEffectsChange;
        #endregion

        #region Variables privées
        private SpriteEffects _effects;
        #endregion

        #region Propriétés
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle BoundingBox { get; private set; }
        public Rectangle ImgBox { get; protected set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Texture2D Image { get; private set; }
        public float Angle { get; set; }
        public bool Remove { get; set; }
        public SpriteEffects Effects { get { return _effects; } protected set { if (_effects != value) { OnSpriteEffectsChange?.Invoke(this, _effects, value); _effects = value; } } }
        #endregion

        #region Constructeur
        public Sprite(Texture2D pImage, Rectangle pImgBox, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale)
        {
            Image = pImage;
            ImgBox = pImgBox;
            Position = pPosition;
            Origin = pOrigin;
            Scale = pScale;
            Effects = SpriteEffects.None;
            MainGame.gameState.CurrentScene.AddActor(this);
        }

        protected Sprite(Texture2D pImage)
        {
            Image = pImage;
            MainGame.gameState.CurrentScene.AddActor(this);
        }
        #endregion

        #region Méthodes

        #region Collisions
        public virtual void TouchedBy(ICollisionnable collisionnable) { }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            Position += Velocity;
            BoundingBox = new Rectangle((int)(Position.X - Origin.X * Scale.X), (int)(Position.Y - Origin.Y * Scale.Y), (int)(ImgBox.Width * Scale.X), (int)(ImgBox.Height * Scale.Y));
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, ImgBox, Color.White, Angle, Origin, Scale, Effects, 0);
            spriteBatch.DrawRectangle(BoundingBox, Color.Red, 2);
        }
        #endregion

        #endregion
    }
}
