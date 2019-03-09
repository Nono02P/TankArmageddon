using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Bullet : Sprite, ISentByTank
        {
            #region Evènements
            public event ExplosionHandler OnBulletExplosion;
            #endregion

            #region Propriétés
            public Action.eActions BulletType { get; protected set; }
            public Tank Sender { get; private set; }
            public int Radius { get; protected set; }
            public int Force { get; protected set; }
            public bool FocusCamera { get; set; }
            #endregion

            #region Constructeur
            public Bullet(Tank pShooter, Texture2D pImage, Vector2 pPosition, Vector2 pVelocity, Action.eActions pBulletType, Vector2 pScale) : base(pImage)
            {
                Layer += 0.2f;
                Sender = pShooter;
                Position = pPosition;
                Velocity = pVelocity;
                Scale = pScale;
                BulletType = pBulletType;
                switch (BulletType)
                {
                    case TankArmageddon.Action.eActions.None:
                        break;
                    case TankArmageddon.Action.eActions.iGrayBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                        Radius = 40;
                        Force = 2;
                        break;
                    case TankArmageddon.Action.eActions.iGrayBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet2.png").ImgBox;
                        Radius = 50;
                        Force = 15;
                        break;
                    case TankArmageddon.Action.eActions.GoldBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet5.png").ImgBox;
                        Radius = 40;
                        Force = 4;
                        break;
                    case TankArmageddon.Action.eActions.GoldBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                        Radius = 50;
                        Force = 25;
                        break;
                    case TankArmageddon.Action.eActions.GrayMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                        Radius = 80;
                        Force = 40;
                        break;
                    case TankArmageddon.Action.eActions.GreenMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
                        Radius = 80;
                        Force = 50;
                        break;
                    default:
                        break;
                }
                Origin = new Vector2(ImgBox.Value.Width / 2, ImgBox.Value.Height / 2);
                OnBulletExplosion += Sender.Parent.Parent.CreateExplosion;
            }
            #endregion

            #region Fin de vie de la bullet
            protected void Die(bool pWithExplosion, Vector2? pPosition = null)
            {
                if (pWithExplosion)
                {
                    OnBulletExplosion?.Invoke(this, new ExplosionEventArgs((Vector2)pPosition, Radius, Force));
                }
                Remove = true;
                OnBulletExplosion -= Sender.Parent.Parent.CreateExplosion;
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

            #region Update
            public override void Update(GameTime gameTime)
            {
                #region Prend le focus de la caméra
                if (FocusCamera)
                {
                    Camera cam = MainGame.Camera;
                    if (cam.Position.Y < 0 && Position.Y - BoundingBox.Height > 0)
                    {
                        cam.SetCameraOnActor(this, HAlign.Center, VAlign.Bottom);
                    }
                    else
                    {
                        cam.SetCameraOnActor(this, true, Position.Y - BoundingBox.Height < 0 || cam.Position.Y < 0);
                    }
                }
                #endregion

                Gameplay g = Sender.Parent.Parent;

                #region Application de la gravité
                float vx = Velocity.X;
                float vy = Velocity.Y;
                vy += GRAVITY / 20;
                Velocity = new Vector2(vx, vy);

                // Dans l'eau
                if (Position.Y > g.WaterLevel)
                {
                    Velocity.Normalize();
                    if (Position.Y > g.WaterLevel + g.WaterHeight)
                        Die(false);
                }
                #endregion

                #region Angle de la bullet en fonction de sa direction
                Angle = (float)utils.MathAngle(Velocity);
                #endregion

                #region Récupération de l'ancienne position pour vérifier les collisions
                Vector2 previousPosition = Position;
                #endregion

                base.Update(gameTime);

                #region Collisions avec le sol
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
                if (Position.Y > Sender.Parent.Parent.MapSize.Y)
                {
                    Die(false, collisionPosition);
                }
                #endregion

                #region Sortie de map
                if (Sender.Parent.Parent.OutOfMap(this))
                {
                    Die(false);
                }
                #endregion
            }
            #endregion
        }
    }
}