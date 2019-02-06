using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Tank : Sprite
    {
        #region Constantes
        private const float CANNON_SPEED = 0.01f;
        private const byte SPEED_MAX = 10;
        private const float SPEED = 10f;
        private const float FRICTION = 5f;
        private const float GRAVITY = 1f;
        #endregion

        #region Enumérations
        public enum eBulletType : byte
        {
            GrayBullet,
            GoldBullet,
            GrayMissile,
            GreenMissile,
        }
        #endregion

        #region Variables privées
        private bool onFloor = false;
        private eDirection _direction = eDirection.Right;
        private float _minCannonAngle = MathHelper.ToRadians(-90);
        private float _maxCannonAngle = MathHelper.ToRadians(0);
        private Vector2 _positionCannon;
        private Vector2 _originCannon;
        private Rectangle _imgCannon;
        private Vector2 _originWheel;
        private Vector2 _originWheelOffset;
        private Vector2 _originWheelNormal;
        private Rectangle _imgWheel;
        private Textbox _textBox;
        #endregion

        #region Propriétés
        public Team Parent { get; set; }
        public Color TeamColor { get; private set; }
        public byte Life { get; set; } = 100;
        public string Name { get; set; }
        public float AngleCannon { get; set; } = MathHelper.ToRadians(360);
        public eBulletType SelectedWeapon { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        #endregion

        #region Constructeur
        public Tank(Team pParent, Color pTeamColor, string pName, Texture2D pImage, Rectangle pTankImage, Rectangle pCannonImage, Rectangle pWheelImage, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, pTankImage, pPosition, pOrigin, pScale)
        {
            Angle = MathHelper.ToRadians(360);
            Parent = pParent;
            TeamColor = pTeamColor;
            Name = pName;
            _imgCannon = pCannonImage;
            _originCannon = new Vector2(0, _imgCannon.Height / 2);
            _imgWheel = pWheelImage;
            _originWheelOffset = new Vector2(ImgBox.Value.Width * 0.04f, 0);
            _originWheelNormal = new Vector2(_imgWheel.Width / 2, _imgWheel.Height / 2 - ImgBox.Value.Height / 2.5f);
            _originWheel = _originWheelNormal + _originWheelOffset;
            _textBox = new Textbox(new Vector2(Position.X, Position.Y - ImgBox.Value.Height), AssetManager.MainFont, Name + " : " + Life);
            _textBox.ApplyColor(pTeamColor, Color.Black);
            _textBox.SetOriginToCenter();
            OnSpriteEffectsChange += SpriteEffectsChange;
        }
        #endregion


        public override void TouchedBy(ICollisionnable collisionnable)
        {
            // TODO : Descendre la vie.
        }

        public void Shoot(byte pForce, eBulletType pBulletType)
        {
            float cosAngle = (float)Math.Cos(AngleCannon + Angle);
            float sinAngle = (float)Math.Sin(AngleCannon + Angle);
            Vector2 p = new Vector2(_imgCannon.Width * Scale.X * cosAngle, _imgCannon.Width * Scale.X * sinAngle);
            p += _positionCannon;
            Bullet b = new Bullet(this, Image, p, new Vector2(cosAngle * pForce, sinAngle * pForce), pBulletType, Scale);
        }

        public void SpriteEffectsChange(object sender, SpriteEffects previous, SpriteEffects after)
        {
            switch (after)
            {
                case SpriteEffects.None:
                    _direction = eDirection.Right;
                    _minCannonAngle = MathHelper.ToRadians(-90);
                    _maxCannonAngle = MathHelper.ToRadians(0);
                    _originWheel = _originWheelNormal + _originWheelOffset;
                    break;
                case SpriteEffects.FlipHorizontally:
                    _direction = eDirection.Left;
                    _minCannonAngle = MathHelper.ToRadians(-180);
                    _maxCannonAngle = MathHelper.ToRadians(-90);
                    _originWheel = _originWheelNormal - _originWheelOffset;
                    break;
                default:
                    break;
            }
            AngleCannon = -MathHelper.ToRadians(180) - AngleCannon;
        }

        public override void Update(GameTime gameTime)
        {
            #region Contrôles du canon
            if (Up && _direction == eDirection.Right || Down && _direction == eDirection.Left)
            {
                AngleCannon -= CANNON_SPEED;
            }
            if (Down && _direction == eDirection.Right || Up && _direction == eDirection.Left)
            {
                AngleCannon += CANNON_SPEED;
            }
            AngleCannon = MathHelper.Clamp(AngleCannon, _minCannonAngle, _maxCannonAngle);
            #endregion

            #region Contrôles des déplacements et de la gravité
            float vx = Velocity.X;
            float vy = Velocity.Y;
            float xSpeed = (float)Math.Cos(Angle) * SPEED;

            if (Left && onFloor)
            {
                vx -= xSpeed;
                Effects = SpriteEffects.FlipHorizontally;
                Parent.RefreshCameraOnSelection();
            }
            if (Right && onFloor)
            {
                vx += xSpeed;
                Effects = SpriteEffects.None;
                Parent.RefreshCameraOnSelection();
            }
            // Applique la gravité uniquement si le tank ne touche pas le sol (permet au tank d'éviter de passer au travers le sol qui n'est pas totalement détruit)
            if (!onFloor)
            {
                vy += GRAVITY;
            }

            if (vx > 0)
            {
                vx -= FRICTION;
                if (vx < 0)
                {
                    if (MathHelper.ToDegrees(Angle) < 0)
                    {
                        vx = 0;
                    }
                    else
                    {
                        vx = SPEED_MAX;
                    }
                }
            }
            else if (vx < 0)
            {
                vx += FRICTION;
                if (vx > 0)
                {
                    if (MathHelper.ToDegrees(Angle) > 0)
                    {
                        vx = 0;
                    }
                    else
                    {
                        vx = -SPEED_MAX;
                    }
                }
            }

            vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
            vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
            Velocity = new Vector2(vx, vy);
            #endregion

            base.Update(gameTime);

            #region Collisions avec le sol et angle du tank
            Vector2 p = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
            if (Parent.Parent.IsSolid(p))
            {
                onFloor = true;

                // Récupère l'altitude en Y à position.X -20 et +20 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                Vector2 center = Parent.Parent.FindHighestPoint(p, 0);
                Vector2 before = Parent.Parent.FindHighestPoint(p, -20);
                Vector2 after = Parent.Parent.FindHighestPoint(p, 20);
                Angle = (float)utils.MathAngle(after - before);

                // Vérifie que le point le plus haut retourné n'est pas plus grand que le tank.
                // Permet d'empêcher le tank de se téléporter au dessus quand il est dans un trou.
                if (center.Y > - BoundingBox.Height)
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
                onFloor = false;
            }
            #endregion

            _textBox.Position = new Vector2(Position.X, Position.Y - ImgBox.Value.Height);

            #region Calcul de la position du canon par rapport à la position et l'angle du tank.
            float hyp = ImgBox.Value.Height * Scale.Y * 0.35f;
            float x = (float)Math.Sin(Angle) * hyp;
            float y = (float)Math.Cos(Angle) * hyp;
            _positionCannon = new Vector2(Position.X + x, Position.Y - y);
            #endregion
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, _imgWheel, Color.White, Angle, _originWheel, Scale, Effects, 0);
            spriteBatch.Draw(Image, _positionCannon, _imgCannon, Color.White, Angle + AngleCannon, _originCannon, Scale, SpriteEffects.None, 0);
            base.Draw(spriteBatch, gameTime);
            _textBox.Draw(spriteBatch, gameTime);
        }

        public class Bullet : Sprite
        {
            public delegate void onBulletExplosion(object sender, BulletEventArgs e);

            public event onBulletExplosion OnBulletExplosion;
            public eBulletType BulletType { get; private set; }
            public Tank Parent { get; private set; }

            public Bullet(Tank pShooter, Texture2D pImage, Vector2 pPosition, Vector2 pVelocity, eBulletType pBulletType, Vector2 pScale) : base(pImage)
            {
                Parent = pShooter;
                Position = pPosition;
                Velocity = pVelocity;
                Scale = pScale;
                BulletType = pBulletType;
                switch (pBulletType)
                {
                    case eBulletType.GrayBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                        break;
                    case eBulletType.GoldBullet:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                        break;
                    case eBulletType.GrayMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                        break;
                    case eBulletType.GreenMissile:
                        ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
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
                // TODO : gérer plusieurs types radian et forces en fonction du type de bullet.
                OnBulletExplosion?.Invoke(this, new BulletEventArgs(Position, 100, 10));
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

            public class BulletEventArgs : EventArgs
            {
                public Vector2 Position { get; private set; }
                public int Radius { get; private set; }
                public int Force { get; private set; }

                public BulletEventArgs(Vector2 pPosition, int pRadius, int pForce)
                {
                    Position = pPosition;
                    Radius = pRadius;
                    Force = pForce;
                }
            }
        }
    }
}
