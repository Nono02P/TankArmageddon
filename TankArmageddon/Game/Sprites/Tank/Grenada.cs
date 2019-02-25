using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Grenada : Sprite
        {
            private Group _group;
            private Textbox _textBox;

            public event ExplosionHandler OnBulletExplosion;
            public Action.eActions BulletType { get; protected set; }
            public Tank Parent { get; private set; }
            public int Radius { get; protected set; }
            public int Force { get; protected set; }
            public int ExplosionTimer { get; set; }

            public Grenada(Tank pShooter, Vector2 pPosition, Vector2 pVelocity, Action.eActions pBulletType) : base()
            {
                Parent = pShooter;
                switch (pBulletType)
                {
                    case Action.eActions.Grenada:
                        Radius = 80;
                        Force = 50;
                        ExplosionTimer = 3;
                        Image = AssetManager.Grenada;
                        Scale = Vector2.One * 0.1f;
                        break;
                    case Action.eActions.SaintGrenada:
                        Radius = 80;
                        Force = 50;
                        ExplosionTimer = 5;
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
                OnBulletExplosion += Parent.Parent.Parent.CreateExplosion;
            }

            protected void Explose()
            {
                OnBulletExplosion?.Invoke(this, new ExplosionEventArgs(Position, Radius, Force));
                Remove = true;
                OnBulletExplosion -= Parent.Parent.Parent.CreateExplosion;
            }

            public override void Update(GameTime gameTime)
            {
                #region Application de la gravité
                float vx = Velocity.X;
                float vy = Velocity.Y;

                vy += GRAVITY / 4;
                #endregion

                /*#region Application de la friction
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
                #endregion*/

                Velocity = new Vector2(vx, vy);

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
                    // Récupère l'altitude en Y à position.X -1 et +1 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                    Vector2 center = g.FindHighestPoint(collisionPosition, 0);
                    Vector2 before = g.FindHighestPoint(collisionPosition, -1);
                    Vector2 after = g.FindHighestPoint(collisionPosition, 1);

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
            }
        }
    }
}
