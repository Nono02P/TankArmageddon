using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Camera
    {
        #region Evènements
        public event onVector3Change OnPositionChange;
        #endregion

        #region Variables privées
        private Vector3 _position;
        #endregion

        #region Propriétés
        public bool LimitOnLeft { get; set; } = true;
        public bool LimitOnRight { get; set; } = true;
        public bool LimitOnTop { get; set; } = true;
        public bool LimitOnBottom { get; set; } = true;
        public bool MouseFollowOnLeft { get; set; } = true;
        public bool MouseFollowOnRight { get; set; } = true;
        public bool MouseFollowOnTop { get; set; } = true;
        public bool MouseFollowOnBottom { get; set; } = true;
        public int Speed { get; set; }
        public Viewport Screen { get; set; }
        public Vector3 CameraSize { get; set; }
        public Vector3 CameraOffset { get; set; }
        public Vector3 Origin { get; set; }
        public float Angle { get; set; }
        public float Zoom { get; set; }
        public bool Enable { get; set; }
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                Vector3 limitedValue = LimitPosition(value);
                if (_position != limitedValue)
                {
                    OnPositionChange?.Invoke(this, _position, limitedValue);
                    _position = limitedValue;
                }
            }
        }
        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(-Position + CameraOffset)
                    * Matrix.CreateScale(Zoom, Zoom, 1f)
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(Angle))
                    * Matrix.CreateTranslation(Origin);
            }
        }
        #endregion

        #region Constructeur
        public Camera(Viewport pScreen, Vector3 pCameraSize, float pZoom = 1, int pSpeed = 0)
        {
            Screen = pScreen;
            CameraSize = pCameraSize;
            CameraOffset = new Vector3();
            Position = new Vector3();
            Origin = new Vector3();
            Zoom = pZoom;
            Speed = pSpeed;
        }

        public Camera(Viewport pScreen, Vector3 pPosition, Vector3 pOrigin, float pAngle, float pZoom = 1, int pSpeed = 0)
        {
            Screen = pScreen;
            Position = pPosition;
            Origin = pOrigin;
            Angle = pAngle;
            Zoom = pZoom;
            Speed = pSpeed;
        }
        #endregion

        private Vector3 LimitPosition(Vector3 pos)
        {
            float x = pos.X;
            float y = pos.Y;
            float z = pos.Z;

            if (LimitOnLeft && x < 0)
            {
                x = 0;
            }
            if (LimitOnRight && x > CameraSize.X - Screen.Width)
            {
                x = CameraSize.X - Screen.Width;
            }
            if (LimitOnTop && y < 0)
            {
                y = 0;
            }
            if (LimitOnBottom && y > CameraSize.Y - Screen.Height)
            {
                y = CameraSize.Y - Screen.Height;
            }
            return new Vector3(x, y, z);
        }

        public Vector2 ScreenToWorld(Vector2 pos)
        {
            return Vector2.Transform(pos, Matrix.Invert(Transformation));
        }

        public Vector2 WorldToScreen(Vector2 pos)
        {
            return Vector2.Transform(pos, Transformation);
        }

        public void SetCameraOnActor(IActor actor, bool xCenter = true, bool yCenter = true)
        {
            HAlign hAlign = HAlign.None;
            VAlign vAlign = VAlign.None;
            if (xCenter)
                hAlign = HAlign.Center;

            if (yCenter)
                vAlign = VAlign.Middle;
            SetCameraOnActor(actor, hAlign, vAlign);
        }

        public void SetCameraOnActor(IActor actor, HAlign hAlign = HAlign.Center, VAlign vAlign = VAlign.Middle)
        {
            float x = Position.X;
            float y = Position.Y;
            float z = Position.Z;
            switch (hAlign)
            {
                case HAlign.Left:
                    x = (int)actor.Position.X;
                    break;
                case HAlign.Center:
                    x = (int)(actor.Position.X - Screen.Width / 2);
                    break;
                case HAlign.Right:
                    x = (int)(actor.Position.X - actor.BoundingBox.Width + Screen.Width);
                    break;
                default:
                    break;
            }
            switch (vAlign)
            {
                case VAlign.Top:
                    y = (int)actor.Position.Y;
                    break;
                case VAlign.Middle:
                    y = (int)(actor.Position.Y - Screen.Height / 2);
                    break;
                case VAlign.Bottom:
                    y = (int)(actor.Position.Y - actor.BoundingBox.Height + Screen.Height);
                    break;
                default:
                    break;
            }
            Position = new Vector3(x, y, z);
        }

        #region Update
        public void Update()
        {
            if (Enable)
            {
                Point mousePosition = Mouse.GetState().Position;
                int camX = (int)Position.X;
                int camY = (int)Position.Y;
                if (MouseFollowOnRight && mousePosition.X >= 0.95f * Screen.Width)
                {
                    camX += Speed;
                }
                if (MouseFollowOnLeft && mousePosition.X <= 0.05f * Screen.Width)
                {
                    camX -= Speed;
                }
                if (MouseFollowOnBottom && mousePosition.Y >= 0.95f * Screen.Height)
                {
                    camY += Speed;
                }
                if (MouseFollowOnTop && mousePosition.Y <= 0.05f * Screen.Height)
                {
                    camY -= Speed;
                }
                Position = new Vector3(camX, camY, Position.Z);
            }
        }
        #endregion
    }
}