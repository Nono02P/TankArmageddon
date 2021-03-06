﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class MultipleShootFromTank : NormalMove
        {
            #region Constantes
            private const byte FORCE = 12;
            private const byte NB_SHOOT = 10;
            #endregion

            #region Variables privées
            private SoundEffect _sndShoot;
            private Texture2D _img = AssetManager.TanksSpriteSheet;
            private int _counter = 0;
            private int _timer = 0;
            private int _presetTimer = 300;
            #endregion

            #region Constructeur
            public MultipleShootFromTank(Tank pParent) : base(pParent)
            {
                _sndShoot = AssetManager.sndShoot;
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Enable)
                {
                    if (Control.OnPressedSpace && _counter >= 15) //(Input.OnPressed(Keys.Space) && _counter >= 15)
                    {
                        _counter = 0;
                    }
                    if (Control.IsDownSpace && _counter <= 15) //(Input.IsDown(Keys.Space) && _counter <= 15)
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
                            Vector2 p = new Vector2(Parent._imgCannon.Width * 1.25f * Parent.Scale.X * cosAngle, Parent._imgCannon.Width * 1.25f * Parent.Scale.X * sinAngle);
                            p += Parent._positionCannon;
                            switch (TankArmageddon.Action.GetCategory(Parent.SelectedAction))
                            {
                                case TankArmageddon.Action.eCategory.None:
                                case TankArmageddon.Action.eCategory.Grenada:
                                case TankArmageddon.Action.eCategory.Drop:
                                case TankArmageddon.Action.eCategory.Mine:
                                    break;
                                case TankArmageddon.Action.eCategory.Bullet:
                                    Bullet b = new Bullet(Parent, _img, p, new Vector2(cosAngle * FORCE, sinAngle * FORCE), Parent.SelectedAction, Parent.Scale);
                                    break;
                                default:
                                    break;
                            }
                            _sndShoot.Play(0.05f, 0, 0);
                            BlockAction = true;
                        }
                    }
                    // Si le nombre de tir est atteint
                    if (_counter >= NB_SHOOT)
                    {
                        Parent.Parent.Parent.FinnishTour();
                        Enable = false;
                        _counter = 0;
                        //if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                        //    Parent.Parent.Inventory[Parent.SelectedAction]--;
                    }
                }
            }
            #endregion

            #region Fin de tour
            public override void EndOfTour()
            {
                base.EndOfTour();
                _counter = 0;
            }
            #endregion
        }
    }
}