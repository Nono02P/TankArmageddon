using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Bullet : Sprite
        {
            public event onExplosion OnBulletExplosion;
            public eActions BulletType { get; private set; }
            public Tank Parent { get; private set; }

            public Bullet(Tank pShooter, Texture2D pImage, Vector2 pPosition, Vector2 pVelocity, eActions pBulletType, Vector2 pScale) : base(pImage)
            {
                Parent = pShooter;
                Position = pPosition;
                Velocity = pVelocity;
                Scale = pScale;
                BulletType = pBulletType;
                switch (BulletType)
                {
                    case eActions.None:
                        break;
                    case eActions.iGrayBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                        break;
                    case eActions.iGrayBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet2.png").ImgBox;
                        break;
                    case eActions.GoldBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet5.png").ImgBox;
                        break;
                    case eActions.GoldBombshell:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                        break;
                    case eActions.GrayMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                        break;
                    case eActions.GreenMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
                        break;
                    case eActions.Mine:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_mineOn.png").ImgBox;
                        break;
                    case eActions.Grenada:
                        break;
                    case eActions.SaintGrenada:
                        break;
                    case eActions.iTankBaseBall:
                        break;
                    case eActions.HelicoTank:
                        break;
                    case eActions.Drilling:
                        break;
                    case eActions.iDropFuel:
                        break;
                    case eActions.iWhiteFlag:
                        break;
                    default:
                        break;
                }
                Origin = new Vector2(ImgBox.Value.Width / 2, ImgBox.Value.Height / 2);
                OnBulletExplosion += Parent.Parent.Parent.CreateExplosion;
            }

            public override void Update(GameTime gameTime)
            {
                Angle = (float)utils.MathAngle(Velocity);
                base.Update(gameTime);
                if (Parent.Parent.Parent.IsSolid(BoundingBox.Center.ToVector2()))
                {
                    Explose();
                }
            }

            private void Explose()
            {
                // TODO : gérer plusieurs types, avec différentes valeurs de radian et forces en fonction du type de bullet.
                OnBulletExplosion?.Invoke(this, new ExplosionEventArgs(Position, 100, 10));
                Remove = true;
            }

            public override void TouchedBy(ICollisionnable collisionnable)
            {
                if (!(collisionnable is Bullet))
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