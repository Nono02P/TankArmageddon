using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                    if (Input.OnPressed(Keys.Space))
                    {
                        switch (Action.GetCategory(Parent.SelectedAction))
                        {
                            case Action.eCategory.None:
                            case Action.eCategory.Bullet:
                            case Action.eCategory.Grenada:
                            case Action.eCategory.Drop:
                                break;
                            case Action.eCategory.Mine:
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