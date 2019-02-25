using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankArmageddon
{
    public class Water : IActor
    {
        private Vector2 _position;
        private Vector2 _size;
        private float _tension;
        private float _dampening;

        public Spring[] Springs { get; private set; }
        public IBoundingBox BoundingBox { get; private set; }
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

        #region Constructeur
        public Water(Vector2 pPosition, Vector2 pSize, int pNumberOfSprings)
        {
            Springs = new Spring[pNumberOfSprings];
            Position = pPosition;
            Size = pSize;
            Tension = 0.025f;
            Dampening = 0.025f;
            MainGame.CurrentScene.AddActor(this);
        }
        #endregion


        public void Splash(Vector2 pPosition, float pVelocity)
        {
            int index = (int)pPosition.X;
            if (index >= 0 && index < Springs.Length)
                Springs[index].Velocity = pVelocity;
        }

        public void RefreshBoundingBox()
        {
            BoundingBox = new RectangleBBox(Position.ToPoint(), Size.ToPoint());
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Springs.Length; i++)
            {
                ref Spring s = ref Springs[i];
                s.Update();
            }

            float[] leftDeltas = new float[Springs.Length];
            float[] rightDeltas = new float[Springs.Length];
            for (int j = 0; j < 8; j++)
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
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

        public void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime)
        {
            Color midnightBlue = new Color(0, 15, 40) * 0.9f;
            Color lightBlue = new Color(0.2f, 0.5f, 1f) * 0.8f;
            
            float bottom = Position.Y + Size.Y;

            // stretch the springs' x positions to take up the entire window
            float scale = (Position + Size).X / (Springs.Length - 1f); // be sure to use float division

            for (int i = 0; i < Springs.Length - 1; i++)
            {
                Spring s1 = Springs[i];
                Spring s2 = Springs[i + 1];
                
                Vector2 p1 = new Vector2(i * scale, s1.Value);
                Vector2 p2 = new Vector2((i + 1) * scale, s2.Value);
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
    }
}
