using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class LetOnFloorFromTank : NormalMove
        {
            #region Constructeur
            public LetOnFloorFromTank(Tank pParent) : base(pParent) { }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Input.OnPressed(Keys.Space))
                {
                    if (Parent.SelectedAction == eActions.Mine)
                    {
                        Mine m = new Mine(Parent, Parent.Position);
                        m.Angle = Parent.Angle;
                        Parent.Parent.Parent.FinnishTour();
                        Enable = false;
                        BlockAction = true;
                    }
                }
            }
            #endregion
        }
    }
}
