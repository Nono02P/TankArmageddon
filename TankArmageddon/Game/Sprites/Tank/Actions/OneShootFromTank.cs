using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class OneShootFromTank : NormalMove
        {
            #region Variables privées
            private SoundEffect _sndShoot;
            #endregion

            #region Propriétés
            public int Force { get; set; }
            public BarGraph LaunchBar { get; private set; }
            #endregion

            #region Constructeur
            public OneShootFromTank(Tank pParent) : base(pParent)
            {
                Texture2D img = AssetManager.TanksSpriteSheet;
                LaunchBar = new BarGraph(0, 100, Parent._positionCannon, Parent._originCannon, Vector2.One, img, img, false);
                LaunchBar.Scale = 0.5f;
                LaunchBar.ImgBoxEmpty = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowEmpty.png").ImgBox;
                LaunchBar.ImgBoxFull = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowFull.png").ImgBox;
                Parent._group.AddElement(LaunchBar);
                _sndShoot = AssetManager.sndShoot;
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);

                if (Enable)
                {
                    if (Input.OnPressed(Keys.Space))
                    {
                        Force = 0;
                    }
                    if (Input.IsDown(Keys.Space))
                    {
                        Force++;
                    }
                    if (Input.OnReleased(Keys.Space) || Force >= 100)
                    {
                        Texture2D img = AssetManager.TanksSpriteSheet;
                        float cosAngle = (float)Math.Cos(Parent.AngleCannon + Parent.Angle);
                        float sinAngle = (float)Math.Sin(Parent.AngleCannon + Parent.Angle);
                        Vector2 p = new Vector2(Parent._imgCannon.Width * Parent.Scale.X * cosAngle, Parent._imgCannon.Width * Parent.Scale.X * sinAngle);
                        p += Parent._positionCannon;
                        switch (Parent.SelectedAction)
                        {
                            case eActions.iGrayBullet:
                            case eActions.iGrayBombshell:
                            case eActions.GoldBullet:
                            case eActions.GoldBombshell:
                                Bullet b = new Bullet(Parent, img, p, new Vector2(cosAngle * Force, sinAngle * Force), Parent.SelectedAction, Parent.Scale);
                                break;
                            case eActions.Grenada:
                            case eActions.SaintGrenada:
                                Grenada g = new Grenada(Parent, img, p, new Vector2(cosAngle * Force, sinAngle * Force), Parent.SelectedAction, Parent.Scale);
                                break;
                            default:
                                break;
                        }
                        
                        _sndShoot.Play();
                        Force = 0;
                        Parent.Parent.Parent.FinnishTour();
                        BlockAction = true;
                        Enable = false;
                        if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                            Parent.Parent.Inventory[Parent.SelectedAction]--;
                    }
                    LaunchBar.Visible = Input.IsDown(Keys.Space) && Force < 100 && Enable;
                    LaunchBar.Value = Force;
                    LaunchBar.Angle = Parent.AngleCannon + Parent.Angle;
                }
            }
            #endregion
        }
    }
}
