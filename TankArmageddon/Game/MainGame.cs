﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    #region Delegate
    public delegate void onBoolChange(object sender, bool previous, bool actual);
    public delegate void onByteChange(object sender, byte previous, byte actual);
    public delegate void onSbyteChange(object sender, sbyte previous, sbyte actual);
    public delegate void onUshortChange(object sender, ushort previous, ushort actual);
    public delegate void onShortChange(object sender, short previous, short actual);
    public delegate void onUintChange(object sender, uint previous, uint actual);
    public delegate void onIntChange(object sender, int previous, int actual);
    public delegate void onUlongChange(object sender, ulong previous, ulong actual);
    public delegate void onLongChange(object sender, long previous, long actual);
    public delegate void onFloatChange(object sender, float previous, float actual);
    public delegate void onDoubleChange(object sender, double previous, double actual);
    public delegate void onDecimalChange(object sender, decimal previous, decimal actual);
    public delegate void onCharChange(object sender, char previous, char actual);
    public delegate void onStringChange(object sender, string previous, string actual);
    public delegate void onVector2Change(object sender, Vector2 previous, Vector2 actual);
    public delegate void onVector3Change(object sender, Vector3 previous, Vector3 actual);
    public delegate void onVector4Change(object sender, Vector4 previous, Vector4 actual);
    public delegate void onRectangleChange(object sender, Rectangle previous, Rectangle actual);
    public delegate void onTexture2DChange(object sender, Texture2D previous, Texture2D actual);
    public delegate void onSpriteEffectsChange(object sender, SpriteEffects previous, SpriteEffects actual);
    #endregion

    #region Enumérations
    public enum eDirection : byte
    {
        Top,
        Right,
        Bottom,
        Left,
    }

    public enum SceneType
    {
        Menu,
        Gameplay,
        Gameover,
        Victory,
    }
    #endregion

    public class MainGame : Game
    {
        #region Propriétés
        public static GraphicsDeviceManager graphics { get; private set; }
        public static SpriteBatch spriteBatch { get; private set; }
        public static Viewport Screen { get; private set; }
        public static Camera Camera { get; private set; }
        public static Scene CurrentScene { get; private set; }

        public static readonly bool UsingMouse = true;
        public static readonly bool UsingKeyboard = true;
        public static readonly bool UsingGamePad = false;
        public static readonly int GamePadMaxPlayer = 0;
        #endregion

        #region Constructeur
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
        }
        #endregion

        #region Méthodes

        #region Load/Unload
        protected override void LoadContent()
        {
            Screen = GraphicsDevice.Viewport;
            Camera = new Camera(Screen, Vector3.Zero);
            IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.Load(Content);
            ChangeScene(SceneType.Gameplay);
        }
        
        protected override void UnloadContent() { }
        #endregion

        #region Gestion des scènes
        public static void ChangeScene(SceneType pSceneType)
        {
            if (CurrentScene != null)
            {
                CurrentScene.UnLoad();
                CurrentScene = null;
            }
            switch (pSceneType)
            {
                case SceneType.Menu:
                    CurrentScene = new Menu();
                    break;
                case SceneType.Gameplay:
                    CurrentScene = new Gameplay();
                    break;
                case SceneType.Gameover:
                    CurrentScene = new Gameover();
                    break;
                case SceneType.Victory:
                    CurrentScene = new Victory();
                    break;
                default:
                    break;
            }
            Camera.Enable = false;
            Camera.Position = Vector3.Zero;
            CurrentScene.Load();
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update();
            Camera.Update();

            CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Avec l'anti-alliasing
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,null, Camera.Transformation);
            CurrentScene.Draw(spriteBatch, gameTime);
            spriteBatch.DrawString(AssetManager.MainFont, Camera.Position.ToString(), new Vector2(80, 60) + new Vector2(Camera.Position.X, Camera.Position.Y), Color.White);
            spriteBatch.DrawString(AssetManager.MainFont, Mouse.GetState().Position.ToString(), new Vector2(80, 80) + new Vector2(Camera.Position.X, Camera.Position.Y), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #endregion
    }
}
