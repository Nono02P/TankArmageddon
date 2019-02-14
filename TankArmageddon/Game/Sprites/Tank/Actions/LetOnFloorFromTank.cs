using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                }
            }
            #endregion
        }
    }
}
