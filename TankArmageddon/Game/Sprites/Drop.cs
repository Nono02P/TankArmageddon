using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Drop : Sprite
    {
        #region Constantes
        private const float GRAVITY = 10f;
        #endregion

        #region Enumérations
        public enum eDropType
        {
            Weapon,
            Health,
            Fuel,
        }
        #endregion

        #region Evènements
        public event ExplosionHandler OnDropExplosion;
        #endregion

        #region Variables privées
        private bool _onFloor = false;
        private Image _imgParachute;
        private Group _group;
        private bool _parachute = true;
        #endregion

        #region Propriétés
        public Gameplay Parent { get; private set; }
        public eDropType DropType { get; private set; }
        public int Value { get; private set; } = utils.MathRnd(50, 100);
        public bool Parachute { get => _parachute; private set { _parachute = value; _imgParachute.Visible = value; } }
        #endregion

        #region Constructeur
        public Drop(Gameplay pParent, eDropType pDropType, Texture2D pImage, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, null, pPosition, pOrigin, pScale)
        {
            Parent = pParent;
            DropType = pDropType;
            switch (DropType)
            {
                case eDropType.Weapon:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_crateAmmo.png").ImgBox;
                    break;
                case eDropType.Health:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_crateRepair.png").ImgBox;
                    break;
                case eDropType.Fuel:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_barrelRed.png").ImgBox;
                    break;
                default:
                    break;
            }
            Origin = new Vector2(ImgBox.Value.Width / 2, ImgBox.Value.Height);

            _group = new Group();
            Rectangle paraImgBox = AssetManager.ParachutesImgBox[utils.MathRnd(0, AssetManager.ParachutesImgBox.Count)];
            _imgParachute = new Image(AssetManager.Parachute, paraImgBox, new Vector2(0, - ImgBox.Value.Height), new Vector2(paraImgBox.Width / 2, paraImgBox.Height));
            _group.AddElement(_imgParachute);

            OnDropExplosion += Parent.CreateExplosion;
        }
        #endregion

        #region Collisions
        public override void TouchedBy(ICollisionnable collisionnable)
        {
            if (collisionnable is Tank.Bullet)
            {
                OnDropExplosion?.Invoke(this, new ExplosionEventArgs(Position, 50, 40));
                Remove = true;
            }
            if (collisionnable is Tank)
            {
                Tank t = (Tank)collisionnable;
                Remove = true;
                switch (DropType)
                {
                    case eDropType.Weapon:
                        t.Parent.OpenLoot();
                        break;
                    case eDropType.Health:
                        t.Life += Value;
                        break;
                    case eDropType.Fuel:
                        t.Fuel += Value;
                        break;
                    default:
                        break;
                }
            }
            /*if (collisionnable is Drop)
            {
                if (!_onFloor)
                {
                    Drop d = (Drop)collisionnable;
                    Position = new Vector2(Position.X, d.BoundingBox.Top);
                    Angle = d.Angle;
                }
            }*/
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Gestion de la gravité en fonction du parachute
            if (Parachute && !_onFloor)
            {
                Velocity = new Vector2(0, GRAVITY / 4);
            }
            else if (!_onFloor)
            {
                Velocity = new Vector2(0, GRAVITY);
            }
            #endregion

            Vector2 previousPosLeft = new Vector2(Position.X - ImgBox.Value.Width / 2, Position.Y);
            Vector2 previousPosMiddle = Position;
            Vector2 previousPosRight = new Vector2(Position.X + ImgBox.Value.Width / 2, Position.Y);

            base.Update(gameTime);

            #region Collision avec le sol
            Vector2 newPosLeft = new Vector2(Position.X - ImgBox.Value.Width / 2, Position.Y);
            Vector2 newPosMiddle = Position;
            Vector2 newPosRight = new Vector2(Position.X + ImgBox.Value.Width / 2, Position.Y);

            if (Parent.IsSolid(newPosLeft, previousPosLeft) || 
                Parent.IsSolid(newPosMiddle, previousPosMiddle) || 
                Parent.IsSolid(newPosRight, previousPosRight))
            {
                Parachute = false;
                _onFloor = true;

                // Récupère l'altitude en Y à position.X -20 et +20 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                Vector2 center = Parent.FindHighestPoint(Position, 0);
                Vector2 before = Parent.FindHighestPoint(Position, -20);
                Vector2 after = Parent.FindHighestPoint(Position, 20);
                Angle = (float)utils.MathAngle(after - before);
                Position += center;
            }
            else
            {
                _onFloor = false;
            }
            #endregion

            _group.Position = Position;
            if (Remove)
            {
                _group.Remove = true;
            }
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Parachute)
            {
                // spriteBatch.Draw(_parachuteImage);
            }
            base.Draw(spriteBatch, gameTime);
        }
        #endregion
    }
}