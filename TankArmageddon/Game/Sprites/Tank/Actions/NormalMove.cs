using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class NormalMove : IAction
        {
            #region Variables privées
            private bool _blockAction;
            #endregion

            #region Propriétés
            public Tank Parent { get; private set; }
            public bool Enable { get; set; }
            public bool BlockAction { get => _blockAction; set { if (_blockAction != value) { _blockAction = value; if (Parent.Parent.Inventory[Parent.SelectedAction] > 0) { Parent.Parent.Inventory[Parent.SelectedAction]--; Parent.Parent.Parent.RefreshActionButton(); } } } }
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
                    Parent.AngleCannon -= SPEED_ROTATION;
                }
                if (Parent.Down && Parent._direction == eDirection.Right || Parent.Up && Parent._direction == eDirection.Left)
                {
                    Parent.AngleCannon += SPEED_ROTATION;
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

            #region Fin du tour
            public virtual void EndOfTour()
            {
                Parent.SelectedAction = Action.eActions.None;
            }
            #endregion
        }
    }
}