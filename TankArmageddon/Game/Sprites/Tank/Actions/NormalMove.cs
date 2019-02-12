﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class NormalMove : IAction
        {
            #region Propriétés
            public Tank Parent { get; private set; }
            #endregion

            #region Constructeur
            public NormalMove(Tank pParent) { Parent = pParent; }
            #endregion

            #region Update
            public virtual void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                #region Contrôles du canon
                if (Parent.Up && Parent._direction == eDirection.Right || Parent.Down && Parent._direction == eDirection.Left)
                {
                    Parent.AngleCannon -= CANNON_SPEED;
                }
                if (Parent.Down && Parent._direction == eDirection.Right || Parent.Up && Parent._direction == eDirection.Left)
                {
                    Parent.AngleCannon += CANNON_SPEED;
                }
                Parent.AngleCannon = MathHelper.Clamp(Parent.AngleCannon, Parent._minCannonAngle, Parent._maxCannonAngle);
                #endregion

                #region Contrôles des déplacements
                float xSpeed = (float)Math.Cos(Parent.Angle) * SPEED;

                if (Parent.Left && Parent._onFloor)
                {
                    if (Parent.Fuel > 0)
                    {
                        vx -= xSpeed;
                        Parent.Fuel -= FUEL_CONSUMPTION;
                        Parent.Parent.RefreshCameraOnSelection();
                    }
                    Parent.Effects = SpriteEffects.FlipHorizontally;
                }
                if (Parent.Right && Parent._onFloor)
                {
                    if (Parent.Fuel > 0)
                    {
                        vx += xSpeed;
                        Parent.Fuel -= FUEL_CONSUMPTION;
                        Parent.Parent.RefreshCameraOnSelection();
                    }
                    Parent.Effects = SpriteEffects.None;
                }
                #endregion
            }
            #endregion
        }
    }
}
