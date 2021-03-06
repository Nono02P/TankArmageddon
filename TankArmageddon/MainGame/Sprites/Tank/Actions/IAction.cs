﻿using Microsoft.Xna.Framework;

namespace TankArmageddon
{
    public interface IAction
    {
        #region Propriétés
        Tank Parent { get; }
        IControl Control { get; }
        bool Enable { get; set; }
        bool BlockAction { get; set; }
        #endregion

        #region Méthodes
        void Update(GameTime gameTime, ref float vx, ref float vy);
        void BeforeActionChange();
        void EndOfTour();
        #endregion
    }
}