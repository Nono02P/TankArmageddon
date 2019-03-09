using Microsoft.Xna.Framework;

namespace TankArmageddon
{
    public enum eControlType
    {
        NeuralNetwork,
        Player,
    }

    public interface IControl
    {
        Team Parent { get; }
        bool OnPressedLeft { get; }
        bool OnPressedRight { get; }
        bool OnPressedUp { get; }
        bool OnPressedDown { get; }
        bool OnPressedSpace { get; }
        bool OnPressedN { get; }
        bool IsDownLeft { get; }
        bool IsDownRight { get; }
        bool IsDownUp { get; }
        bool IsDownDown { get; }
        bool IsDownSpace { get; }
        bool IsDownN { get; }
        bool OnReleasedSpace { get; }

        Vector2 CursorPosition(bool pOffensive);
        void Update(bool pRefresh);
    }
}