using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public enum eDirection : byte
    {
        Top,
        Right,
        Bottom,
        Left,
    }

    public class MainGame : Game
    {
        #region Propriétés
        public static GraphicsDeviceManager graphics { get; private set; }
        public static SpriteBatch spriteBatch { get; private set; }
        public static GameState gameState { get; private set; }
        public static Viewport Screen { get; private set; }
        public static Camera Camera { get; set; }

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
            gameState = new GameState();
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
        }
        #endregion

        #region Méthodes

        #region Initialisation
        protected override void Initialize()
        {
            Screen = GraphicsDevice.Viewport;
            Camera = new Camera(Screen, Vector3.Zero);
            IsMouseVisible = true;
            base.Initialize();
        }
        #endregion

        #region Load/Unload
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.Load(Content);
            gameState.ChangeScene(GameState.SceneType.Gameplay);
            
        }
        
        protected override void UnloadContent() { }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update();
            Camera.Update();

            gameState.CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Avec l'anti-alliasing
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,null, Camera.Transformation);
            gameState.CurrentScene.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #endregion
    }
}
