using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class OneShootFromTank : NormalMove
        {
            #region Constantes
            private const byte FORCE_MAX = 13;
            private const byte FORCE_MIN = 1;
            private const float FORCE_SPEED = 0.5f;
            #endregion

            #region Variables privées
            private SoundEffect _sndShoot;
            private SoundEffect _sndSaintGrenada;
            #endregion

            #region Propriétés
            public float Force { get; set; }
            public BarGraph LaunchBar { get; private set; }
            #endregion

            #region Constructeur
            public OneShootFromTank(Tank pParent) : base(pParent)
            {
                Texture2D img = AssetManager.TanksSpriteSheet;
                LaunchBar = new BarGraph(0, FORCE_MIN, FORCE_MAX, Parent._positionCannon, Parent._originCannon, Vector2.One, img, img, false);
                LaunchBar.Scale = 0.5f;
                LaunchBar.ImgBoxEmpty = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowEmpty.png").ImgBox;
                LaunchBar.ImgBoxFull = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowFull.png").ImgBox;
                Parent._group.AddElement(LaunchBar);
                _sndShoot = AssetManager.sndShoot;
                _sndSaintGrenada = AssetManager.sndSaintGrenada;
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);
                if (Enable)
                {
                    if (Control.OnPressedSpace) //(Input.OnPressed(Keys.Space))
                    {
                        Force = FORCE_MIN;
                    }
                    if (Control.IsDownSpace) //(Input.IsDown(Keys.Space))
                    {
                        Force += FORCE_SPEED;
                    }
                    if (Control.OnReleasedSpace || Force >= FORCE_MAX) //(Input.OnReleased(Keys.Space) || Force >= FORCE_MAX)
                    {
                        Texture2D img = AssetManager.TanksSpriteSheet;
                        float cosAngle = (float)Math.Cos(Parent.AngleCannon + Parent.Angle);
                        float sinAngle = (float)Math.Sin(Parent.AngleCannon + Parent.Angle);
                        Vector2 p = new Vector2(Parent._imgCannon.Width * 1.25f * Parent.Scale.X * cosAngle, Parent._imgCannon.Width * 1.25f * Parent.Scale.X * sinAngle);
                        p += Parent._positionCannon;
                        switch (TankArmageddon.Action.GetCategory(Parent.SelectedAction))
                        {
                            case TankArmageddon.Action.eCategory.None:
                            case TankArmageddon.Action.eCategory.Drop:
                            case TankArmageddon.Action.eCategory.Mine:
                                break;
                            case TankArmageddon.Action.eCategory.Bullet:
                                Bullet b = new Bullet(Parent, img, p, new Vector2(cosAngle * Force, sinAngle * Force), Parent.SelectedAction, Parent.Scale);
                                b.FocusCamera = true;
                                break;
                            case TankArmageddon.Action.eCategory.Grenada:
                                Grenada g = new Grenada(Parent, p, new Vector2(cosAngle * Force, sinAngle * Force), Parent.SelectedAction);
                                g.FocusCamera = true;
                                break;
                            default:
                                break;
                        }
                        if (Parent.SelectedAction == TankArmageddon.Action.eActions.SaintGrenada)
                        {
                            _sndSaintGrenada.Play();
                        }
                        else
                        {
                            _sndShoot.Play();
                        }
                        Force = FORCE_MIN;
                        Parent.Parent.Parent.FinnishTour();
                        BlockAction = true;
                        Enable = false;
                        //if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                        //    Parent.Parent.Inventory[Parent.SelectedAction]--;
                    }
                    LaunchBar.Visible = Control.IsDownSpace && Force < 100 && Enable; //Input.IsDown(Keys.Space) && Force < 100 && Enable;
                    LaunchBar.Value = Force;
                    LaunchBar.Angle = Parent.AngleCannon + Parent.Angle;
                }
            }
            #endregion

            #region Fin de tour
            public override void EndOfTour()
            {
                base.EndOfTour();
                LaunchBar.Visible = false;
            }
            #endregion
        }
    }
}