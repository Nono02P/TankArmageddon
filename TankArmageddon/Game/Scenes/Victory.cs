using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace TankArmageddon
{
    class Victory : Scene
    {
        #region Constructeur
        public Victory() {}
        #endregion

        #region Méthodes

        #region Load/Unload
        public override void Load()
        {
            sndMusic = AssetManager.sndMusicVictory;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;

            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(AssetManager.MainFont, "This is the Victory !!", new Vector2(1, 1), Color.White);
            base.Draw(spriteBatch, gameTime);
        }
        #endregion
        
        #endregion
    }
}
