using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class HowToPlay : Scene
    {
        #region Constantes
        private const int TIMER_INTRO = 2;
        #endregion

        #region Variables privées
        private float _currentTimerIntro = 0;
        private Texture2D _background;
        private Color _backgroundColor;
        private Textbox _menu;
        #endregion

        #region Méthodes

        #region Load/Unload
        public override void Load()
        {
            /*
            #region Chargement de la musique
            sndMusic = AssetManager.mscMenu;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;
            #endregion
            */
            #region Image de background
            _backgroundColor = Color.Transparent;
            _background = AssetManager.HowToPlay;
            #endregion

            #region Création des éléments la GUI
            SpriteFont font = AssetManager.MenuFont;
            int screenWidth = MainGame.Screen.Width;
            int screenHeight = MainGame.Screen.Height;
            
            string text = "RETOUR";
            _menu = new Textbox(new Vector2(10, 10), font, text);
            _menu.ApplyColor(Color.Red, Color.Gray);
            _menu.Color_Hover = Color.Yellow;
            _menu.ColorBck_Hover = Color.Red;
            _menu.OnClick += Textbox_OnClick;
            #endregion

            base.Load();
        }
        #endregion

        #region Gestion des boutons
        private void Textbox_OnClick(object sender, ClickType Clicks)
        {
            if (_currentTimerIntro >= TIMER_INTRO && Clicks == ClickType.Left)
            {
                MainGame.ChangeScene(SceneType.Menu);
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (_currentTimerIntro < TIMER_INTRO)
            {
                _currentTimerIntro += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _backgroundColor = Color.Lerp(Color.Transparent, Color.White, _currentTimerIntro / TIMER_INTRO);
            }
            else
            {
                if (Input.OnPressed(Keys.Enter) || Input.OnPressed(Keys.Space) || Input.OnPressed(Keys.Escape))
                    MainGame.ChangeScene(SceneType.Menu);
            }
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_background, _background.Bounds, _backgroundColor);
            base.Draw(spriteBatch, gameTime);
        }
        #endregion

        #endregion
    }
}
