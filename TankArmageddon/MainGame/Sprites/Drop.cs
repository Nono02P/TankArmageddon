using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Drop : Sprite, ISentByTank
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
        public Tank Sender { get; private set; }
        public eDropType DropType { get; private set; }
        public int Value { get; set; } = utils.MathRnd(50, 100);
        public bool Parachute { get => _parachute; private set { if (_parachute != value) { _parachute = value; _imgParachute.Visible = value; Parent.FinnishTour(true); } } }
        #endregion

        #region Constructeur
        public Drop(Gameplay pParent, eDropType pDropType, Texture2D pImage, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale, Tank pSender = null) : base(pImage, null, pPosition, pOrigin, pScale)
        {
            Parent = pParent;
            Sender = pSender;
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
            Parent.OnExplosion += Gameplay_OnExplosion;
        }
        #endregion

        #region Explosion
        private void Explode()
        {
            // Le désabonnement se fait avant de créer l'explosion, pour éviter qu'il vérifie qu'il soit dans son rayon d'explosion.
            Parent.OnExplosion -= Gameplay_OnExplosion;
            
            OnDropExplosion?.Invoke(this, new ExplosionEventArgs(Position, 50 + Value, 40 + Value / 10));
            Remove = true;
            OnDropExplosion -= Parent.CreateExplosion;
        }
        #endregion

        #region Sur explosion sur la map
        private void Gameplay_OnExplosion(object sender, ExplosionEventArgs e)
        {
            Circle c = e.ExplosionCircle;
            if (c.Intersects(BoundingBox))
            {
                Explode();
                ISentByTank sentByTank = (ISentByTank)sender;
                if (sentByTank.Sender != null)
                {
                    sentByTank.Sender.Parent.Control.FitnessScore += NeuralNetworkControl.BonusDropTouched;
                }
            }
        }
        #endregion

        #region Collisions
        public override void TouchedBy(ICollisionnable collisionnable)
        {
            if (collisionnable is Tank)
            {
                if (Parachute)
                {
                    Parachute = false;
                }
                Tank t = (Tank)collisionnable;
                if (t.IsControlled)
                {
                    t.Parent.Control.FitnessScore += NeuralNetworkControl.BonusDropPickUp;
                }
                Parent.RefreshActionButtonInventory();
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

            Vector2 previousPosition = Position;

            base.Update(gameTime);

            #region Collision avec le sol
            Gameplay g = Parent;

            bool collision = false;
            Vector2 normalised = Vector2.Normalize(Velocity);
            Vector2 collisionPosition = previousPosition;
            do
            {
                collisionPosition += normalised;
                if (g.IsSolid(collisionPosition))
                {
                    collision = true;
                    collisionPosition -= normalised;
                }
            } while (!collision && Math.Abs((collisionPosition - Position).X) >= Math.Abs(normalised.X) && Math.Abs((collisionPosition - Position).Y) >= Math.Abs(normalised.Y));
            if (collision)
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

            #region Gestion de la GUI
            _group.Position = Position;
            if (Remove)
            {
                _group.Remove = true;
            }
            #endregion
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