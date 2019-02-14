﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class MultipleShootFromTank : NormalMove
        {
            #region Constantes
            private float FORCE = 12; 
            #endregion

            #region Variables privées
            private Texture2D _img = AssetManager.TanksSpriteSheet;
            private int _counter = 0;
            private int _timer = 0;
            private int _presetTimer = 300;
            #endregion

            #region Constructeur
            public MultipleShootFromTank(Tank pParent) : base(pParent) { }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Enable)
                {
                    if (Input.OnPressed(Keys.Space) && _counter >= 15)
                    {
                        _counter = 0;
                    }
                    if (Input.IsDown(Keys.Space) && _counter <= 15)
                    {
                        if (_timer > 0)
                        {
                            _timer -= gameTime.ElapsedGameTime.Milliseconds;
                        }
                        else
                        {
                            _timer = _presetTimer;
                            _counter++;
                            float cosAngle = (float)Math.Cos(Parent.AngleCannon + Parent.Angle);
                            float sinAngle = (float)Math.Sin(Parent.AngleCannon + Parent.Angle);
                            Vector2 p = new Vector2(Parent._imgCannon.Width * Parent.Scale.X * cosAngle, Parent._imgCannon.Width * Parent.Scale.X * sinAngle);
                            p += Parent._positionCannon;
                            Bullet b = new Bullet(Parent, _img, p, new Vector2(cosAngle * FORCE, sinAngle * FORCE), Parent.SelectedAction, Parent.Scale);
                        }
                    }
                    if (_counter >= 15)
                    {
                        Parent.Parent.Parent.FinnishTour();
                        Enable = false;
                    }
                }
            }
            #endregion
        }
    }
}