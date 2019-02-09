﻿using System;
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
        private const float FUEL_CONSUMPTION = 0.1f;
        #endregion

        #region Variables privées
        private bool _onFloor = false;
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
        private int _life = 100;
        private float _fuel = 100;
        private BarGraph _lifeBar;
        private BarGraph _fuelBar;
        private TimeSpan _barSpeed = new TimeSpan(0, 0, 0, 0, 250);
        private Group _group;
        private Image _guiGameplayIndex;
        #endregion

        #region Propriétés
        public Team Parent { get; set; }
        public Color TeamColor { get; private set; }
        public string Name { get; set; }
        public float AngleCannon { get; set; } = MathHelper.ToRadians(360);
        public eActions SelectedWeapon { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Parachute { get; private set; } = true;
        public int Life
        {
            get { return _life; }
            set { _life = MathHelper.Clamp(value, 0, 100); _lifeBar.SetProgressiveValue(value, _barSpeed); }
        }
        public float Fuel
        {
            get { return _fuel; }
            set { _fuel = MathHelper.Clamp(value, 0, 100); _fuelBar.SetProgressiveValue(value, _barSpeed); }
        }
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

            _group = new Group();
            _textBox = new Textbox(new Vector2(0, - ImgBox.Value.Height * 1.25f), AssetManager.MainFont, Name + " : " + Life);
            _textBox.ApplyColor(pTeamColor, Color.Black);
            _textBox.SetOriginToCenter();
            _group.AddElement(_textBox);

            Vector2 s = new Vector2(100, 15);
            _lifeBar = new BarGraph(Life, Life, new Vector2(0, -ImgBox.Value.Height), new Vector2(s.X / 2, 0), s, Color.Blue, Color.Green);
            _group.AddElement(_lifeBar);

            _fuelBar = new BarGraph(Fuel, Fuel, new Vector2(0, -ImgBox.Value.Height * 0.75f), new Vector2(s.X / 2, 0), s, Color.Blue, new Color(240, 105, 105));
            _group.AddElement(_fuelBar);

            Texture2D cursor = new Texture2D(MainGame.graphics.GraphicsDevice, 15, 10);
            Color[] data = new Color[cursor.Width * cursor.Height];
            Color teamColor = Parent.TeamColor;
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = teamColor;
            }
            cursor.SetData(data);

            _guiGameplayIndex = new Image(cursor, Parent.Parent.GetPositionOnMinimap(pPosition.X), new Vector2(cursor.Width / 2, cursor.Height * 1.65f));
            Parent.Parent.GUIGroup.AddElement(_guiGameplayIndex);

            OnSpriteEffectsChange += SpriteEffectsChange;
        }
        #endregion

        public override void TouchedBy(ICollisionnable collisionnable)
        {
            
        }

        public void Shoot(byte pForce, eActions pBulletType)
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

            if (Left && _onFloor)
            {
                if (Fuel > 0)
                {
                    vx -= xSpeed;
                    Fuel -= FUEL_CONSUMPTION;
                    Parent.RefreshCameraOnSelection();
                }
                Effects = SpriteEffects.FlipHorizontally;
            }
            if (Right && _onFloor)
            {
                if (Fuel > 0)
                {
                    vx += xSpeed;
                    Fuel -= FUEL_CONSUMPTION;
                    Parent.RefreshCameraOnSelection();
                }
                Effects = SpriteEffects.None;
            }

            #region Gestion de la gravité en fonction du parachute
            // Applique la gravité uniquement si le tank ne touche pas le sol (permet au tank d'éviter de passer au travers le sol qui n'est pas totalement détruit)
            if (Parachute && !_onFloor)
            {
                vy = GRAVITY;
            }
            else if (!_onFloor)
            {
                vy += GRAVITY;
            }
            #endregion

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
                Parachute = false;
                _onFloor = true;

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
                _onFloor = false;
            }
            #endregion

            _group.Position = Position;
            _guiGameplayIndex.Position = Parent.Parent.GetPositionOnMinimap(Position.X);

            #region Calcul de la position du canon par rapport à la position et l'angle du tank.
            float hyp = ImgBox.Value.Height * Scale.Y * 0.35f;
            float x = (float)Math.Sin(Angle) * hyp;
            float y = (float)Math.Cos(Angle) * hyp;
            _positionCannon = new Vector2(Position.X + x, Position.Y - y);
            #endregion

            if (Position.Y > Parent.Parent.WaterLevel)
            {
                Remove = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, _imgWheel, Color.White, Angle, _originWheel, Scale, Effects, 0);
            spriteBatch.Draw(Image, _positionCannon, _imgCannon, Color.White, Angle + AngleCannon, _originCannon, Scale, SpriteEffects.None, 0);
            base.Draw(spriteBatch, gameTime);
            _group.Draw(spriteBatch, gameTime);
        }

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
                    case eActions.Helicotank:
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