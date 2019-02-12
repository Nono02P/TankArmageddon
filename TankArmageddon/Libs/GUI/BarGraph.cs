using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankArmageddon.GUI
{
    public class BarGraph : Element
    {
        #region Variables privées
        private Rectangle _bckgndRect;
        private Rectangle _frontRect;
        private float _value;
        private float _previousValue;
        private float _desiredValue;
        private TimeSpan _currentTimeSpan;
        private TimeSpan _setpointTimeSpan;
        #endregion

        #region Propriétés
        public int Thickness { get; set; } = 2;
        public Color BckgndColor { get; set; } = Color.LawnGreen;
        public Color BarColor { get; set; } = Color.Green;
        public float MaxValue { get; private set; }
        public float Value { get { return _value; } set { _value = MathHelper.Clamp(value, 0, MaxValue); RefreshRectangles(); } }
        public Texture2D ImageEmpty { get; private set; } = null;
        public Rectangle? ImgBoxEmpty { get; set; } = null;
        public Texture2D ImageFull { get; private set; } = null;
        public Rectangle? ImgBoxFull { get; set; } = null;
        public SpriteEffects SpriteEffects { get; set; }
        #endregion

        #region Constructeur
        public BarGraph(float pValue, float pMaxValue, Vector2 pPosition, Vector2 pOrigin, Vector2 pSize, bool pVisible = true) : base(pPosition, pOrigin, pSize, pVisible)
        {
            MaxValue = pMaxValue;
            Value = pValue;
        }

        public BarGraph(float pValue, float pMaxValue, Vector2 pPosition, Vector2 pOrigin, Vector2 pSize, Color pBckgndColor, Color pBarColor, bool pVisible = true) : base(pPosition, pOrigin, pSize, pVisible)
        {
            MaxValue = pMaxValue;
            Value = pValue;
            BckgndColor = pBckgndColor;
            BarColor = pBarColor;
            OnPositionChange += RectangleChanged;
            OnSizeChange += RectangleChanged;
            OnOriginChange += RectangleChanged;
        }

        public BarGraph(float pValue, float pMaxValue, Vector2 pPosition, Vector2 pOrigin, Vector2 pSize, Texture2D pImageEmpty, Texture2D pImageFull, bool pVisible = true) : base(pPosition, pOrigin, pSize, pVisible)
        {
            MaxValue = pMaxValue;
            Value = pValue;
            ImageEmpty = pImageEmpty;
            ImageFull = pImageFull;
            OnPositionChange += RectangleChanged;
            OnSizeChange += RectangleChanged;
            OnOriginChange += RectangleChanged;
        }
        #endregion

        #region Méthodes
        private void RefreshRectangles()
        {
            _bckgndRect = new Rectangle((Position - Origin).ToPoint(), Size.ToPoint());
            _frontRect = new Rectangle(new Point((int)(Position.X - Origin.X) + Thickness, (int)(Position.Y - Origin.Y) + Thickness), new Point((int)((Size.X - 2 * Thickness) * Value / MaxValue), (int)Size.Y - 2 * Thickness));
        }

        public void SetProgressiveValue(float pDesiredValue, TimeSpan pTimeSpan)
        {
            _desiredValue = pDesiredValue;
            _previousValue = Value;
            _currentTimeSpan = TimeSpan.Zero;
            _setpointTimeSpan = pTimeSpan;
        }

        private void RectangleChanged(object sender, Vector2 before, Vector2 actual)
        {
            RefreshRectangles();
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (_currentTimeSpan < _setpointTimeSpan)
            {
                _currentTimeSpan += gameTime.ElapsedGameTime;
                Value = (float)(_previousValue - (_previousValue - _desiredValue) * _currentTimeSpan.TotalMilliseconds / _setpointTimeSpan.TotalMilliseconds);
            }
            else if (_currentTimeSpan >= _setpointTimeSpan && _setpointTimeSpan != TimeSpan.Zero)
            {
                _currentTimeSpan = TimeSpan.Zero;
                _setpointTimeSpan = TimeSpan.Zero;
                Value = _desiredValue;
            }
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Visible)
            {
                if (ImageEmpty != null && ImageFull != null)
                {
                    spriteBatch.Draw(ImageEmpty, new Rectangle((Position - Origin).ToPoint(), (ImgBoxEmpty.Value.Size.ToVector2() * Scale).ToPoint()), ImgBoxEmpty, Color.White, Angle, Origin, SpriteEffects, 0);
                    Vector2 s = new Vector2((int)(ImgBoxFull.Value.Size.X * Value / MaxValue), (int)(ImgBoxFull.Value.Size.Y));
                    spriteBatch.Draw(ImageFull, new Rectangle((Position - Origin).ToPoint(), (s * Scale).ToPoint()), new Rectangle(ImgBoxFull.Value.Location, s.ToPoint()), Color.White, Angle, Origin, SpriteEffects, 0);
                }
                else
                {
                    spriteBatch.FillRectangle(_bckgndRect, BckgndColor);
                    spriteBatch.FillRectangle(_frontRect, BarColor);
                }
            }
            base.Draw(spriteBatch, gameTime);
        }
        #endregion

        #endregion
    }
}
