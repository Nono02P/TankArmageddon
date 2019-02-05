﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Timers;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Gameplay : Scene
    {
        #region Constantes
        private const int TIME_PER_TOUR = 60;
        private const int TIME_BETWEEN_TOUR = 10;
        private const byte NUMBER_OF_TEAMS = 4;
        private const byte NUMBER_OF_TANK_PER_TEAM = 5;
        #endregion

        #region Variables privées
        private Group _group;
        private Textbox _timerTextBox;
        private List<Team> _teams;
        private Textbox _currentTeamTextBox;
        private Timer _timerSecond;
        private int _counter = TIME_PER_TOUR;
        private bool _inTour = true;
        private Texture2D _mapTexture;
        private float[] _perlinNoise;
        private byte _indexTeam;
        #endregion

        #region Propriétés
        public byte IndexTeam
        {
            get { return _indexTeam; }
            private set
            {
                if (_teams.Count > 0 && _indexTeam != value)
                {
                    _indexTeam = value;
                    _teams[value].RefreshCameraOnSelection();
                }
            }
        }

        public Vector2 MapSize { get; private set; } = new Vector2(4096, MainGame.Screen.Height);
        public byte[] MapData { get; private set; }
        public Color[] MapColors { get; private set; }
        #endregion

        #region Constructeur
        public Gameplay() { }
        #endregion

        #region Méthodes

        #region Load/Unload
        public override void Load()
        {
            #region Démarrage des musiques
            sndMusic = AssetManager.sndMusicGameplay;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;
            #endregion

            #region Création de la map
            _perlinNoise = PerlinNoise.Generate1DMap((int)MapSize.X, 550f);
            _mapTexture = new Texture2D(MainGame.spriteBatch.GraphicsDevice, (int)MapSize.X, (int)MapSize.Y);
            MapData = new byte[(int)(MapSize.X * MapSize.Y)];
            MapColors = new Color[MapData.Length];
            for (ushort i = 0; i < _perlinNoise.Length; i++)
            {
                ushort x = (ushort)(i % MapSize.X);
                ushort h = (ushort)Math.Floor((_perlinNoise[i] + 1) * MapSize.Y / 2);
                for (ushort y = h; y < MapSize.Y; y++)
                {
                    uint index = (uint)(x + y * MapSize.X);
                    MapColors[index] = Color.Green;
                    MapData[index] = 1;
                }
            }
            _mapTexture.SetData(MapColors);
            #endregion

            #region Paramétrage de la Caméra
            Camera c = MainGame.Camera;
            c.MapSize = new Vector3(MapSize.X, MapSize.Y, 0);
            c.Enable = true;
            c.Speed = 10;
            #endregion

            #region Création d'un timer de tours
            _timerSecond = new Timer(1000);
            _timerSecond.Elapsed += OnTimerElapsed;
            _timerSecond.AutoReset = true;
            _timerSecond.Enabled = true;
            #endregion

            #region Création des équipes
            _teams = new List<Team>();
            Texture2D img = AssetManager.TanksSpriteSheet;
            for (byte i = 0; i < NUMBER_OF_TEAMS; i++)
            {
                _teams.Add(new Team(this, img, NUMBER_OF_TANK_PER_TEAM, i));
            }
            _teams[IndexTeam].RefreshCameraOnSelection();
            #endregion

            #region Création des éléments de GUI
            _timerTextBox = new Textbox(new Vector2(10, 10), AssetManager.MainFont, TIME_BETWEEN_TOUR.ToString());
            _currentTeamTextBox = new Textbox(new Vector2(40, 10), AssetManager.MainFont, "Team " + (IndexTeam + 1).ToString());
            _currentTeamTextBox.ApplyColor(_teams[IndexTeam].TeamColor, Color.Black);
            _group = new Group();
            _group.AddElement(_timerTextBox);
            _group.AddElement(_currentTeamTextBox);
            c.OnPositionChange += OnCameraPositionChange;
            #endregion

            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }
        #endregion

        /// <summary>
        /// Evènements du timer (1 sec) pour la gestion des tours.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="e"></param>
        public void OnTimerElapsed(object state, ElapsedEventArgs e)
        {
            _counter--;
            _timerTextBox.Text = _counter.ToString();
            if (_counter <= 0)
            {
                if (_inTour)
                {
                    _counter = TIME_BETWEEN_TOUR;
                    IndexTeam++;
                    IndexTeam = (byte)(IndexTeam % _teams.Count);
                    _currentTeamTextBox.Text = "Team " + (IndexTeam + 1).ToString();
                    _currentTeamTextBox.ApplyColor(_teams[IndexTeam].TeamColor, Color.Black);
                }
                else
                {
                    _counter = TIME_PER_TOUR;
                }
                _inTour = !_inTour;
            }
        }

        /// <summary>
        /// Créé une explosion à l'emplacement passé par le missille.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bulletEventArgs"></param>
        public void CreateExplosion(object sender, Tank.Bullet.BulletEventArgs bulletEventArgs)
        {
            Vector2 pPosition = bulletEventArgs.Position;
            int pRadius = bulletEventArgs.Radius;
            for (uint x = (uint)(pPosition.X - pRadius); x <= (uint)(pPosition.X + pRadius); x++)
            {
                if (x >= 0 && x < MapSize.X)
                {
                    for (uint y = (uint)(pPosition.Y - pRadius); y <= (uint)(pPosition.Y + pRadius); y++)
                    {
                        if (y >= 0 && y < MapSize.Y)
                        {
                            if (Math.Pow(pPosition.X - x, 2) + Math.Pow(pPosition.Y - y, 2) < Math.Pow(pRadius, 2))
                            {
                                if (IsSolid(new Vector2(x, y)))
                                {
                                    MapData[x + y * (int)MapSize.X] = 0;
                                    MapColors[x + y * (int)MapSize.X] = new Color();
                                }
                            }
                        }
                    }
                } 
            }
            _mapTexture.SetData(MapColors);
        }

        /// <summary>
        /// Renvoies l'écart entre la position et le premier point le plus haut en Y sur la map à l'offset X.
        /// </summary>
        /// <param name="pPosition">Position du point qui doit être calculé.</param>
        /// <param name="pXOffset">Offset en X à vérifier par rapport à la position.</param>
        /// <returns>Renvoies un Vector2 correspondant à l'écart entre la position passée et le premier point le plus haut à l'offset X.</returns>
        public Vector2 FindHighestPoint(Vector2 pPosition, int pHeightLimit, int pXOffset)
        {
            // Remonte pour trouver le pixel le plus haut à l'emplacement Position.X + XOffset.
            // Afin d'éviter de trop remonter, le paramètre HeightLimit correspond au nombre de pixel max à remonter
            int y = (int)pPosition.Y;
            while (y > pPosition.Y - pHeightLimit)
            {
                y--;
                if (MapData[(uint)(pPosition.X + pXOffset + y * MapSize.X)] == 0)
                {
                    break;
                }
            }
            // Redescend pour les cas où le pixel le plus haut est en dessous.
            while (y < MapSize.Y)
            {
                y++;
                if (MapData[(uint)(pPosition.X + pXOffset + y * MapSize.X)] > 0)
                {
                    break;
                }
            }
            return new Vector2(pXOffset, y - pPosition.Y);
        }

        /// <summary>
        /// Vérifie que la position passé en paramètre soit sur un solide.
        /// </summary>
        /// <param name="pPosition"></param>
        /// <returns></returns>
        public bool IsSolid(Vector2 pPosition)
        {
            bool result = false;
            if (pPosition.X >= 0 && pPosition.X < MapSize.X && pPosition.Y >= 0 && pPosition.Y < MapSize.Y)
            {
                result = MapData[(uint)(pPosition.X) + (uint)(pPosition.Y * MapSize.X)] > 0;
            }
            return result;
        }

        #region Calcul de la position de GUI sur changement de la position de Caméra.
        public void OnCameraPositionChange(object sender, Vector3 previous, Vector3 actual)
        {
            _group.Position = new Vector2(actual.X, actual.Y);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Collisions
            // Gère les collisions entre IActors
            foreach (IActor actor in lstActors)
            {
                if (actor is ICollisionnable)
                {
                    foreach (IActor actor2 in lstActors)
                    {
                        if (actor2 is ICollisionnable)
                        {
                            ICollisionnable col = (ICollisionnable)actor;
                            ICollisionnable col2 = (ICollisionnable)actor2;
                            if (utils.Collide(actor, actor2))
                            {
                                col.TouchedBy(col2);
                                col2.TouchedBy(col);
                            }
                        }
                    }
                }
            }
            #endregion

            _teams[IndexTeam].Update(gameTime, _inTour);

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_mapTexture, Vector2.Zero, Color.White);
            base.Draw(spriteBatch, gameTime);
        }
        #endregion
        
        #endregion
    }
}