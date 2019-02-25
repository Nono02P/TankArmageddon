﻿namespace TankArmageddon
{
    public static class Action
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

        public enum eCategory : byte
        {
            None,
            Bullet,
            Grenada,
            Drop,
            Mine,
        }
        #endregion

        #region Catégories
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
        #endregion
    }
}
