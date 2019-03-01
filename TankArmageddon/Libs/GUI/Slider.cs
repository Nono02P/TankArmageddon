using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon.GUI
{
    public class Slider : Element
    {
        #region Variables privées
        private Vector2 _barSize = new Vector2();
        private Vector2 _cursorSize = new Vector2();
        private int _value = 50;
        #endregion

        #region Propriétés
        public new Vector2 Position { get { return _position; } set { _position = value; RefreshCursorPosition(); } }

        /// <summary>
        /// Taille de la barre du slider
        /// </summary>
        public Vector2 BarSize { get { return _barSize; } set { _barSize = value; RefreshSize(); } }

        public Vector2 CursorPosition { get; private set; }
        /// <summary>
        /// Taille du curseur du slider
        /// </summary>
        public Vector2 CursorSize
        {
            get { return _cursorSize; }
            set
            {
                if (BarSize.X <= value.X)
                {
                    throw new Exception("Sur un slider, une largeur de curseur doit être plus petite que la largeur de barre.");
                }
                else
                {
                    _cursorSize = value;
                    RefreshSize();
                }
            }
        }
        
        /// <summary>
        /// Valeur du curseur. 
        /// Peut varier de 0 à 100 %
        /// </summary>
        public int Value { get { return _value; } private set { _value = MathHelper.Clamp(value,0,100); RefreshCursorPosition(); } }

        /// <summary>
        /// Objet TextBox intégré au slider.
        /// </summary>
        public Textbox TextBox { get; private set; }
        #endregion

        #region Constructeur
        public Slider(Vector2 pPosition, Vector2 pOrigin, Vector2 pBarSize, Vector2 pCursorSize, bool pVisible = true, float pScale = 1) : 
            base(pPosition, pOrigin, new Vector2(Math.Max(pBarSize.X, pCursorSize.X), Math.Max(pBarSize.Y, pCursorSize.Y)), pVisible, pScale)
        {
            BarSize = pBarSize;
            CursorSize = pCursorSize;
        }
        #endregion

        #region Méthodes

        private void RefreshSize()
        {
            Size = new Vector2(Math.Max(BarSize.X, CursorSize.X), Math.Max(BarSize.Y, CursorSize.Y));
            RefreshCursorPosition();
        }

        private void RefreshCursorPosition()
        {
            CursorPosition = new Vector2(Position.X + (Value * BarSize.X)/100 - CursorSize.X / 2, Position.Y);
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Clicked)
            {
                float x = Mouse.GetState().X - Position.X;
                float y = Mouse.GetState().Y - Position.Y;
                double hyp = utils.MathHypothenus(x, y);
                Value = (int)((x / BarSize.X)*100);
            }
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (Visible)
            {
                Primitives2D.FillRectangle(MainGame.spriteBatch, Position, BarSize, Color.Gray, Angle);
                Primitives2D.FillRectangle(MainGame.spriteBatch, Position, new Vector2((BarSize.X * Value) / 100, BarSize.Y), Color.Blue, Angle);
                Primitives2D.FillRectangle(MainGame.spriteBatch, CursorPosition, CursorSize, Color.White, Angle);
            }
        }
        #endregion

        #endregion
    }
}