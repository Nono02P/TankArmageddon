using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Menu : Scene
    {
        #region Constantes
        private const int TIMER_INTRO = 2;
        #endregion

        #region Variables privées
        private float _currentTimerIntro = 0;
        private Texture2D _background;
        private Color _backgroundColor;
        private GroupSelection _groupMenu;
        private Textbox _play;
        private Tweening _tweeningPlay;
        private Textbox _howToPlay;
        private Tweening _tweeninghowToPlay;
        private Textbox _exit;
        private Tweening _tweeningExit;
        #endregion

        #region Constructeur
        public Menu() {}
        #endregion

        #region Méthodes

        #region Gestion des clicks boutons
        public void OnClickPlay(Element element, ClickType clickType)
        {
            if (element is Button)
            {
                if (clickType == ClickType.Left)
                {
                    MainGame.ChangeScene(SceneType.Gameplay);
                }
            }
        }
        #endregion

        #region Load/Unload
        public override void Load()
        {
            #region Chargement de la musique
            sndMusic = AssetManager.mscMenu;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;
            #endregion

            #region Image de background
            _backgroundColor = Color.Transparent;
            _background = AssetManager.Menu;
            #endregion

            #region Création des éléments la GUI
            SpriteFont font = AssetManager.MenuFont;
            int screenWidth = MainGame.Screen.Width;
            int screenHeight = MainGame.Screen.Height;

            _groupMenu = new GroupSelection();
            
            string text = "JOUER";
            _play = new Textbox(new Vector2(-200, (screenHeight - font.MeasureString(text).Y) / 2 - 80), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_play);

            text = "COMMENT JOUER";
            _howToPlay = new Textbox(new Vector2(-500, (screenHeight - font.MeasureString(text).Y) / 2), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_howToPlay);

            text = "QUITTER";
            _exit = new Textbox(new Vector2(-800, (screenHeight - font.MeasureString(text).Y) / 2 + 80), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_exit);

            _groupMenu.RefreshColors();
            _groupMenu.CurrentSelection = 0;

            foreach (Textbox txt in _groupMenu.Elements)
            {
                txt.OnHover += Textbox_OnHover;
                txt.OnClick += Textbox_OnClick;
            }
            #endregion

            #region Création du tweening
            _tweeningPlay = new Tweening(Tweening.Tween.InSine, (int)_play.Position.X, (int)(screenWidth - _play.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeninghowToPlay = new Tweening(Tweening.Tween.InSine, (int)_howToPlay.Position.X, (int)(screenWidth - _howToPlay.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningExit = new Tweening(Tweening.Tween.InSine, (int)_exit.Position.X, (int)(screenWidth - _exit.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            #endregion

            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
            base.UnLoad();
        }
        #endregion

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

        private void Select()
        {
            switch (_groupMenu.CurrentSelection)
            {
                case 0:
                    MainGame.ChangeScene(SceneType.Gameplay);
                    break;
                case 1:
                    MainGame.ChangeScene(SceneType.HowToPlay);
                    break;
                case 2:
                    MainGame.ExitGame = true;
                    break;
                default:
                    break;
            }
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            _tweeningPlay.Update(gameTime);
            _tweeninghowToPlay.Update(gameTime);
            _tweeningExit.Update(gameTime);

            _play.Position = new Vector2(_tweeningPlay.Result, _play.Position.Y);
            _howToPlay.Position = new Vector2(_tweeninghowToPlay.Result, _howToPlay.Position.Y);
            _exit.Position = new Vector2(_tweeningExit.Result, _exit.Position.Y);

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