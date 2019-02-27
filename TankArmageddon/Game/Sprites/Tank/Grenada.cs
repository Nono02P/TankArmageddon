using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Grenada : Sprite
        {
            #region Variables privées
            private Textbox _textBox;
            private Timer _timer;
            private int _counter;
            private int _timerExplosion;
            #endregion

            #region Propriétés
            public event ExplosionHandler OnExplosion;
            public Action.eActions BulletType { get; protected set; }
            public Tank Parent { get; private set; }
            public int Radius { get; protected set; }
            public int Force { get; protected set; }
            public bool FocusCamera { get; set; }
            #endregion

            #region Constructeur
            public Grenada(Tank pShooter, Vector2 pPosition, Vector2 pVelocity, Action.eActions pBulletType) : base()
            {
                #region Initialisation des valeurs
                Layer += 0.2f;
                Parent = pShooter;
                switch (pBulletType)
                {
                    case Action.eActions.Grenada:
                        Radius = 80;
                        Force = 25;
                        _timerExplosion = 5;
                        Image = AssetManager.Grenada;
                        Scale = Vector2.One * 0.1f;
                        break;
                    case Action.eActions.SaintGrenada:
                        Radius = 80;
                        Force = 50;
                        _timerExplosion = 5;
                        Image = AssetManager.SaintGrenada;
                        Scale = Vector2.One * 0.08f;
                        break;
                    default:
                        break;
                }
                Position = pPosition;
                Velocity = pVelocity;
                BulletType = pBulletType;
                Origin = new Vector2(Image.Width / 2, Image.Height);
                #endregion

                #region Initialisation de la GUI
                Vector2 p = new Vector2(BoundingBox.Left, BoundingBox.Top - BoundingBox.Height);
                _textBox = new Textbox(p, AssetManager.MainFont, _timerExplosion.ToString());
                _textBox.ApplyColor(Color.Red, Color.White);
                #endregion

                #region Initialisation du timer d'explosion
                _timer = new Timer(1000);
                _timer.Enabled = true;
                #endregion

                #region Abonnement aux évènements
                OnExplosion += Parent.Parent.Parent.CreateExplosion;
                _timer.Elapsed += OnTimerExplosionElapsed;
                #endregion
            }
            #endregion

            #region Evènements du timer d'explosion
            public void OnTimerExplosionElapsed(object sender, ElapsedEventArgs e)
            {
                _counter++;
                _textBox.Text = (_timerExplosion - _counter).ToString();
                if (_counter >= _timerExplosion)
                {
                    Die(true);
                }
            }
            #endregion

            #region Fin de vie de la bullet
            protected void Die(bool pWithExplosion)
            {
                if (pWithExplosion)
                {
                    OnExplosion?.Invoke(this, new ExplosionEventArgs(Position, Radius, Force));
                }
                Remove = true;
                _textBox.Remove = true;
                OnExplosion -= Parent.Parent.Parent.CreateExplosion;
                _timer.Elapsed -= OnTimerExplosionElapsed;
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

                #region Application de la gravité
                float vx = Velocity.X;
                float vy = Velocity.Y;

                vy += GRAVITY / 10;
                Velocity = new Vector2(vx, vy);
                #endregion

                #region Récupération de l'ancienne position pour vérifier les collisions
                Vector2 previousPosition = Position;
                #endregion

                base.Update(gameTime);

                #region Collision avec le sol
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
                    #region Calcul de l'emplacement de la nouvelle position
                    Vector2 center;
                    Vector2 before;
                    Vector2 after;
                    int delta = 2;
                    if (collisionPosition.Y > previousPosition.Y)
                    {
                        center = g.FindHighestPoint(collisionPosition, 0);
                        before = g.FindHighestPoint(collisionPosition, -delta);
                        after = g.FindHighestPoint(collisionPosition, delta);
                    }
                    else
                    {
                        center = g.FindHighestPoint(previousPosition, 0);
                        before = g.FindHighestPoint(previousPosition, -delta);
                        after = g.FindHighestPoint(previousPosition, delta);
                    }
                    #endregion

                    #region Rebond sur le sol
                    float hyp = (float)utils.MathDist(Vector2.Zero, Velocity) - FRICTION / 2;

                    if (hyp > 0)
                    {
                        float angleFloor = (float)utils.MathAngle(after - before);
                        float angleDirection = (float)utils.MathAngle(Velocity);
                        float normAngle = angleDirection - angleFloor;
                        vx = (float)Math.Cos(normAngle) * hyp;
                        vy = -(float)Math.Sin(normAngle) * hyp;
                        angleDirection = (float)utils.MathAngle(new Vector2(vx, vy));
                        angleDirection += angleFloor;
                        vx = (float)Math.Cos(angleDirection) * hyp;
                        vy = (float)Math.Sin(angleDirection) * hyp;
                        Velocity = new Vector2(vx, vy);
                    }
                    Position = collisionPosition + center;
                    #endregion
                }
                #endregion

                #region Positionnement de la GUI
                _textBox.Position = new Vector2(BoundingBox.Left, BoundingBox.Top - BoundingBox.Height);
                #endregion

                #region Sortie de map
                if (Parent.Parent.Parent.OutOfMap(this))
                {
                    Die(false);
                }
                #endregion
            }
            #endregion
        }
    }
}