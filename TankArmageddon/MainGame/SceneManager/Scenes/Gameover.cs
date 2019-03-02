using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Gameover : Scene
    {
        #region Constantes
        private const int TIMER_INTRO = 2;
        #endregion

        #region Variables privées
        private float _currentTimerIntro = 0;
        private Texture2D _background;
        private Color _backgroundColor;
        private GroupSelection _groupMenu;
        private Textbox _looser;
        private Tweening _tweeningLooser;
        private Textbox _menu;
        private Tweening _tweeningMenu;
        private Textbox _exit;
        private Tweening _tweeningExit;
        #endregion

        #region Méthodes

        #region Load/Unload
        public override void Load()
        {
            #region Chargement de la musique
            sndMusic = AssetManager.mscGameover;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;
            #endregion

            #region Image de background
            _backgroundColor = Color.Transparent;
            _background = AssetManager.Gameover;
            #endregion

            #region Création des éléments la GUI
            SpriteFont font = AssetManager.MenuFont;
            int screenWidth = MainGame.Screen.Width;
            int screenHeight = MainGame.Screen.Height;

            _groupMenu = new GroupSelection();

            string text = "MENU";
            _menu = new Textbox(new Vector2(-200, (screenHeight - font.MeasureString(text).Y) / 2 - 40), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_menu);

            text = "QUITTER";
            _exit = new Textbox(new Vector2(-2200, (screenHeight - font.MeasureString(text).Y) / 2 + 40), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_exit);

            text = "Tout le monde est mort !";
            _looser = new Textbox(new Vector2(-3000, 50), font, text);
            _looser.ApplyColor(Color.Green, Color.Black);

            _groupMenu.RefreshColors();
            _groupMenu.CurrentSelection = 0;

            foreach (Textbox txt in _groupMenu.Elements)
            {
                txt.OnHover += Textbox_OnHover;
                txt.OnClick += Textbox_OnClick;
            }
            #endregion

            #region Création du tweening
            _tweeningMenu = new Tweening(Tweening.Tween.InSine, (int)_menu.Position.X, (int)(screenWidth - _menu.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningExit = new Tweening(Tweening.Tween.InSine, (int)_exit.Position.X, (int)(screenWidth - _exit.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningLooser = new Tweening(Tweening.Tween.InSine, (int)_looser.Position.X, (int)(screenWidth - _looser.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            #endregion

            base.Load();
        }
        #endregion

        #region Gestion des boutons
        private void Textbox_OnHover(object sender, EventArgs e)
        {
            if (_currentTimerIntro >= TIMER_INTRO)
            {
                _groupMenu.CurrentSelection = _groupMenu.Elements.FindIndex(elm => elm == (Element)sender);
            }
        }

        private void Textbox_OnClick(object sender, ClickType Clicks)
        {
            if (_currentTimerIntro >= TIMER_INTRO && Clicks == ClickType.Left)
            {
                _groupMenu.CurrentSelection = _groupMenu.Elements.FindIndex(elm => elm == (Element)sender);
                Select();
            }
        }
        #endregion

        #region Sélection de scène
        private void Select()
        {
            switch (_groupMenu.CurrentSelection)
            {
                case 0:
                    MainGame.ChangeScene(SceneType.Menu);
                    break;
                case 1:
                    MainGame.ExitGame = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            _tweeningMenu.Update(gameTime);
            _tweeningExit.Update(gameTime);
            _tweeningLooser.Update(gameTime);

            _menu.Position = new Vector2(_tweeningMenu.Result, _menu.Position.Y);
            _exit.Position = new Vector2(_tweeningExit.Result, _exit.Position.Y);
            _looser.Position = new Vector2(_tweeningLooser.Result, _looser.Position.Y);

            if (_currentTimerIntro < TIMER_INTRO)
            {
                _currentTimerIntro += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _backgroundColor = Color.Lerp(Color.Transparent, Color.White, _currentTimerIntro / TIMER_INTRO);
            }
            else
            {
                if (Input.OnPressed(Keys.Up))
                    _groupMenu.CurrentSelection--;

                if (Input.OnPressed(Keys.Down))
                    _groupMenu.CurrentSelection++;

                if (Input.OnPressed(Keys.Enter) || Input.OnPressed(Keys.Space))
                {
                    Select();
                }
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