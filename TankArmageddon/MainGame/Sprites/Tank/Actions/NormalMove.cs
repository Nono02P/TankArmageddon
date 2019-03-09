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
            private bool _fuelEmpty = false;
            private bool _tankAlreadyMoved = false;
            private bool _cannonAlreadyMoved = false;
            #endregion

            #region Propriétés
            public Tank Parent { get; private set; }
            public IControl Control { get; private set; }
            public bool Enable { get; set; }
            public bool BlockAction
            {
                get => _blockAction;
                set
                {
                    if (_blockAction != value)
                    {
                        _blockAction = value;
                        if (Control is NeuralNetworkControl)
                        {
                            ((NeuralNetworkControl)Control).Genome.FitnessScore += NeuralNetworkControl.BonusShoot;
                        }
                        if (Parent.SelectedAction != TankArmageddon.Action.eActions.None)
                        {
                            if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                            {
                                Parent.Parent.Inventory[Parent.SelectedAction]--;
                                Parent.Parent.Parent.RefreshActionButtonInventory();
                            }
                        }
                    }
                }
            }
            #endregion

            #region Constructeur
            public NormalMove(Tank pParent)
            {
                Parent = pParent;
                Control = Parent.Parent.Control;
            }
            #endregion

            #region Update
            public virtual void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                #region Contrôles du canon
                if (Parent.Up && Parent._direction == eDirection.Right || Parent.Down && Parent._direction == eDirection.Left)
                {
                    Parent.AngleCannon -= SPEED_ROTATION;
                    if (!_cannonAlreadyMoved && Control is NeuralNetworkControl)
                    {
                        ((NeuralNetworkControl)Control).Genome.FitnessScore += NeuralNetworkControl.BonusCannonMove;
                        _cannonAlreadyMoved = true;
                    }
                }
                if (Parent.Down && Parent._direction == eDirection.Right || Parent.Up && Parent._direction == eDirection.Left)
                {
                    Parent.AngleCannon += SPEED_ROTATION;
                    if (!_cannonAlreadyMoved && Control is NeuralNetworkControl)
                    {
                        ((NeuralNetworkControl)Control).Genome.FitnessScore += NeuralNetworkControl.BonusCannonMove;
                        _cannonAlreadyMoved = true;
                    }
                }
                Parent.AngleCannon = MathHelper.Clamp(Parent.AngleCannon, Parent._minCannonAngle, Parent._maxCannonAngle);
                #endregion

                #region Contrôles des déplacements
                float xSpeed = (float)Math.Cos(Parent.Angle) * SPEED;

                if (Parent.Left && !Parent.Right && Parent._onFloor)
                {
                    if (Parent.Fuel > 0)
                    {
                        vx -= xSpeed;
                        Parent.Fuel -= FUEL_CONSUMPTION;
                        Parent.Parent.RefreshCameraOnSelection();
                        _fuelEmpty = false;
                        if (!_tankAlreadyMoved && Control is NeuralNetworkControl)
                        {
                            ((NeuralNetworkControl)Control).Genome.FitnessScore += NeuralNetworkControl.BonusTankMove;
                            _tankAlreadyMoved = true;
                        }
                    }
                    else
                    {
                        if (!_fuelEmpty && Control is NeuralNetworkControl)
                        {
                            ((NeuralNetworkControl)Control).Genome.FitnessScore -= NeuralNetworkControl.MalusFuelEmpty;
                            _fuelEmpty = true;
                        }
                    }
                    Parent.Effects = SpriteEffects.FlipHorizontally;
                }
                if (Parent.Right && !Parent.Left && Parent._onFloor)
                {
                    if (Parent.Fuel > 0)
                    {
                        vx += xSpeed;
                        Parent.Fuel -= FUEL_CONSUMPTION;
                        Parent.Parent.RefreshCameraOnSelection();
                    }
                    else
                    {
                        if (!_fuelEmpty && Control is NeuralNetworkControl)
                        {
                            ((NeuralNetworkControl)Control).Genome.FitnessScore -= NeuralNetworkControl.MalusFuelEmpty;
                            _fuelEmpty = true;
                        }
                    }
                    Parent.Effects = SpriteEffects.None;
                }
                #endregion
            }
            #endregion

            public virtual void BeforeActionChange() { Enable = false; }

            #region Fin du tour
            public virtual void EndOfTour()
            {
                Parent.SelectedAction = TankArmageddon.Action.eActions.None;
            }
            #endregion
        }
    }
}