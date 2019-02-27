using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    class BackGround
    {
        #region Propriétés
        public Vector2 Position1 { get; private set; }
        public Vector2 Position2 { get; private set; }
        public Texture2D Image { get; }
        public float Speed { get; set; }
        #endregion

        #region Constructeur
        public BackGround(Texture2D pTexture, float pSpeed)
        {
            Image = pTexture;
            Speed = pSpeed;
            Position1 = new Vector2(0, 0);
            Position2 = new Vector2(Image.Width, 0);
        }
        #endregion
        
        #region Update
        public void Update(GameTime gameTime)
        {
            float x1 = Position1.X;
            float x2 = Position2.X;
            x1 += Speed;
            x2 += Speed;

            if (Position1.X < - Image.Width)
            {
                x1 = Position2.X + Image.Width;
            }
            if (Position2.X < -Image.Width)
            {
                x2 = Position1.X + Image.Width;
            }
            Position1 = new Vector2(x1, Position1.Y);
            Position2 = new Vector2(x2, Position2.Y);
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position1, Color.White);
            spriteBatch.Draw(Image, Position2, Color.White);
        }
        #endregion
    }
}