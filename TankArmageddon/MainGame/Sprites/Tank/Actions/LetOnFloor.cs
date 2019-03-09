using Microsoft.Xna.Framework;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class LetOnFloor : NormalMove
        {
            #region Constructeur
            public LetOnFloor(Tank pParent) : base(pParent) { }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Enable)
                {
                    if (Control.OnPressedSpace) //(Input.OnPressed(Keys.Space))
                    {
                        switch (TankArmageddon.Action.GetCategory(Parent.SelectedAction))
                        {
                            case TankArmageddon.Action.eCategory.None:
                            case TankArmageddon.Action.eCategory.Bullet:
                            case TankArmageddon.Action.eCategory.Grenada:
                            case TankArmageddon.Action.eCategory.Drop:
                                break;
                            case TankArmageddon.Action.eCategory.Mine:
                                Mine m = new Mine(Parent, Parent.Position);
                                m.Angle = Parent.Angle;
                                Parent.Parent.Parent.FinnishTour();
                                BlockAction = true;
                                Enable = false;
                                //if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                                //    Parent.Parent.Inventory[Parent.SelectedAction]--;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion
        }
    }
}