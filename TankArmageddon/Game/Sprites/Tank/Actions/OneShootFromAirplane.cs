using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank
    {
        private class OneShootFromAirplane : NormalMove
        {
            public Image ShootCursor { get; set; }

            public OneShootFromAirplane(Tank pParent) : base(pParent)
            {
                Vector2 pos = Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
                Texture2D img = AssetManager.Crosshair;
                ShootCursor = new Image(img, pos, new Vector2(img.Width / 2, img.Height / 2), false);
            }

            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);
                if (Enable)
                {
                    ShootCursor.Visible = true;
                    ShootCursor.Position = Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
                    if (Input.OnPressed(Keys.Space))
                    {
                        if (Parent.SelectedAction == eActions.iDropFuel)
                        {
                            Drop d = new Drop(Parent.Parent.Parent, Drop.eDropType.Fuel, AssetManager.TanksSpriteSheet, new Vector2(ShootCursor.Position.X, 0), Vector2.Zero, Vector2.One);
                            d.Value = (int)Parent.Fuel / 2;
                            Parent.Fuel -= d.Value;
                        }
                        else
                        {
                            Bullet b = new Bullet(Parent, AssetManager.TanksSpriteSheet, new Vector2(ShootCursor.Position.X, 0), Vector2.Zero, Parent.SelectedAction, Vector2.One);
                        }
                        Parent.Parent.Parent.FinnishTour();
                        ShootCursor.Visible = false;
                        Enable = false;
                        if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                            Parent.Parent.Inventory[Parent.SelectedAction]--;
                    }
                }
            }

            public override void EndOfTour()
            {
                base.EndOfTour();
                ShootCursor.Visible = false;
            }
        }
    }
}