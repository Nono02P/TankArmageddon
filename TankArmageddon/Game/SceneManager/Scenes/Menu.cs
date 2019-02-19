using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace TankArmageddon
{
    class Menu : Scene
    {
        #region Variables privées
        private GUI.Button _myButton;
        #endregion

        #region Constructeur
        public Menu() {}
        #endregion

        #region Méthodes

        #region Gestion des clicks boutons
        public void OnClickPlay(GUI.Element element, ClickType clickType)
        {
            if (element is GUI.Button)
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
            sndMusic = AssetManager.mscMenu;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;

            /*Texture2D img = AssetManager.Button;
            Vector2 position = new Vector2(MainGame.Screen.Width / 2, MainGame.Screen.Height / 2);
            Vector2 origin = new Vector2(img.Width / 2, img.Height / 2);
            _myButton = new GUI.Button(position, origin, 1.0f, true, img, null, null, AssetManager.MainFont, "ESSAIS");
            _myButton.OnClick += OnClickPlay;
            _myButton.AlignText(GUI.HAlign.Center, GUI.VAlign.Middle);
            _myButton.TextBox.ShowBoundingBox = true;
            lstActors.Add(_myButton);

            GUI.Slider slider = new GUI.Slider(new Vector2(100), Vector2.Zero, new Vector2(300, 5), new Vector2(5, 10));
            lstActors.Add(slider);*/

            base.Load();
        }

        public override void UnLoad()
        {
            MediaPlayer.Stop();
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
            spriteBatch.DrawString(AssetManager.MainFont, "This is the menu !!", new Vector2(1, 1), Color.White);
            base.Draw(spriteBatch, gameTime);
        }
        #endregion
        
        #endregion
    }
}