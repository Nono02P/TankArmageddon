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
        public int FittingScore { get; set; }
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
            _inputs = new float[30];
            _neuralNetwork = new NeuralNetwork(_inputs.Length, new int[] { 24, 20 }, 20, ActivationFunctions.eActivationFunction.TanH, true);
        }
        #endregion

        #region Normalisation des entrées
        private Vector2 MapNormalisation(Vector2 pPosition)
        {
            Vector2 mapSize = Parent.Parent.MapSize;
            return new Vector2((float)utils.MapValue(-mapSize.X, mapSize.X, -1, 1, pPosition.X), (float)utils.MapValue(-mapSize.Y, mapSize.Y, -1, 1, pPosition.Y));
        }
        #endregion

        #region Recherche d'acteurs le plus proche
        private IActor ShortestDistance(Tank pTank, List<IActor> actors)
        {
            IActor result = null;
            float distMin = -1;
            for (int i = 0; i < actors.Count; i++)
            {
                IActor actor = actors[i];
                if (actor != pTank)
                {
                    float dist = (float)Math.Abs(utils.MathDist(pTank.Position, actor.Position));
                    if (distMin > dist || distMin == -1)
                    {
                        distMin = dist;
                        result = actor;
                    }
                }
            }
            return result;
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
                _inputs[2] = (float)utils.MapValue(MathHelper.ToRadians(0), MathHelper.ToRadians(360), -1, 1, CurrentTank.Angle);
                _inputs[3] = (float)utils.MapValue(MathHelper.ToRadians(0), MathHelper.ToRadians(360), -1, 1, CurrentTank.AngleCannon);
                _inputs[4] = CurrentTank.Life / 100;
                _inputs[5] = CurrentTank.Fuel / 100;
                
                #region Eau la plus proche
                Vector2 min = - Vector2.One;
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
                _inputs[6] = waterDist.X;
                _inputs[7] = waterDist.Y;

                #region Drop le plus proche
                Drop drop = (Drop)ShortestDistance(CurrentTank, Parent.Parent.LstActors.FindAll(d => d is Drop));
                Vector2 dropDist;
                if (drop == null)
                {
                    dropDist = Vector2.One;
                }
                else
                {
                    dropDist = MapNormalisation(drop.Position - CurrentTank.Position);
                }
                #endregion

                _inputs[8] = dropDist.X;
                _inputs[9] = dropDist.Y;

                #region Allié le plus proche
                Tank friend = (Tank)ShortestDistance(CurrentTank, Parent.Tanks.FindAll(t => t.Parent == Parent && t != CurrentTank).Cast<IActor>().ToList());
                Vector2 friendDist;
                float life = 0;
                if (friend == null)
                {
                    friendDist = Vector2.One;
                    life = 1;
                }
                else
                {
                    friendDist = MapNormalisation(friend.Position - CurrentTank.Position);
                    life = friend.Life / 100;
                }
                #endregion

                _inputs[10] = friendDist.X;
                _inputs[11] = friendDist.Y;
                _inputs[12] = life;

                #region Ennemi le plus proche
                List<Tank> tanks = new List<Tank>();
                for (int i = 0; i < Parent.Parent.Teams.Count; i++)
                {
                    Team team = Parent.Parent.Teams[i];
                    tanks.AddRange(team.Tanks);
                }
                Tank ennemy = (Tank)ShortestDistance(CurrentTank, tanks.FindAll(t => t.Parent != Parent).Cast<IActor>().ToList());
                Vector2 ennemyDist;
                if (ennemy == null)
                {
                    ennemyDist = Vector2.One;
                    life = 1;
                }
                else
                {
                    ennemyDist = MapNormalisation(ennemy.Position - CurrentTank.Position);
                    life = ennemy.Life / 100;
                }
                #endregion

                _inputs[13] = ennemyDist.X;
                _inputs[14] = ennemyDist.Y;
                _inputs[15] = life;

                #region Action sélectionnée
                for (int i = 0; i < Enum.GetValues(typeof(Action.eActions)).Length; i++)
                {
                    if (CurrentTank.SelectedAction == (Action.eActions)i)
                        _inputs[16 + i] = 1;
                    else
                        _inputs[16 + i] = 0;
                }
                #endregion

                #endregion

                float[] outputs = _neuralNetwork.FeedForward(_inputs);

                #region Affectation des sorties
                bool[] commands = new bool[outputs.Length];
                for (int i = 0; i < outputs.Length; i++)
                {
                    if (outputs[i] >= 0f)
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
                        Parent.SelectAction((Action.eActions)i);
                        //if (Parent.SelectAction((Action.eActions)i))
                            //break;
                }
                #endregion
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
        }
        #endregion
    }
}