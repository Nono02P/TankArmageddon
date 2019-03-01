using Microsoft.Xna.Framework;
using System;
using System.Timers;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Mine : Sprite
        {
            #region Constantes
            private const int TIMER_EXPLOSION = 2;
            private const int RADIUS_EXPLOSION = 50;
            private const int FORCE = 15;
            #endregion

            #region Enumérations
            public enum eState
            {
                Off,
                On,
                Pressed,
            }
            #endregion

            #region Variables privées
            private Timer _timerExplosion;
            private int _counter = 0;
            private Rectangle _imgBoxOn;
            private Rectangle _imgBoxPressed;
            private bool _onFloor;
            #endregion

            #region Propriétés
            public Tank Parent { get; private set; }
            public eState State { get; private set; }
            #endregion

            #region Constructeur
            public Mine(Tank pParent, Vector2 pPosition) : base()
            {
                #region Initialisation des valeurs
                Layer += 0.2f;
                Parent = pParent;
                Image = AssetManager.TanksSpriteSheet;
                ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_mineOff.png").ImgBox;
                _imgBoxOn = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_mineOn.png").ImgBox;
                _imgBoxPressed = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_minePressed.png").ImgBox;
                State = eState.Off;
                Position = pPosition;
                Origin = new Vector2(ImgBox.Value.Width / 2, ImgBox.Value.Height);
                Scale = Vector2.One * 0.5f;
                BoundingBox = new Circle(Position.ToPoint(), RADIUS_EXPLOSION, 20, Color.White);
                #endregion

                #region Initialisation du timer d'explosion
                _timerExplosion = new Timer(1000);
                _timerExplosion.Enabled = false;
                #endregion

                #region Abonnement aux évènements
                _timerExplosion.Elapsed += OnTimerExplosionElapsed;
                Parent.Parent.Parent.OnTourTimerEnd += Gameplay_OnTourTimerEnd;
                #endregion
            }
            #endregion

            #region Sur fin de tour Activation de la mine
            private void Gameplay_OnTourTimerEnd(object sender, EventArgs e)
            {
                State = eState.On;
                ImgBox = _imgBoxOn;
                Parent.Parent.Parent.OnTourTimerEnd -= Gameplay_OnTourTimerEnd;
            }
            #endregion

            #region Sur collision active la mine
            public override void TouchedBy(ICollisionnable collisionnable)
            {
                if (State == eState.On && collisionnable is Tank)
                {
                    State = eState.Pressed;
                    _timerExplosion.Enabled = true;
                }
            }
            #endregion

            #region Evènements du timer d'explosion
            public void OnTimerExplosionElapsed(object sender, ElapsedEventArgs e)
            {
                _counter++;
                if (ImgBox.Value == _imgBoxOn)
                {
                    ImgBox = _imgBoxPressed;
                }
                else
                {
                    ImgBox = _imgBoxOn;
                }
                if (_counter >= TIMER_EXPLOSION)
                {
                    Parent.Parent.Parent.CreateExplosion(this, new ExplosionEventArgs(Position, RADIUS_EXPLOSION, FORCE));
                    Remove = true;
                    _timerExplosion.Elapsed -= OnTimerExplosionElapsed;
                }
            }
            #endregion

            #region 
            private void Die(bool pWithExplosion)
            {
                
            }
            #endregion

            #region BoundingBox
            public override void RefreshBoundingBox()
            {
                BoundingBox.Location = Position.ToPoint();
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime)
            {
                Gameplay g = Parent.Parent.Parent;

                #region Gestion de la gravité
                float vx = Velocity.X;
                float vy = Velocity.Y;

                if (!_onFloor)
                {
                    vy = GRAVITY;
                }

                // Dans l'eau
                if (Position.Y > g.WaterLevel)
                {
                    Velocity.Normalize();
                    if (Position.Y > g.WaterLevel + g.WaterHeight)
                        Die(false);
                }
                #endregion

                #region Limitations de la vélocité
                vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
                vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
                Velocity = new Vector2(vx, vy);
                #endregion

                #region Récupération de l'ancienne position en 3 points en bas (Gauche / Centre / Droite) pour vérifier les collisions
                Vector2 previousPosMiddle = new Vector2(BoundingBox.Location.X, BoundingBox.Size.Y);
                Vector2 previousPosLeft = new Vector2(BoundingBox.Left, BoundingBox.Bottom);
                Vector2 previousPosRight = new Vector2(BoundingBox.Right, BoundingBox.Bottom);
                #endregion

                base.Update(gameTime);

                #region Collision avec le sol
                Vector2 newPosLeft = new Vector2(Position.X - ImgBox.Value.Width / 2, Position.Y);
                Vector2 newPosMiddle = Position;
                Vector2 newPosRight = new Vector2(Position.X + ImgBox.Value.Width / 2, Position.Y);

                if (g.IsSolid(newPosLeft, previousPosLeft) ||
                    g.IsSolid(newPosMiddle, previousPosMiddle) ||
                    g.IsSolid(newPosRight, previousPosRight))
                {
                    _onFloor = true;

                    // Récupère l'altitude en Y à position.X -20 et +20 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                    Vector2 center = g.FindHighestPoint(Position, 0);
                    Vector2 before = g.FindHighestPoint(Position, - BoundingBox.Width / 2);
                    Vector2 after = g.FindHighestPoint(Position, BoundingBox.Width / 2);
                    Angle = (float)utils.MathAngle(after - before);
                    Position += center;
                }
                else
                {
                    _onFloor = false;
                }
                RefreshBoundingBox();
                #endregion
            }
            #endregion
        }
    }
}