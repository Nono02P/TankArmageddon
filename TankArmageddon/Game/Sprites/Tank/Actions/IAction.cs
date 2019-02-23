using Microsoft.Xna.Framework;

namespace TankArmageddon
{
    #region Actions
    public static class Action
    {
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

        public enum eCategory : byte
        {
            None,
            Bullet,
            Grenada,
            Drop,
            Mine,
        }

        public static eCategory GetCategory(eActions action)
        {
            switch (action)
            {
                case eActions.iGrayBullet:
                case eActions.iGrayBombshell:
                case eActions.GoldBullet:
                case eActions.GoldBombshell:
                case eActions.GrayMissile:
                case eActions.GreenMissile:
                    return eCategory.Bullet;
                case eActions.Mine:
                    return eCategory.Mine;
                case eActions.Grenada:
                case eActions.SaintGrenada:
                    return eCategory.Grenada;
                case eActions.iDropFuel:
                    return eCategory.Drop;
                default:
                    return eCategory.None;
            }
        }
    }
    #endregion

    public interface IAction
    {
        Tank Parent { get; }
        bool Enable { get; set; }
        bool BlockAction { get; set; }

        void Update(GameTime gameTime, ref float vx, ref float vy);
        void EndOfTour();
    }
}
