using IA;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TankArmageddon
{
    public class NeuralNetworkControl : IControl
    {
        #region Variables privées
        private float[] _inputs;
        private NeuralNetwork _neuralNetwork;
        #endregion

        #region Propriétés
        public Team Parent { get; private set; }
        public bool OnPressedLeft { get; private set; }
        public bool OnPressedRight { get; private set; }
        public bool OnPressedUp { get; private set; }
        public bool OnPressedDown { get; private set; }
        public bool OnPressedSpace { get; private set; }
        public bool OnPressedN { get; private set; }
        public bool IsDownLeft { get; private set; }
        public bool IsDownRight { get; private set; }
        public bool IsDownUp { get; private set; }
        public bool IsDownDown { get; private set; }
        public bool IsDownSpace { get; private set; }
        public bool IsDownN { get; private set; }
        public bool OnReleasedSpace { get; private set; }
        #endregion

        #region Constructeur
        public NeuralNetworkControl(Team pParent)
        {
            Parent = pParent;
            _inputs = new float[24];
            _neuralNetwork = new NeuralNetwork(_inputs.Length, new int[] { 32, 16 }, 20);
        }
        #endregion

        #region Normalisation des entrées
        private Vector2 MapNormalisation(Vector2 pPosition)
        {
            Vector2 mapSize = Parent.Parent.MapSize;
            Vector2 p = new Vector2(MathHelper.Clamp(pPosition.X, 0, mapSize.X), MathHelper.Clamp(pPosition.Y, 0, mapSize.Y));
            p /= mapSize;
            return p * 2 - Vector2.One;
        }
        #endregion

        #region Recherche du tank le plus proche
        private Vector2 ShortestDistance(Tank pTank, List<Tank> tanks)
        {
            Vector2 min = new Vector2(100000);
            float distMin = -1;
            for (int i = 0; i < tanks.Count; i++)
            {
                Tank t = tanks[i];
                if (t != pTank)
                {
                    float dist = (float)Math.Abs(utils.MathDist(pTank.Position, t.Position));
                    if (distMin > dist || distMin == -1)
                    {
                        distMin = dist;
                        min = t.Position;
                    }
                }
            }
            return MapNormalisation(min - pTank.Position);
        }
        #endregion

        #region Retourne la position du curseur
        public Vector2 CursorPosition(bool pOffensive)
        {
            Vector2 result;
            if (pOffensive)
            {
                result = new Vector2(_inputs[8] * Parent.Parent.MapSize.X, _inputs[9] * Parent.Parent.MapSize.Y);
            }
            else
            {
                result = new Vector2(_inputs[6] * Parent.Parent.MapSize.X, _inputs[7] * Parent.Parent.MapSize.Y);
            }
            return result;
        }
        #endregion

        #region Update
        public void Update(bool pRefresh)
        {
            if (pRefresh)
            {
                #region Affectation des entrées
                Tank CurrentTank = Parent.Tanks[Parent.IndexTank];
                Vector2 normalisedTankPos = MapNormalisation(CurrentTank.Position);
                _inputs[0] = normalisedTankPos.X;
                _inputs[1] = normalisedTankPos.Y;
                _inputs[2] = CurrentTank.Angle;
                _inputs[3] = CurrentTank.AngleCannon;

                #region Eau la plus proche
                Vector2 min = new Vector2(100000);
                float distMin = -1;
                for (int i = 0; i < Parent.Parent.WaterPosition.Count; i++)
                {
                    Rectangle water = Parent.Parent.WaterPosition[i];
                    if (water.Location.X < CurrentTank.Position.X)
                    {
                        float dist = (float)Math.Abs(utils.MathDist(CurrentTank.Position, water.Location.ToVector2()));
                        if (distMin > dist || distMin == -1)
                        {
                            distMin = dist;
                            min = water.Location.ToVector2();
                        }
                    }
                    else
                    {
                        float dist = (float)Math.Abs(utils.MathDist(CurrentTank.Position.X, CurrentTank.Position.Y, water.Right, water.Top));
                        if (distMin > dist || distMin == -1)
                        {
                            distMin = dist;
                            min = new Vector2(water.Right, water.Top);
                        }
                    }
                }
                #endregion

                Vector2 waterDist = MapNormalisation(min) - normalisedTankPos;
                _inputs[4] = waterDist.X;
                _inputs[5] = waterDist.Y;

                #region Allié le plus proche
                Vector2 friendDist = ShortestDistance(CurrentTank, Parent.Tanks.FindAll(t => t.Parent == Parent && t != CurrentTank));
                #endregion

                _inputs[6] = friendDist.X;
                _inputs[7] = friendDist.Y;

                #region Ennemi le plus proche
                Vector2 ennemyDist = ShortestDistance(CurrentTank, Parent.Tanks.FindAll(t => t.Parent != Parent));
                #endregion

                _inputs[8] = ennemyDist.X;
                _inputs[9] = ennemyDist.Y;

                #region Action sélectionné
                for (int i = 0; i < Enum.GetValues(typeof(Action.eActions)).Length; i++)
                {
                    if (CurrentTank.SelectedAction == (Action.eActions)i)
                        _inputs[10 + i] = 1;
                    else
                        _inputs[10 + i] = 0;
                }
                #endregion

                #endregion

                float[] outputs = _neuralNetwork.FeedForward(_inputs);

                #region Affectation des sorties
                bool[] commands = new bool[outputs.Length];
                for (int i = 0; i < outputs.Length; i++)
                {
                    if (outputs[i] >= 0.5f)
                    {
                        commands[i] = true;
                    }
                }
                OnPressedLeft = commands[0] && !IsDownLeft;
                IsDownLeft = commands[0];

                OnPressedRight = commands[1] && !IsDownRight;
                IsDownRight = commands[1];

                OnPressedUp = commands[2] && !IsDownUp;
                IsDownUp = commands[2];

                OnPressedDown = commands[3] && !IsDownDown;
                IsDownDown = commands[3];

                OnPressedSpace = commands[4] && !IsDownSpace;
                OnReleasedSpace = !commands[4] && IsDownSpace;
                IsDownSpace = commands[4];

                OnPressedN = commands[5] && !IsDownN;
                IsDownN = commands[5];

                for (int i = 1; i < Enum.GetValues(typeof(Action.eActions)).Length; i++)
                {
                    if (commands[i + 5])
                        if (Parent.SelectAction((Action.eActions)i))
                            break;
                }
            }
            else
            {
                OnPressedLeft = false;
                OnPressedRight = false;
                OnPressedUp = false;
                OnPressedDown = false;
                OnPressedSpace = false;
                OnPressedN = false;
                IsDownLeft = false;
                IsDownRight = false;
                IsDownUp = false;
                IsDownDown = false;
                IsDownSpace = false;
                IsDownN = false;
                OnReleasedSpace = false;
            }
            #endregion
        }
        #endregion
    }
}