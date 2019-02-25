using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Bullet : Sprite
        {
            #region Evènements
            public event ExplosionHandler OnBulletExplosion;
            #endregion

            #region Propriétés
            public Action.eActions BulletType { get; protected set; }
            public Tank Parent { get; private set; }
            public int Radius { get; protected set; }
            public int Force { get; protected set; }
            #endregion

            #region Constructeur
            protected Bullet(Tank pShooter) : base()
            {
                Parent = pShooter;
                OnBulletExplosion += Parent.Parent.Parent.CreateExplosion;
            }

            public Bullet(Tank pShooter, Texture2D pImage, Vector2 pPosition, Vector2 pVelocity, Action.eActions pBulletType, Vector2 pScale) : base(pImage)
            {
                Parent = pShooter;
                Position = pPosition;
                Velocity = pVelocity;
                Scale = pScale;
                BulletType = pBulletType;
                switch (BulletType)
                {
                    case Action.eActions.None:
                        break;
                    case Action.eActions.iGrayBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                        Radius = 40;
                        Force = 2;
                        break;
                    case Action.eActions.iGrayBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet2.png").ImgBox;
                        Radius = 50;
                        Force = 10;
                        break;
                    case Action.eActions.GoldBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet5.png").ImgBox;
                        Radius = 40;
                        Force = 5;
                        break;
                    case Action.eActions.GoldBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                        Radius = 50;
                        Force = 20;
                        break;
                    case Action.eActions.GrayMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                        Radius = 80;
                        Force = 30;
                        break;
                    case Action.eActions.GreenMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
                        Radius = 80;
                        Force = 50;
                        break;
                    default:
                        break;
                }
                Origin = new Vector2(ImgBox.Value.Width / 2, ImgBox.Value.Height / 2);
                OnBulletExplosion += Parent.Parent.Parent.CreateExplosion;
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime)
            {
                #region Application de la gravité
                float vx = Velocity.X;
                float vy = Velocity.Y;
                vy += GRAVITY / 20;

                Velocity = new Vector2(vx, vy);
                #endregion

                #region Angle de la bullet en fonction de sa direction
                Angle = (float)utils.MathAngle(Velocity);
                #endregion

                #region Récupération de l'ancienne position pour vérifier les collisions
                Vector2 previousPosition = Position;
                #endregion

                base.Update(gameTime);

                #region Collisions avec le sol
                Gameplay g = Parent.Parent.Parent;

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
                    Die(true, collisionPosition);
                }
                if (Position.Y > Parent.Parent.Parent.MapSize.Y)
                {
                    Die(false, collisionPosition);
                }
                #endregion
            }
            #endregion

            #region Fin de vie de la bullet
            protected void Die(bool pWithExplosion, Vector2 pPosition)
            {
                if (pWithExplosion)
                {
                    OnBulletExplosion?.Invoke(this, new ExplosionEventArgs(pPosition, Radius, Force));
                }
                Remove = true;
                OnBulletExplosion -= Parent.Parent.Parent.CreateExplosion;
            }
            #endregion

            #region Collisions
            public override void TouchedBy(ICollisionnable collisionnable)
            {
                if (collisionnable is Drop || collisionnable is Tank || collisionnable is Mine)
                {
                    Die(true, Position);
                }
            }
            #endregion
        }
    }
}