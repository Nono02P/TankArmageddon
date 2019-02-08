using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Particle : Sprite
    {
        private const float GRAVITY = 1f;
        private const float SPEED_MAX = 3f;

        private Gameplay Parent;

        public Particle(Gameplay pParent, Texture2D pImage, Rectangle? pImgBox, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, pImgBox, pPosition, pOrigin, pScale)
        {
            Parent = pParent;
        }

        public override void Update(GameTime gameTime)
        {
            if (Parent.IsSolid(Position))
            {
                Remove = true;
            }

            float vx = Velocity.X;
            float vy = Velocity.Y;
            vy += GRAVITY;
            vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
            vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
            Velocity = new Vector2(vx, vy);
            base.Update(gameTime);
        }
    }
}
