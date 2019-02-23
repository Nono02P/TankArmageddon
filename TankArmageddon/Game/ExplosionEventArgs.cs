using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon
{
    public delegate void ExplosionHandler(object sender, ExplosionEventArgs e);

    public class ExplosionEventArgs : EventArgs
    {
        public Circle ExplosionCircle { get; private set; }
        public int Force { get; private set; }

        public ExplosionEventArgs(Vector2 pPosition, int pRadius, int pForce)
        {
            ExplosionCircle = new Circle(pPosition.ToPoint(), pRadius, 0, Color.White);
            Force = pForce;
        }
    }
}