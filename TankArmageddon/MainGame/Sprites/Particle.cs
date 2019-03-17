using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Particle : Sprite
    {
        #region Constantes
        private const float GRAVITY = .1f;
        private const float SPEED_MAX = 3f;
        #endregion

        #region Variables privées
        private Gameplay Parent;
        #endregion

        public bool RotationFollowVelocity { get; set; }

        #region Constructeur
        public Particle(Gameplay pParent, Texture2D pImage, Rectangle? pImgBox, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, pImgBox, pPosition, pOrigin, pScale)
        {
            Parent = pParent;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Application de la gravité
            float vx = Velocity.X;
            float vy = Velocity.Y;
            vy += GRAVITY;
            //vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
            //vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
            Velocity = new Vector2(vx, vy);
            #endregion

            #region Rotation suivant la vélocité
            if (RotationFollowVelocity)
            {
                Angle = (float)utils.MathAngle(Velocity);
            }
            #endregion

            #region Récupération de l'ancienne position pour gérer les collisions
            Vector2 previousPos = Position;
            #endregion

            base.Update(gameTime);

            #region Collision avec le sol
            if (Parent.IsSolid(Position) || Position.X < 0 || Position.X > Parent.MapSize.X || Position.Y < 0 || Position.Y > Parent.MapSize.Y)
            {
                Remove = true;
            }
            #endregion
        }
        #endregion
    }
}