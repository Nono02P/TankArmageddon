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
        private const int TIMER_INTRO = 1;
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
        private Textbox _trainingMode;
        private Tweening _tweeningTrainingMode;
        private GroupSelection _groupNbOfTeam;
        private Textbox _numberOfTeamDescription;
        private Tweening _tweeningNbOfTeamDescription;
        private Textbox[] _numberOfTeam;
        private Tweening[] _tweeningNbOfTeam;
        private GroupSelection _groupNbOfTank;
        private Textbox _numberOfTankDescription;
        private Tweening _tweeningNbOfTankDescription;
        private Textbox[] _numberOfTank;
        private Tweening[] _tweeningNbOfTank;
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

            #region Menu
            _groupMenu = new GroupSelection();
            
            string text = "JOUER";
            _play = new Textbox(new Vector2(-200, (screenHeight - font.MeasureString(text).Y) / 2 - 80), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_play);

            text = "COMMENT JOUER";
            _howToPlay = new Textbox(new Vector2(-1200, (screenHeight - font.MeasureString(text).Y) / 2), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_howToPlay);

            text = "QUITTER";
            _exit = new Textbox(new Vector2(-2200, (screenHeight - font.MeasureString(text).Y) / 2 + 80), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_exit);

            text = "ENTRAINER L'IA";
            _trainingMode = new Textbox(new Vector2(-3200, (screenHeight - font.MeasureString(text).Y) / 2 + 160), font, text);
            _groupMenu.AddElement((IIntegrableMenu)_trainingMode);

            _groupMenu.RefreshColors();
            _groupMenu.CurrentSelection = 0;

            foreach (Textbox txt in _groupMenu.Elements)
            {
                txt.OnHover += MenuTextbox_OnHover;
                txt.OnClick += MenuTextbox_OnClick;
            }
            #endregion

            #region Nombre d'équipes
            _numberOfTeamDescription = new Textbox(new Vector2(-1000, 10), font, "Nombre d'équipes : ");

            _groupNbOfTeam = new GroupSelection();
            _numberOfTeam = new Textbox[3];
            for (int i = 0; i < _numberOfTeam.Length; i++)
            {
                _numberOfTeam[i] = new Textbox(new Vector2(-1610 + 80 * i, 10), font, (i + 2).ToString());
                _groupNbOfTeam.AddElement((IIntegrableMenu)_numberOfTeam[i]);
                
                _numberOfTeam[i].OnClick += NbOfTeamTextbox_OnClick;
            }
            _groupNbOfTeam.CurrentSelection = 0;
            SelectNbOfTeam();
            #endregion

            #region Nombre de tanks
            _numberOfTankDescription = new Textbox(new Vector2(-1000, 80), font, "Nombre de tanks : ");

            _groupNbOfTank = new GroupSelection();
            _numberOfTank = new Textbox[5];
            for (int i = 0; i < _numberOfTank.Length; i++)
            {
                _numberOfTank[i] = new Textbox(new Vector2(-1610 + 80 * i, 80), font, (i + 1).ToString());
                _groupNbOfTank.AddElement((IIntegrableMenu)_numberOfTank[i]);

                _numberOfTank[i].OnClick += NbOfTankTextbox_OnClick;
            }
            _groupNbOfTank.CurrentSelection = 0;
            SelectNbOfTank();
            #endregion

            #endregion

            #region Création du tweening
            _tweeningPlay = new Tweening(Tweening.Tween.InSine, (int)_play.Position.X, (int)(screenWidth - _play.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeninghowToPlay = new Tweening(Tweening.Tween.InSine, (int)_howToPlay.Position.X, (int)(screenWidth - _howToPlay.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningExit = new Tweening(Tweening.Tween.InSine, (int)_exit.Position.X, (int)(screenWidth - _exit.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningTrainingMode = new Tweening(Tweening.Tween.InSine, (int)_trainingMode.Position.X, (int)(screenWidth - _trainingMode.Size.X) / 2, new TimeSpan(0, 0, 0, TIMER_INTRO));

            _tweeningNbOfTeamDescription = new Tweening(Tweening.Tween.InSine, (int)_numberOfTeamDescription.Position.X, 10, new TimeSpan(0, 0, 0, TIMER_INTRO));
            _tweeningNbOfTankDescription = new Tweening(Tweening.Tween.InSine, (int)_numberOfTankDescription.Position.X, 10, new TimeSpan(0, 0, 0, TIMER_INTRO));

            _tweeningNbOfTeam = new Tweening[_numberOfTeam.Length];
            for (int i = 0; i < _tweeningNbOfTeam.Length; i++)
            {
                _tweeningNbOfTeam[i] = new Tweening(Tweening.Tween.InSine, (int)_numberOfTeam[i].Position.X, 610 + 80 * i, new TimeSpan(0, 0, 0, TIMER_INTRO));
            }

            _tweeningNbOfTank = new Tweening[_numberOfTank.Length];
            for (int i = 0; i < _tweeningNbOfTank.Length; i++)
            {
                _tweeningNbOfTank[i] = new Tweening(Tweening.Tween.InSine, (int)_numberOfTank[i].Position.X, 610 + 80 * i, new TimeSpan(0, 0, 0, TIMER_INTRO));
            }
            #endregion

            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
            base.UnLoad();
        }
        #endregion

        #region Gestion des boutons

        #region Menu
        private void MenuTextbox_OnHover(object sender, EventArgs e)
        {
            if (_currentTimerIntro >= TIMER_INTRO)
            {
                _groupMenu.CurrentSelection = _groupMenu.Elements.FindIndex(elm => elm == (Element)sender);
            }
        }


        private void MenuTextbox_OnClick(object sender, ClickType Clicks)
        {
            if (_currentTimerIntro >= TIMER_INTRO && Clicks == ClickType.Left)
            {
                _groupMenu.CurrentSelection = _groupMenu.Elements.FindIndex(elm => elm == (Element)sender);
                Select();
            }
        }
        #endregion

        #region Nombre d'équipes
        private void NbOfTeamTextbox_OnClick(object sender, ClickType Clicks)
        {
            if (_currentTimerIntro >= TIMER_INTRO && Clicks == ClickType.Left)
            {
                _groupNbOfTeam.CurrentSelection = _groupNbOfTeam.Elements.FindIndex(elm => elm == (Element)sender);
                SelectNbOfTeam();
            }
        }
        #endregion

        #region Nombre de tanks.
        private void NbOfTankTextbox_OnClick(object sender, ClickType Clicks)
        {
            if (_currentTimerIntro >= TIMER_INTRO && Clicks == ClickType.Left)
            {
                _groupNbOfTank.CurrentSelection = _groupNbOfTank.Elements.FindIndex(elm => elm == (Element)sender);
                SelectNbOfTank();
            }
        }
        #endregion

        #endregion

        #region Sélection de scène
        private void Select()
        {
            MainGame.IATrainingMode = false;
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
                case 3:
                    MainGame.IATrainingMode = true;
                    MainGame.NumberOfTeam = 15;
                    MainGame.NumberOfTank = 2;
                    MainGame.AutoRestart = true;
                    MainGame.ChangeScene(SceneType.Gameplay);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Sélection du nombre d'équipes
        private void SelectNbOfTeam()
        {
            Textbox txt = (Textbox)_groupNbOfTeam.Elements[_groupNbOfTeam.CurrentSelection];
            MainGame.NumberOfTeam = int.Parse(txt.Text);
        }
        #endregion

        #region Sélection du nombre de tanks
        private void SelectNbOfTank()
        {
            Textbox txt = (Textbox)_groupNbOfTank.Elements[_groupNbOfTank.CurrentSelection];
            MainGame.NumberOfTank = int.Parse(txt.Text);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            _tweeningPlay.Update(gameTime);
            _tweeninghowToPlay.Update(gameTime);
            _tweeningExit.Update(gameTime);
            _tweeningTrainingMode.Update(gameTime);

            _play.Position = new Vector2(_tweeningPlay.Result, _play.Position.Y);
            _howToPlay.Position = new Vector2(_tweeninghowToPlay.Result, _howToPlay.Position.Y);
            _exit.Position = new Vector2(_tweeningExit.Result, _exit.Position.Y);
            _trainingMode.Position = new Vector2(_tweeningTrainingMode.Result, _trainingMode.Position.Y);

            _tweeningNbOfTeamDescription.Update(gameTime);
            _numberOfTeamDescription.Position = new Vector2(_tweeningNbOfTeamDescription.Result, _numberOfTeamDescription.Position.Y);
            for (int i = 0; i < _tweeningNbOfTeam.Length; i++)
            {
                _tweeningNbOfTeam[i].Update(gameTime);
                _numberOfTeam[i].Position = new Vector2(_tweeningNbOfTeam[i].Result, _numberOfTeam[i].Position.Y);
            }

            _tweeningNbOfTankDescription.Update(gameTime);
            _numberOfTankDescription.Position = new Vector2(_tweeningNbOfTankDescription.Result, _numberOfTankDescription.Position.Y);
            for (int i = 0; i < _tweeningNbOfTank.Length; i++)
            {
                _tweeningNbOfTank[i].Update(gameTime);
                _numberOfTank[i].Position = new Vector2(_tweeningNbOfTank[i].Result, _numberOfTank[i].Position.Y);
            }

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
            if (MainGame.AutoRestart)
            {
                MainGame.IATrainingMode = true;
                MainGame.NumberOfTeam = 15;
                MainGame.NumberOfTank = 2;
                MainGame.ChangeScene(SceneType.Gameplay);
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
    }
}