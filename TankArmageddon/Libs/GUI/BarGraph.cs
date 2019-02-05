using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon.GUI
{
    public class BarGraph
    {
        #region Variables privées
        private Vector2 _position = Vector2.Zero;
        private Vector2 _size = Vector2.One;
        private Rectangle _bckgndRect;
        private Rectangle _frontRect;
        private float _value;
        private float _previousValue;
        private float _desiredValue;
        private TimeSpan _currentTimeSpan;
        private TimeSpan _setpointTimeSpan;
        #endregion

        #region Propriétés
        public Vector2 Position { get { return _position; } set { _position = value; RefreshRectangles(); } }
        public Vector2 Size { get { return _size; } set { _size = value; RefreshRectangles(); } }
        public int Thickness { get; set; } = 2;

        public Color BckgndColor { get; set; } = Color.LawnGreen;
        public Color BarColor { get; set; } = Color.Green;
        public float MaxValue { get; private set; }
        public float Value { get { return _value; } set { _value = MathHelper.Clamp(value, 0, MaxValue); RefreshRectangles(); } }
        #endregion

        #region Constructeur
        public BarGraph(int pValue, int pMaxValue, Vector2 pPosition, Vector2 pSize)
        {
            MaxValue = pMaxValue;
            Value = pValue;
            Position = pPosition;
            Size = pSize;
        }

        public BarGraph(int pValue, int pMaxValue, Vector2 pPosition, Vector2 pSize, Color pBckgndColor, Color pBarColor)
        {
            MaxValue = pMaxValue;
            Value = pValue;
            Position = pPosition;
            Size = pSize;
            BckgndColor = pBckgndColor;
            BarColor = pBarColor;
        }
        #endregion

        #region Méthodes

        private void RefreshRectangles()
        {
            _bckgndRect = new Rectangle(Position.ToPoint(), Size.ToPoint());
            _frontRect = new Rectangle(new Point((int)Position.X + Thickness, (int)Position.Y + Thickness), new Point((int)((Size.X - 2 * Thickness) * Value / MaxValue), (int)Size.Y - 2 * Thickness));
        }

        public void SetProgressiveValue(int pDesiredValue, TimeSpan pTimeSpan)
        {
            _desiredValue = pDesiredValue;
            _previousValue = Value;
            _currentTimeSpan = TimeSpan.Zero;
            _setpointTimeSpan = pTimeSpan;
        }

        #region Update
        public void Update(GameTime gameTime)
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
        }
        #endregion

        #region Draw
        public void Draw()
        {
            MainGame.spriteBatch.FillRectangle(_bckgndRect, BckgndColor);
            MainGame.spriteBatch.FillRectangle(_frontRect, BarColor);
        }
        #endregion

        #endregion
    }
}
