﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Bullet : Sprite
        {
            public event ExplosionHandler OnBulletExplosion;
            public Action.eActions BulletType { get; private set; }
            public Tank Parent { get; private set; }
            public int Radius { get; private set; }
            public int Force { get; private set; }

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

            public override void Update(GameTime gameTime)
            {
                float vx = Velocity.X;
                float vy = Velocity.Y;
                vy += GRAVITY / 20;

                Velocity = new Vector2(vx, vy);
                Angle = (float)utils.MathAngle(Velocity);
                base.Update(gameTime);
                if (Parent.Parent.Parent.IsSolid(BoundingBox.Center.ToVector2()))
                {
                    Explose();
                }
                if (Position.Y > Parent.Parent.Parent.WaterLevel)
                {
                    Remove = true;
                }
            }

            private void Explose()
            {
                OnBulletExplosion?.Invoke(this, new ExplosionEventArgs(Position, Radius, Force));
                Remove = true;
            }

            public override void TouchedBy(ICollisionnable collisionnable)
            {
                if (!(collisionnable is Bullet) && !(collisionnable is Particle))
                {
                    Explose();
                }
            }

            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                spriteBatch.DrawRectangle(ImgBox.Value, Color.Red, 2);
                base.Draw(spriteBatch, gameTime);
            }
        }
    }
}