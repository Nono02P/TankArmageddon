using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public partial class Tank
    {
        public class Grenada : Bullet
        {
            public Grenada(Tank pShooter, Texture2D pImage, Vector2 pPosition, Vector2 pVelocity, eActions pBulletType, Vector2 pScale) : base(pShooter, pImage, pPosition, pVelocity, pBulletType, pScale)
            {
            }
        }
    }
}
