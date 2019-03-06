using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon
{
    public static class utils
    {
        #region Variables privées
        private static Random _rnd = new Random();
        #endregion

        #region Méthodes

        #region Collisions
        /// <summary>
        /// Vérifie la collision entre deux acteurs.
        /// </summary>
        public static bool Collide(IActor Actor1, IActor Actor2)
        {
            bool result = false;
            if (Actor1 != Actor2)
            {
                result = Actor1.BoundingBox.Intersects(Actor2.BoundingBox);
            }
            return result;
        }
        #endregion

        #region OutOfScreen
        /// <summary>
        /// Teste si la position est en dehors de l'écran
        /// </summary>
        public static bool OutOfScreen(Vector2 pPosition)
        {
            bool result = false;
            Vector3 pCam = MainGame.Camera.Position;
            Vector2 p = new Vector2(pPosition.X - pCam.X, pPosition.Y - pCam.Y);
            if (p.X < 0 || p.X > MainGame.Screen.Width || 
                p.Y < 0 || p.Y > MainGame.Screen.Height)
            {
                result = true;
            }
            return result;
        }
        public static bool OutOfScreen(MainGame pMainGame, Sprite pSprite)
        {
            bool result = false;
            Vector3 pCam = MainGame.Camera.Position;
            Vector2 Position = pSprite.Position;
            Vector2 p = new Vector2(Position.X - pCam.X, Position.Y - pCam.Y);
            Vector2 Origin = pSprite.Origin;
            if (p.X - Origin.X < 0 || p.X - Origin.X > MainGame.Screen.Width ||
                p.Y < 0 - Origin.Y || p.Y - Origin.Y > MainGame.Screen.Height)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region Math
        /// <summary>
        /// Choisir un chiffre aléatoire entre Min/Max (Max exclu)
        /// </summary>
        public static int MathRnd(int pMin, int pMax)
        {
            return _rnd.Next(pMin, pMax);
        }   

        /// <summary>
        /// Renvoie l'hypothénus à partir des côtés opposé et adjacent
        /// </summary>
        public static double MathHypothenus(float a, float b)
        {
            double A2 = Math.Pow(a, 2);
            double B2 = Math.Pow(b, 2);
            return Math.Sqrt(A2 + B2);
        }

        /// <summary>
        /// Renvoie l'hypothénus à partir des côtés opposé et adjacent provenant d'un Vector2.
        /// </summary>
        public static double MathHypothenus(Vector2 AB)
        {
            return MathHypothenus(AB.X, AB.Y);
        }

        /// <summary>
        /// Renvoie l'angle d'un élément en fonction de sa vélocité
        /// </summary
        public static double MathAngle(Vector2 Velocity)
        {
            return Math.Atan2(Velocity.Y, Velocity.X);
        }

        /// <summary>
        /// Renvoie l'angle entre les coordonnées de 2 éléments
        /// </summary>
        public static double MathAngle(float x1, float y1, float x2, float y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }

        /// <summary>
        /// Renvoie l'angle entre les coordonnées de 2 éléments
        /// </summary>
        public static double MathAngle(Vector2 v1, Vector2 v2)
        {
            return MathAngle(v1.X, v1.Y, v2.X, v2.Y);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 2D
        /// </summary>
        public static double MathDist(float x1, float y1, float x2, float y2)
        {
            double x = Math.Pow((x2 - x1), 2);
            double y = Math.Pow((y2 - y1), 2);
            return Math.Pow((x + y), 0.5f);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 2D
        /// </summary>
        public static double MathDist(Vector2 v1, Vector2 v2)
        {
            return MathDist(v1.X, v1.Y, v2.X, v2.Y);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 3D
        /// </summary>
        public static double MathDist(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            double x = Math.Pow((x2 - x1), 2);
            double y = Math.Pow((y2 - y1), 2);
            double z = Math.Pow((z2 - z1), 2);
            return Math.Pow((x + y + z), 0.5f);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 3D
        /// </summary>
        public static double MathDist(Vector3 v1, Vector3 v2)
        {
            return MathDist(v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z);
        }


        /// <summary>
        /// Effectue une règle de trois et renvois le résultat de la mise à l'échelle de a
        /// </summary>	
	public static double MapValue(double a0, double a1, double b0, double b1, double a, bool pWithClamp = true)
        {
            double val = a;
            if (pWithClamp)
                val = MathHelper.Clamp((float)a, (float)a0, (float)a1);

            return b0 + (b1 - b0) * ((val - a0) / (a1 - a0));
        }
        #endregion

        #endregion
    }
}