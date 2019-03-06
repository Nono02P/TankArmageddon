using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public class PlayerControl : IControl
    {
        #region Propriétés
        public Team Parent { get; private set; }
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
        }
        #endregion

        public Vector2 CursorPosition(bool pOffensive)
        {
            return Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
        }

        #region Update
        public void Update(bool pRefresh)
        {
            OnPressedLeft = Input.OnPressed(Keys.Left);
            OnPressedRight = Input.OnPressed(Keys.Right);
            OnPressedUp = Input.OnPressed(Keys.Up);
            OnPressedDown = Input.OnPressed(Keys.Down);
            OnPressedSpace = Input.OnPressed(Keys.Space);
            OnPressedN = Input.OnPressed(Keys.N);
            IsDownLeft = Input.IsDown(Keys.Left);
            IsDownRight = Input.IsDown(Keys.Right);
            IsDownUp = Input.IsDown(Keys.Up);
            IsDownDown = Input.IsDown(Keys.Down);
            IsDownSpace = Input.IsDown(Keys.Space);
            IsDownN = Input.IsDown(Keys.N);
            OnReleasedSpace = Input.OnReleased(Keys.Space);
        }
        #endregion
    }
}