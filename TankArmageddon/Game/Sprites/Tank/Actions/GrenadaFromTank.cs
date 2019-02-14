using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class GrenadaFromTank : NormalMove
        {
            #region Constructeur
            public GrenadaFromTank(Tank pParent) : base(pParent) { }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Input.OnPressed(Keys.Space))
                {

                }
            }
            #endregion
        }
    }
}
