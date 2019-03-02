namespace TankArmageddon
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
            iMine,
            Grenada,
            SaintGrenada,
            //iTankBaseBall,
            HelicoTank,
            Drilling,
            DropHealth,
            iDropFuel,
        }

        public enum eCategory : byte
        {
            None,
            Bullet,
            Grenada,
            Drop,
            Mine,
            Drill,
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
                case eActions.iMine:
                    return eCategory.Mine;
                case eActions.Grenada:
                case eActions.SaintGrenada:
                    return eCategory.Grenada;
                case eActions.iDropFuel:
                case eActions.DropHealth:
                    return eCategory.Drop;
                case eActions.Drilling:
                    return eCategory.Drill;
                default:
                    return eCategory.None;
            }
        }
        #endregion
    }
}