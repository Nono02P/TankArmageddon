using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public class PlayerControl : IControl
    {
        #region Variables privées
        private bool _previousRefreshState;
        private SoundEffect _horn;
        #endregion

        #region Propriétés
        public Team Parent { get; private set; }
        public int FitnessScore { get; set; }
        public bool OnPressedLeft { get; private set; }
        public bool OnPressedRight { get; private set; }
        public bool OnPressedUp { get; private set; }
        public bool OnPressedDown { get; private set; }
        public bool OnPressedSpace { get; private set; }
        public bool OnPressedN { get; private set; }
        public bool IsDownLeft { get; private set; }
        public bool IsDownRight { get; private set; }
        public bool IsDownUp { get; private set; }
        public bool IsDownDown { get; private set; }
        public bool IsDownSpace { get; private set; }
        public bool IsDownN { get; private set; }
        public bool OnReleasedSpace { get; private set; }
        #endregion

        #region Constructeur
        public PlayerControl(Team pParent)
        {
            Parent = pParent;
            _horn = AssetManager.sndHorn;
        }
        #endregion

        public Vector2 CursorPosition(bool pOffensive)
        {
            return Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
        }

        #region Update
        public void Update(bool pRefresh)
        {
            if (pRefresh && !_previousRefreshState)
                _horn.Play();

            OnPressedLeft = Input.OnPressed(Keys.Left) && pRefresh;
            OnPressedRight = Input.OnPressed(Keys.Right) && pRefresh;
            OnPressedUp = Input.OnPressed(Keys.Up) && pRefresh;
            OnPressedDown = Input.OnPressed(Keys.Down) && pRefresh;
            OnPressedSpace = Input.OnPressed(Keys.Space) && pRefresh;
            OnPressedN = Input.OnPressed(Keys.N) && pRefresh;
            IsDownLeft = Input.IsDown(Keys.Left) && pRefresh;
            IsDownRight = Input.IsDown(Keys.Right) && pRefresh;
            IsDownUp = Input.IsDown(Keys.Up) && pRefresh;
            IsDownDown = Input.IsDown(Keys.Down) && pRefresh;
            IsDownSpace = Input.IsDown(Keys.Space) && pRefresh;
            IsDownN = Input.IsDown(Keys.N) && pRefresh;
            OnReleasedSpace = Input.OnReleased(Keys.Space) && pRefresh;
            _previousRefreshState = pRefresh;
        }
        #endregion
    }
}