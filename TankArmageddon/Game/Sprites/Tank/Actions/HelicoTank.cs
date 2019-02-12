using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            #endregion

            #region Constructeur
            public HelicoTank(Tank pParent) { Parent = pParent; }
            #endregion

            #region Fonctions
            public virtual void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                if (Input.IsDown(Keys.Left) && !Parent._onFloor)
                {
                    Parent.Angle -= CANNON_SPEED;
                }

                if (Input.IsDown(Keys.Right) && !Parent._onFloor)
                {
                    Parent.Angle += CANNON_SPEED;
                }

                if (Input.IsDown(Keys.Space) && Parent.Fuel > 0)
                {
                    vx += (float)(Math.Sin(Parent.Angle) * SPEED);
                    vy -= (float)(Math.Cos(Parent.Angle) * SPEED / 2);
                    Parent.Fuel -= FUEL_CONSUMPTION;
                }
                Parent.Parent.RefreshCameraOnSelection();
            }
            #endregion
        }
    }
}
