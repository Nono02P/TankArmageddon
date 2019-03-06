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
            #region Propriétés
            public Image ShootCursor { get; set; }
            #endregion

            #region Constructeur
            public OneShootFromAirplane(Tank pParent) : base(pParent)
            {
                Vector2 pos = Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
                Texture2D img = AssetManager.Crosshair;
                ShootCursor = new Image(img, pos, new Vector2(img.Width / 2, img.Height / 2), false);
            }
            #endregion

            #region Update
            public override void Update(GameTime gameTime, ref float vx, ref float vy)
            {
                base.Update(gameTime, ref vx, ref vy);
                if (Enable)
                {
                    ShootCursor.Visible = true;
                    Action.eCategory category = Action.GetCategory(Parent.SelectedAction);
                    ShootCursor.Position = Control.CursorPosition(category != Action.eCategory.Drop); //Mouse.GetState().Position.ToVector2() + new Vector2(MainGame.Camera.Position.X, MainGame.Camera.Position.Y);
                    if (Control.OnPressedSpace) //(Input.OnPressed(Keys.Space))
                    {
                        switch (category)
                        {
                            case Action.eCategory.None:
                            case Action.eCategory.Grenada:
                            case Action.eCategory.Mine:
                                break;
                            case Action.eCategory.Bullet:
                                Bullet b = new Bullet(Parent, AssetManager.TanksSpriteSheet, new Vector2(ShootCursor.Position.X, 0), Vector2.Zero, Parent.SelectedAction, Vector2.One);
                                break;
                            case Action.eCategory.Drop:
                                Drop d;
                                switch (Parent.SelectedAction)
                                {
                                    case Action.eActions.DropHealth:
                                        d = new Drop(Parent.Parent.Parent, Drop.eDropType.Health, AssetManager.TanksSpriteSheet, new Vector2(ShootCursor.Position.X, 0), Vector2.Zero, Vector2.One);
                                        break;
                                    case Action.eActions.iDropFuel:
                                        d = new Drop(Parent.Parent.Parent, Drop.eDropType.Fuel, AssetManager.TanksSpriteSheet, new Vector2(ShootCursor.Position.X, 0), Vector2.Zero, Vector2.One);
                                        d.Value = (int)Parent.Fuel / 2;
                                        Parent.Fuel -= d.Value;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        Parent.Parent.Parent.FinnishTour();
                        ShootCursor.Visible = false;
                        Enable = false;
                        //if (Parent.Parent.Inventory[Parent.SelectedAction] > 0)
                        //    Parent.Parent.Inventory[Parent.SelectedAction]--;
                    }
                }
            }
            #endregion

            #region Avant le passage sur une autre action
            public override void BeforeActionChange()
            {
                ShootCursor.Visible = false;
                base.BeforeActionChange();                
            }
            #endregion

            #region Fin de tour
            public override void EndOfTour()
            {
                base.EndOfTour();
                ShootCursor.Visible = false;
            }
            #endregion
        }
    }
}