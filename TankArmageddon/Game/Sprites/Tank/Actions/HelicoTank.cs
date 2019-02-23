using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class HelicoTank : IAction
        {
            #region Propriétés
            public Tank Parent { get; private set; }
            public bool Enable { get; set; }
            public bool BlockAction { get; set; }
            #endregion

            #region Constructeur
            public HelicoTank(Tank pParent) { Parent = pParent; }
            #endregion

            #region Update
            public virtual void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                if (Input.IsDown(Keys.Left) && !Parent._onFloor)
                {
                    Parent.Angle -= SPEED_ROTATION;
                }

                if (Input.IsDown(Keys.Right) && !Parent._onFloor)
                {
                    Parent.Angle += SPEED_ROTATION;
                }

                if (Input.IsDown(Keys.Space) && Parent.Fuel > 0)
                {
                    vx += (float)(Math.Sin(Parent.Angle) * SPEED);
                    vy -= (float)(Math.Cos(Parent.Angle) * SPEED);
                    Parent.Fuel -= FUEL_CONSUMPTION;
                }
                Parent.Parent.RefreshCameraOnSelection();
            }
            #endregion

            #region Fin du tour
            public virtual void EndOfTour()
            {
                Parent.SelectedAction = Action.eActions.None;
            }
            #endregion
        }
    }
}
