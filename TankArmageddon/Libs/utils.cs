﻿using Microsoft.Xna.Framework;
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

            if (pPosition.X < 0 || pPosition.X > MainGame.Screen.Width || 
                pPosition.Y < 0 || pPosition.Y > MainGame.Screen.Height)
            {
                result = true;
            }
            return result;
        }
        public static bool OutOfScreen(MainGame pMainGame, Sprite pSprite)
        {
            bool result = false;
            Vector2 Position = pSprite.Position;
            Vector2 Origin = pSprite.Origin;
            if (Position.X - Origin.X < 0 || Position.X - Origin.X > MainGame.Screen.Width ||
                Position.Y < 0 - Origin.Y || Position.Y - Origin.Y > MainGame.Screen.Height)
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
        public static double MathDist(int x1, int y1, int x2, int y2)
        {
            int x = (x2 - x1) ^ 2;
            int y = (y2 - y1) ^ 2;
            return Math.Pow((x + y), 0.5f);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 2D
        /// </summary>
        public static double MathDist(Vector2 v1, Vector2 v2)
        {
            return MathDist((int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 3D
        /// </summary>
        public static double MathDist(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int x = (x2 - x1) ^ 2;
            int y = (y2 - y1) ^ 2;
            int z = (z2 - z1) ^ 2;
            return Math.Pow((x + y + z), 0.5f);
        }

        /// <summary>
        /// Renvoie la distance entre les coordonnées de 2 éléments 3D
        /// </summary>
        public static double MathDist(Vector3 v1, Vector3 v2)
        {
            return MathDist((int)v1.X, (int)v1.Y, (int)v1.Z, (int)v2.X, (int)v2.Y, (int)v2.Z);
        }
        #endregion

        #endregion
    }
}
