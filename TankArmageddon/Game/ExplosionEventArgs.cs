using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon
{
    public delegate void onExplosion(object sender, ExplosionEventArgs e);

    public class ExplosionEventArgs : EventArgs
    {
        public Vector2 Position { get; private set; }
        public int Radius { get; private set; }
        public int Force { get; private set; }

        public ExplosionEventArgs(Vector2 pPosition, int pRadius, int pForce)
        {
            Position = pPosition;
            Radius = pRadius;
            Force = pForce;
        }
    }
}