using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class ShootFromTank : NormalMove
        {
            #region Propriétés
            public int Force { get; set; }
            public BarGraph LaunchBar { get; private set; }
            #endregion

            #region Constructeur
            public ShootFromTank(Tank pParent) : base(pParent)
            {
                Texture2D img = AssetManager.TanksSpriteSheet;
                LaunchBar = new BarGraph(0, 100, Parent._positionCannon, Parent._originCannon, Vector2.One, img, img, false);
                LaunchBar.Scale = 0.5f;
                LaunchBar.ImgBoxEmpty = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowEmpty.png").ImgBox;
                LaunchBar.ImgBoxFull = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_arrowFull.png").ImgBox;
                Parent._group.AddElement(LaunchBar);
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);
                
                float cosAngle = (float)Math.Cos(Parent.AngleCannon + Parent.Angle);
                float sinAngle = (float)Math.Sin(Parent.AngleCannon + Parent.Angle);
                //Vector2 p = new Vector2(Parent._imgCannon.Width * Parent.Scale.X * cosAngle, Parent._imgCannon.Width * Parent.Scale.X * sinAngle);
                //p += Parent._positionCannon;
                /*Bullet b = new Bullet(this, Image, p, new Vector2(cosAngle * pForce, sinAngle * pForce), pBulletType, Scale);
                */
                if (Input.OnPressed(Keys.Space))
                {
                    Force = 0;
                }
                if (Input.IsDown(Keys.Space))
                {
                    Force++;
                }
                LaunchBar.Visible = Input.IsDown(Keys.Space);
                LaunchBar.Value = Force;
                LaunchBar.Angle = Parent.AngleCannon + Parent.Angle;
                //LaunchBar.Position = p += Parent._positionCannon;
            }
            #endregion
        }
    }
}
