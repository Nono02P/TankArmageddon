using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        iWhiteFlag,
    }
    #endregion

    public interface IAction
    {
        Tank Parent { get; }

        void Update(GameTime gameTime, ref float vx, ref float vy);
    }
}
