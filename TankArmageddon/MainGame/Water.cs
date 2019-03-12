using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Water : ICollisionnable
    {
        #region Variables privées
        private Vector2 _position;
        private Vector2 _size;
        private int _maxBottom;
        private float _tension;
        private float _dampening;
        private List<Particle> _particles;
        #endregion

        #region Propriétés
        public float Layer { get; set; } = 1f;
        public Gameplay Parent { get; private set; }
        public Spring[] Springs { get; private set; }
        public IBoundingBox BoundingBox { get; private set; }
        public int WaveWidth { get; set; }
        public bool Remove { get; set; }
        public Vector2 Position { get => _position; set { _position = value; RefreshBoundingBox(); WaterLevel = (int)value.Y; } }
        public Vector2 Size { get => _size;  set { _size = value; RefreshBoundingBox(); } }
        public float Spread { get; set; } = 0.25f;
        public float Tension
        {
            get => _tension;
            set
            {
                _tension = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref Spring s = ref Springs[i];
                    s.Tension = value;
                }
            }
        }
        public float Dampening
        {
            get => _dampening;
            set
            {
                _dampening = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref Spring s = ref Springs[i];
                    s.Dampening = value;
                }
            }
        }
        public int WaterLevel
        {
            get => (int)_position.Y;
            set
            {
                _position.Y = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref Spring s = ref Springs[i];
                    s.TargetValue = value;
                    if (s.Value == 0)
                        s.Value = s.TargetValue;
                }
            }
        }
        #endregion

        #region Constructeur
        public Water(Gameplay pParent, Vector2 pPosition, Vector2 pSize, int pWaveWidth = 4)
        {
            Parent = pParent;
            WaveWidth = pWaveWidth;
            _maxBottom = MainGame.Screen.Height;
            int numberOfSprings = (int)pSize.X / WaveWidth;
            Springs = new Spring[numberOfSprings];
            Position = pPosition;
            Size = pSize;
            Tension = 0.025f;
            Dampening = 0.025f;
            _particles = new List<Particle>();
            MainGame.CurrentScene.AddActor(this);

            Parent.OnExplosion += Gameplay_OnExplosion;
        }
        #endregion

        #region Unload
        public void Unload()
        {
            Parent.OnExplosion -= Gameplay_OnExplosion;
        }
        #endregion

        #region Splash
        public void Splash(Vector2 pPosition, float pVelocity)
        {
            int index = (int)pPosition.X / WaveWidth;
            if (index >= 0 && index < Springs.Length)
                Springs[index].Velocity = pVelocity;
        }
        #endregion

        #region BoundingBox
        public void RefreshBoundingBox()
        {
            BoundingBox = new RectangleBBox(Position.ToPoint(), Size.ToPoint());
        }
        #endregion

        #region Sur explosion sur la map
        private void Gameplay_OnExplosion(object sender, ExplosionEventArgs e)
        {
            if (e.ExplosionCircle.Bottom > WaterLevel)
            {
                Splash(e.ExplosionCircle.Location.ToVector2() / WaveWidth, e.Force * 20);
            }
        }
        #endregion

        #region Collisions
        public void TouchedBy(ICollisionnable collisionnable)
        {
            if (collisionnable is Sprite)
            {
                Sprite s = (Sprite)collisionnable;
                Splash(s.Position, s.Velocity.Y * 10);
            }
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            #region Update des ressorts
            for (int i = 0; i < Springs.Length; i++)
            {
                ref Spring s = ref Springs[i];
                s.Update();
            }
            #endregion

            #region Propagation des ressorts voisins
            float[] leftDeltas = new float[Springs.Length];
            float[] rightDeltas = new float[Springs.Length];
            for (int j = 0; j < 7; j++)
            {
                for (int i = 1; i < Springs.Length - 1; i++)
                {
                    ref Spring s0 = ref Springs[i - 1];
                    ref Spring s1 = ref Springs[i];
                    ref Spring s2 = ref Springs[i + 1];
                    leftDeltas[i] = Spread * (s1.Value - s0.Value);
                    s0.Velocity += leftDeltas[i];
                    rightDeltas[i] = Spread * (s1.Value - s2.Value);
                    s2.Velocity += rightDeltas[i];
                }

                for (int i = 1; i < Springs.Length - 1; i++)
                {
                    ref Spring s0 = ref Springs[i - 1];
                    ref Spring s2 = ref Springs[i + 1];
                    s0.Value += leftDeltas[i];
                    s2.Value += rightDeltas[i];
                }
            }
            #endregion
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

        public void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime)
        {
            Color midnightBlue = new Color(0, 15, 40) * 0.9f;
            Color lightBlue = new Color(0.2f, 0.5f, 1f) * 0.8f;
            Camera cam = MainGame.Camera;
            Vector3 offsetPos = cam.Position - cam.CameraOffset;
            float bottom = Position.Y + offsetPos.Y + Size.Y;
            bottom = MathHelper.Clamp(bottom, 0, _maxBottom);
            float scale = (Position.X + offsetPos.X + Size.X) / (Springs.Length - 1f); // be sure to use float division

            for (int i = (int)(offsetPos.X / scale); i < (int)((offsetPos.X + MainGame.Screen.Width) / scale); i++) //Springs.Length - 1; i++)
            {
                Spring s1 = Springs[i];
                Spring s2 = Springs[i + 1];
                
                Vector2 p1 = new Vector2(i * scale - offsetPos.X, s1.Value - offsetPos.Y);
                Vector2 p2 = new Vector2((i + 1) * scale - offsetPos.X, s2.Value - offsetPos.Y);
                Vector2 p3 = new Vector2(p2.X, bottom);
                Vector2 p4 = new Vector2(p1.X, bottom);

                primitiveBatch.AddVertex(p1, lightBlue);
                primitiveBatch.AddVertex(p2, lightBlue);
                primitiveBatch.AddVertex(p3, midnightBlue);

                primitiveBatch.AddVertex(p1, lightBlue);
                primitiveBatch.AddVertex(p3, midnightBlue);
                primitiveBatch.AddVertex(p4, midnightBlue);
            }
        }
        #endregion
    }
}