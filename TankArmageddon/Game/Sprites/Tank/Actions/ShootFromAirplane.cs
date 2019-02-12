using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class ShootFromAirplane : NormalMove
        {
            public Image MyProperty { get; set; }

            public ShootFromAirplane(Tank pParent) : base(pParent) { }

            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);
                if (Parent.SelectedAction == eActions.iDropFuel)
                {

                }
                else
                {

                }
            }
        }
    }
}