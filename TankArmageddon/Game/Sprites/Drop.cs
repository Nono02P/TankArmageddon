using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Drop : Sprite
    {
        #region Enumérations
        public enum eDropType
        {
            Weapon,
            Health,
            Fuel,
        }
        #endregion

        #region Evènements
        public event onExplosion OnDropExplosion;
        #endregion

        #region Constantes
        private const float GRAVITY = 1f;
        #endregion

        #region Variables privées
        private bool _onFloor = false;
        private Texture2D _parachuteImage;
        #endregion

        #region Propriétés
        public Gameplay Parent { get; private set; }
        public eDropType DropType { get; private set; }
        public int Value { get; private set; } = utils.MathRnd(50, 100);
        public bool Parachute { get; private set; } = true;
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
            Origin = new Vector2(ImgBox.Value.Center.X / 2, ImgBox.Value.Bottom);

            _parachuteImage = AssetManager.Parachute;
            OnDropExplosion += Parent.CreateExplosion;
        }
        #endregion

        #region Fonctions
        public override void TouchedBy(ICollisionnable collisionnable)
        {
            Remove = true;
            if (collisionnable is Tank.Bullet)
            {
                OnDropExplosion?.Invoke(this, new ExplosionEventArgs(Position, 50, 40));
            }
            if (collisionnable is Tank)
            {
                Tank t = (Tank)collisionnable;
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

            #region Collision avec le sol
            if (Parent.IsSolid(Position))
            {
                Parachute = false;
                _onFloor = true;

                // Récupère l'altitude en Y à position.X -20 et +20 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                Vector2 center = Parent.FindHighestPoint(Position, 0);
                Vector2 before = Parent.FindHighestPoint(Position, -20);
                Vector2 after = Parent.FindHighestPoint(Position, 20);
                Angle = (float)utils.MathAngle(after - before);

                // Vérifie que le point le plus haut retourné n'est pas plus grand que le tank.
                // Permet d'empêcher le tank de se téléporter au dessus quand il est dans un trou.
                if (center.Y > -BoundingBox.Height)
                {
                    Position += center;
                }
                else if (before.Y > -BoundingBox.Height)
                {
                    Position += before;
                }
                else if (after.Y > -BoundingBox.Height)
                {
                    Position += after;
                }
            }
            else
            {
                _onFloor = false;
            }
            #endregion

            base.Update(gameTime);
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

        #endregion
    }
}
