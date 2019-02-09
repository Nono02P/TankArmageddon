using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public class Camera
    {
        #region Evènements
        public event onVector3Change OnPositionChange;
        #endregion

        private Vector3 _position;

        public int Speed { get; set; }
        public Viewport Screen { get; set; }
        public Vector3 MapSize { get; set; }
        public Vector3 Origin { get; set; }
        public float Angle { get; set; }
        public float Zoom { get; set; }
        public bool Enable { get; set; }
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                value.X = (int)MathHelper.Clamp(value.X, 0, MapSize.X - Screen.Width);
                value.Y = (int)MathHelper.Clamp(value.Y, 0, MapSize.Y - Screen.Height);
                value.Z = (int)MathHelper.Clamp(value.Y, 0, MapSize.Z);
                if (_position != value)
                {
                    OnPositionChange?.Invoke(this, _position, value);
                    _position = value;
                }
            }
        }

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(-Position)
                    * Matrix.CreateScale(Zoom, Zoom, 1f)
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(Angle))
                    * Matrix.CreateTranslation(Origin);
            }
        }

        public Camera(Viewport pScreen, Vector3 pMapSize, float pZoom = 1, int pSpeed = 0)
        {
            Screen = pScreen;
            MapSize = pMapSize;
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

        public Vector2 ScreenToWorld(Vector2 pos)
        {
            return Vector2.Transform(pos, Matrix.Invert(Transformation));
        }

        public Vector2 WorldToScreen(Vector2 pos)
        {
            return Vector2.Transform(pos, Transformation);
        }

        public void SetCameraOnActor(IActor actor)
        {
            Position = new Vector3((int)(actor.Position.X - Screen.Width / 2), (int)(actor.Position.Y - Screen.Height / 2), 0);
        }

        public void Update()
        {
            if (Enable)
            {
                Point mousePosition = Mouse.GetState().Position;
                int camX = (int)Position.X;
                int camY = (int)Position.Y;
                if (mousePosition.X >= 0.95f * Screen.Width)
                {
                    camX += Speed;
                }
                if (mousePosition.X <= 0.05f * Screen.Width)
                {
                    camX -= Speed;
                }
                if (mousePosition.Y >= 0.95f * Screen.Height)
                {
                    camY += Speed;
                }
                if (mousePosition.Y <= 0.05f * Screen.Height)
                {
                    camY -= Speed;
                }
                Position = new Vector3(camX, camY, 0);
            }
        }
    }
}