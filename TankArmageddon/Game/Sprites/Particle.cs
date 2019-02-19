using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Particle : Sprite
    {
        private const float GRAVITY = .1f;
        private const float SPEED_MAX = 3f;

        private Gameplay Parent;

        public Particle(Gameplay pParent, Texture2D pImage, Rectangle? pImgBox, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, pImgBox, pPosition, pOrigin, pScale)
        {
            Parent = pParent;
        }

        public override void Update(GameTime gameTime)
        {
            float vx = Velocity.X;
            float vy = Velocity.Y;
            vy += GRAVITY;
            //vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
            //vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
            Velocity = new Vector2(vx, vy);
            Vector2 previousPos = Position;

            base.Update(gameTime);

            if (Parent.IsSolid(Position))
            {
                Remove = true;
            }

            /*
            if (Parent.IsSolid(Position, previousPos))
            {
                Vector2 center = Parent.FindHighestPoint(Position, 0);
                Vector2 before = Parent.FindHighestPoint(Position, -20);
                Vector2 after = Parent.FindHighestPoint(Position, 20);
                
                Position += center;

                float floorAngle = (float)utils.MathAngle(after - before) - MathHelper.ToRadians(-90);
                Velocity = new Vector2((float)Math.Cos(floorAngle) * 10, (float)Math.Cos(floorAngle) * 10);
            }
            */
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, ImgBox, Color.Gray, Angle, Origin, Scale, Effects, 0);
        }
    }
}
