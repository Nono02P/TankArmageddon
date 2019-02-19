using Microsoft.Xna.Framework;

namespace TankArmageddon
{
    #region Enumérations
    public enum eActions : byte
    {
        None,
        iGrayBullet,
        iGrayBombshell,
        GoldBullet,
        GoldBombshell,
        GrayMissile,
        GreenMissile,
        Mine,
        Grenada,
        SaintGrenada,
        iTankBaseBall,
        HelicoTank,
        Drilling,
        iDropFuel,
    }
    #endregion

    public interface IAction
    {
        Tank Parent { get; }
        bool Enable { get; set; }
        bool BlockAction { get; set; }

        void Update(GameTime gameTime, ref float vx, ref float vy);
    }
}
