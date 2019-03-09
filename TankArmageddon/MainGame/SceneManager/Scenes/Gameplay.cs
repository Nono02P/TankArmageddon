using IA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public class Gameplay : Scene
    {
        #region Constantes
        private const int TIME_PER_TOUR = 60;
        private const int TIME_PER_TOUR_IA_TRAINING = 20;
        private const int TIME_AFTER_ACTION = 10;
        private const int TIME_BETWEEN_TOUR = 10;
        #endregion

        #region Evènements
        public event ExplosionHandler OnExplosion;
        public event EventHandler OnTourTimerEnd;
        #endregion

        #region Variables privées
        private Textbox _fittingScoreTextBox;
        private Textbox _timerTextBox;
        private Textbox _bigTimerTextBox;
        private Textbox _currentTeamTextBox;
        private Textbox _currentTankTextBox;
        private Textbox _infoBulle;
        private Timer _timerSecond;
        private int _counter = TIME_PER_TOUR;
        private bool _inTour = false;
        private Texture2D _mapTexture;
        private Texture2D _skyTexture;
        private int _skyHeight = 3000;
        private float[] _perlinNoise;
        private int _indexTeam = 0;
        private Image _gameBarImage;
        private Image _cursorImage;
        private List<Action.eActions> _lootBag;
        private SoundEffect _sndexplosion;
        private Water _water;
        private bool _gameFinnished;
        private List<string> _names = new List<string>()
            {
                "Almex",
                "Anais_Ld",
                "Anata",
                "Arnkil",
                "Asthegor",
                "Azharis",
                "Bertho",
                "BreakingBardo",
                "Cehem",
                "David",
                "Demacedius",
                "Duruti",
                "Exe siga",
                "Flashjaysan",
                "FrenchAssassinX",
                "Guitoon",
                "helloWorld",
                "HydroGene",
                "JadisGames",
                "Jérôme",
                "Jpcr",
                "Jray",
                "Kiba",
                "Krayne Radion_Wave",
                "Liolabs",
                "Lou",
                "LoubiTek",
                "Matutu",
                "Mega",
                "Mickdev",
                "Morgan",
                "Neortik",
                "Nerils",
                "Nono02P",
                "Padawan",
                "Pompo",
                "Pseudotom",
                "Puurple",
                "PXLcat",
                "Pyxel",
                "Raoul",
                "Rayndar",
                "S3rval",
                "Steph.",
                "Tetsuro",
                "Thomas T",
                "Tomroux03",
                "Torto",
                "Ufo97",
                "Valdaria",
                "Veronimish",
                "Vesgames",
                "Vince8",
                "Wazou",
                "Wile",
                "Zethzer",
            };
        #endregion

        #region Propriétés
        public bool IATrainingMode { get; private set; }
        public Population Population { get; private set; }
        public Group GUIGroup { get; private set; }
        public GroupSelection GUIGroupButtons { get; private set; }
        public int WaterLevel { get { return (int)(MapSize.Y - MapSize.Y * 0.05f); } }
        public int WaterHeight { get; private set; }
        public Vector2 MapSize { get; private set; } = new Vector2(4096, MainGame.Screen.Height - AssetManager.GameBottomBar.Height);
        public byte[] MapData { get; private set; }
        public Color[] MapColors { get; private set; }
        public List<Rectangle> WaterPosition;
        public List<Team> Teams { get; private set; }
        public int IndexTeam
        {
            get { return _indexTeam; }
            private set
            {
                if (Teams.Count > 0 && _indexTeam != value)
                {
                    _indexTeam = value % Teams.Count;
                    Team curTeam = Teams[_indexTeam];
                    if (IATrainingMode)
                    {
                        GeneticNeuralNetwork genome = ((NeuralNetworkControl)curTeam.Control).Genome;
                        if (_indexTeam != value)
                        {
                            Population.NextGeneration();
                        }
                        _fittingScoreTextBox.Text = "Fitness Score : " + genome.FitnessScore + " Generation : " + Population.Generation;
                    }
                    curTeam.RefreshCameraOnSelection();
                    RefreshActionButtonInventory();
                }
            }
        }
        #endregion

        #region Load/Unload
        public override void Load()
        {
            #region Initialisation de la population (en cas de mode entrainement IA)
            IATrainingMode = MainGame.IATrainingMode;
            if (IATrainingMode)
            {
                Population = Population.OpenFromFile("Population");
                //Population = new Population();
                Population.OnGenomesChanged += Population_OnGenomesChanged;
            }
            #endregion

            #region Démarrage des musiques
            sndMusic = AssetManager.mscGameplay;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            #endregion

            #region Ajout des bruitages
            _sndexplosion = AssetManager.sndExplosion;
            #endregion

            #region Création du ciel
            _skyTexture = new Texture2D(MainGame.spriteBatch.GraphicsDevice, (int)MapSize.X, _skyHeight);
            Color[] skyData = new Color[_skyTexture.Width * _skyTexture.Height];
            int skyH = _skyTexture.Height;
            int skyW = _skyTexture.Width;
            for (int y = 0; y < skyH; y++)
            {
                Color skyColor = Color.Lerp(Color.Black, new Color(119, 181, 254), (float)y / skyH);
                for (int x = 0; x < skyW; x++)
                {
                    skyData[x + y * skyW] = skyColor;
                }
            }
            _skyTexture.SetData(skyData);
            #endregion

            #region Création de la map et relevé des points d'eau pour le réseau de neurone
            WaterPosition = new List<Rectangle>();
            bool beginWater = false;
            int beginWaterPosition = 0;
            _perlinNoise = PerlinNoise.Generate1DMap((int)MapSize.X, 550f);
            _mapTexture = new Texture2D(MainGame.spriteBatch.GraphicsDevice, (int)MapSize.X, (int)MapSize.Y);
            MapData = new byte[(int)(MapSize.X * MapSize.Y)];
            MapColors = new Color[MapData.Length];

            Color[] colors = new Color[5];
            colors[0] = Color.White;
            colors[1] = Color.Green;
            colors[2] = Color.DarkGreen;
            colors[4] = new Color(132, 73, 44);
            colors[3] = new Color(62, 28, 0);

            for (ushort i = 0; i < _perlinNoise.Length; i++)
            {
                float noiseVal = _perlinNoise[i];
                ushort x = (ushort)(i % MapSize.X);
                ushort h = (ushort)Math.Floor((noiseVal + 1) * MapSize.Y * 0.5f);
                h = (ushort)MathHelper.Clamp(h, 0, WaterLevel + 1);
                int min = (int)Math.Floor((_perlinNoise.Min() + 1) * MapSize.Y * 0.5f);
                int dif = (int)MapSize.Y - min;
                for (ushort y = h; y < MapSize.Y; y++)
                {
                    uint index = (uint)(x + y * MapSize.X);
                    if (y <= WaterLevel)
                    {
                        float pourcent = (float)(y - min) / dif;
                        float p1 = 0.25f;
                        float p2 = 0.5f;
                        float p3 = 0.75f;
                        float p4 = 1f;
                        if (pourcent < p1)
                        {
                            MapColors[index] = Color.Lerp(colors[0], colors[1], pourcent / p1);
                        }
                        else if (pourcent < p2)
                        {
                            MapColors[index] = Color.Lerp(colors[1], colors[2], (pourcent - p1) / (p2 - p1));
                        }
                        else if (pourcent < p3)
                        {
                            MapColors[index] = Color.Lerp(colors[2], colors[3], (pourcent - p2) / (p3 - p2));
                        }
                        else if (pourcent < p4)
                        {
                            MapColors[index] = Color.Lerp(colors[3], colors[4], (pourcent - p3) / (p4 - p3));
                        }
                        MapData[index] = 1;
                    }
                    else
                    {
                        MapColors[index] = Color.Brown;
                    }
                }
                if (!IsSolid(new Vector2(x, WaterLevel - 1)))
                {
                    if (!beginWater)
                    {
                        beginWater = true;
                        beginWaterPosition = x;
                    }
                }
                else
                {
                    if (beginWater)
                    {
                        beginWater = false;
                        WaterPosition.Add(new Rectangle(beginWaterPosition, WaterLevel, x - beginWaterPosition, (int)MapSize.Y - WaterLevel));
                    }
                }
            }
            _mapTexture.SetData(MapColors);
            #endregion

            #region Création de l'eau
            Vector2 p = new Vector2(0, WaterLevel);
            WaterHeight = (int)(MapSize - p).Y;
            Vector2 s = new Vector2(MapSize.X, WaterHeight);
            _water = new Water(this, p, s);
            #endregion

            #region Paramétrage de la Caméra
            Camera c = MainGame.Camera;
            c.CameraSize = new Vector3(MapSize.X, MapSize.Y, 0);
            c.CameraOffset = new Vector3(0, MapSize.Y - MainGame.Screen.Height, 0);
            c.Enable = true;
            c.Speed = 10;
            #endregion

            #region Création d'un timer de tours
            _timerSecond = new Timer(1000);
            _timerSecond.Elapsed += OnTimerElapsed;
            _timerSecond.AutoReset = true;
            _timerSecond.Enabled = true;
            _counter = TIME_BETWEEN_TOUR;
            #endregion

            #region Création des éléments de GUI
            GUIGroup = new Group();
            Texture2D texture = AssetManager.GameBottomBar;
            _gameBarImage = new Image(texture, new Vector2(MainGame.Screen.Width / 2, MainGame.Screen.Height - texture.Height / 2));
            _gameBarImage.Layer -= 0.1f;
            _gameBarImage.SetOriginToCenter();
            GUIGroup.AddElement(_gameBarImage);

            texture = AssetManager.Cursor;
            _cursorImage = new Image(texture, new Vector2(_gameBarImage.Position.X - _gameBarImage.Origin.X + (_gameBarImage.Position.X / MapSize.X), _gameBarImage.Position.Y - texture.Height * 0.75f));
            _cursorImage.SetOriginToCenter();
            GUIGroup.AddElement(_cursorImage);

            Viewport screen = MainGame.Screen;
            SpriteFont font = AssetManager.MenuFont;
            _bigTimerTextBox = new Textbox(new Vector2((screen.Width - font.MeasureString("3").X) / 2, (screen.Height - font.MeasureString("3").Y) / 2), font, "3");
            _bigTimerTextBox.ApplyColor(Color.White, Color.Black);
            _bigTimerTextBox.Visible = false;
            _bigTimerTextBox.Layer += 0.2f;
            GUIGroup.AddElement(_bigTimerTextBox);

            font = AssetManager.MainFont;
            _timerTextBox = new Textbox(new Vector2(382, 725), font, TIME_BETWEEN_TOUR.ToString() + "sec");
            _timerTextBox.ApplyColor(Color.Red, Color.Black);
            GUIGroup.AddElement(_timerTextBox);

            _currentTeamTextBox = new Textbox(new Vector2(25, 725), font, "Equipe des rouges");
            GUIGroup.AddElement(_currentTeamTextBox);

            _currentTankTextBox = new Textbox(new Vector2(196, 725), font, ".");
            _currentTankTextBox.ApplyColor(Color.Yellow, Color.Black);
            GUIGroup.AddElement(_currentTankTextBox);

            _infoBulle = new Textbox(new Vector2(925, 725), font, "InfoBulle :");
            _infoBulle.ApplyColor(Color.Yellow, Color.Black);
            GUIGroup.AddElement(_infoBulle);

            if (IATrainingMode)
            {
                _fittingScoreTextBox = new Textbox(Vector2.One, font, "Fitness Score : 0 Generation : " + Population.Generation);
                _fittingScoreTextBox.ApplyColor(Color.Yellow, Color.Black);
                GUIGroup.AddElement(_fittingScoreTextBox);
            }

            GUIGroupButtons = new GroupSelection();
            for (int i = 0; i < Enum.GetValues(typeof(Action.eActions)).Length; i++)
            {
                ButtonAction btn;
                if (i == 0)
                {
                    btn = new ButtonAction(this, (Action.eActions)i, Vector2.Zero, Vector2.Zero, AssetManager.MainFont, string.Empty);
                }
                else
                {
                    btn = new ButtonAction(this, (Action.eActions)i, new Vector2(442 + 37 * (i - 1), 725), Vector2.Zero, AssetManager.MainFont, string.Empty);
                }
                GUIGroupButtons.AddElement((IIntegrableMenu)btn);
                btn.OnHover += OnButtonHover;
                btn.OnClick += OnButtonClicked;
            }
            c.OnPositionChange += OnCameraPositionChange;
            #endregion

            #region Remplissage du sac à loot
            _lootBag = new List<Action.eActions>();
            FillLootBag();
            #endregion

            #region Création des équipes
            Teams = new List<Team>();
            Texture2D img = AssetManager.TanksSpriteSheet;
            Team t;
            int numberOfTeam = MainGame.NumberOfTeam;
            int numberOfTankPerTeam = MainGame.NumberOfTank;
            
            for (byte i = 0; i < numberOfTeam; i++)
            {
                eControlType controlType; 
                if (IATrainingMode)
                {
                    controlType = eControlType.NeuralNetwork;
                }
                else
                {
                    controlType = eControlType.Player; // MainGame.ControlTypes[i];
                }
                t = new Team(this, img, numberOfTankPerTeam, i, controlType);
                Teams.Add(t);
                if (IATrainingMode)
                {
                    NeuralNetworkControl nn = (NeuralNetworkControl)t.Control;
                    if (Population.Genomes.Count > 0)
                    {
                        nn.Genome = Population.Genomes[i % Population.Genomes.Count];
                        nn.Genome.OnFitnessScoreChange += Genome_OnFittingScoreChange;
                    }
                    else
                    {
                        Population.Genomes.Add(nn.Genome);
                        nn.Genome.OnFitnessScoreChange += Genome_OnFittingScoreChange;
                    }
                }
                t.OnTankSelectionChange += OnTankSelectionChange;
            }
            t = Teams[IndexTeam];
            t.RefreshCameraOnSelection();
            _currentTeamTextBox.ApplyColor(t.TeamColor, Color.Black);
            _currentTankTextBox.Text = t.Tanks[t.IndexTank].Name;
            RefreshActionButtonInventory();
            #endregion

            base.Load();
        }

        public override void UnLoad()
        {
            _timerSecond.Elapsed -= OnTimerElapsed;
            for (int i = 0; i < Teams.Count; i++)
            {
                Team t = Teams[i];
                t.OnTankSelectionChange -= OnTankSelectionChange;
            }
            if (IATrainingMode)
            {
                Population.OnGenomesChanged -= Population_OnGenomesChanged;
            }
            base.UnLoad();
        }
        #endregion

        #region Changement du Fitting Score
        private void Genome_OnFittingScoreChange(object sender, int previous, int actual)
        {
            _fittingScoreTextBox.Text = "Fitness Score : " + actual + " Generation : " + Population.Generation;
        }
        #endregion

        #region Changements sur la population
        private void Population_OnGenomesChanged(object sender, PopulationManagerEventArgs e)
        {
            int index = 0;
            for (int i = 0; i < Teams.Count; i++)
            {
                Team t = Teams[i];
                if (t.Control is NeuralNetworkControl)
                {
                    NeuralNetworkControl nn = (NeuralNetworkControl)t.Control;
                    nn.Genome = e.Genomes[i];
                    nn.Genome.OnFitnessScoreChange += Genome_OnFittingScoreChange;
                    index++;
                }
            }
        }
        #endregion

        #region Boutons d'inventaire
        public void OnButtonHover(object sender, EventArgs e)
        {
            ButtonAction btn = (ButtonAction)sender;
            _infoBulle.Text = btn.InfoBulle;
        }

        public void OnButtonClicked(Element pSender, ClickType Clicks)
        {
            if (Clicks == ClickType.Left && _inTour)
            {
                ButtonAction btn = (ButtonAction)pSender;
                Team team = Teams[IndexTeam];
                if (team.Control is PlayerControl)
                {
                    if (team.SelectAction(btn.ActionType))
                    {
                        GUIGroupButtons.CurrentSelection = GUIGroupButtons.Elements.FindIndex(b => b == btn);
                    }
                }
            }
        }

        public void RefreshActionButtonSelection(Action.eActions action)
        {
            GUIGroupButtons.CurrentSelection = GUIGroupButtons.Elements.FindIndex(delegate (Element elm)
            {
                return ((ButtonAction)elm).ActionType == action;
            });
        }

        public void RefreshActionButtonInventory()
        {
            int nbBtn = GUIGroupButtons.Elements.Count;
            Dictionary<Action.eActions, int> inv = Teams[IndexTeam].Inventory;
            for (int i = 0; i < nbBtn; i++)
            {
                ButtonAction btn = (ButtonAction)GUIGroupButtons.Elements[i];
                if (inv.ContainsKey(btn.ActionType))
                {
                    btn.Number = inv[btn.ActionType];
                }
            }
        }
        #endregion

        #region Calcul de la position de GUI sur changement de la position de Caméra
        public void OnCameraPositionChange(object sender, Vector3 previous, Vector3 actual)
        {
            Camera cam = (Camera)sender;
            int x = (int)(_gameBarImage.Position.X - _gameBarImage.Origin.X + _gameBarImage.Size.X * (actual.X + MainGame.Screen.Width / 2) / MapSize.X);
            int y = (int)_cursorImage.Position.Y;
            _cursorImage.Position = GetPositionOnMinimap(actual.X + MainGame.Screen.Width / 2);
            Vector2 newCamPos = new Vector2(actual.X, actual.Y);
            Vector2 camOffset = new Vector2(cam.CameraOffset.X, cam.CameraOffset.Y);
            GUIGroup.Position = newCamPos - camOffset;
            GUIGroupButtons.Position = newCamPos - camOffset;
        }
        #endregion

        #region Gestion des tours et sélections de tanks
        /// <summary>
        /// Evènements du timer (1 sec) pour la gestion des tours.
        /// </summary>
        public void OnTimerElapsed(object state, ElapsedEventArgs e)
        {
            _counter--;
            _timerTextBox.Text = _counter.ToString() + " sec";
            if (_counter <= 3)
            {
                _bigTimerTextBox.Text = _counter.ToString();
                _bigTimerTextBox.Visible = true;
            }
            if (_counter <= 0)
            {
                _bigTimerTextBox.Visible = false;
                if (_inTour)
                {
                    _counter = TIME_BETWEEN_TOUR;
                    IndexTeam++;
                    Team t = Teams[IndexTeam];
                    _currentTeamTextBox.Text = t.ToString();
                    _currentTeamTextBox.ApplyColor(t.TeamColor, Color.Black);
                    _timerTextBox.ApplyColor(Color.Red, Color.Black);
                    _bigTimerTextBox.ApplyColor(Color.White, Color.Black);
                    _currentTankTextBox.Text = t.Tanks[t.IndexTank].Name;
                    Drop d = new Drop(this, (Drop.eDropType)utils.MathRnd(0, 3), AssetManager.TanksSpriteSheet, new Vector2(utils.MathRnd(20, (int)MapSize.X - 20), 1), Vector2.Zero, Vector2.One);
                    while (!CanAppear(d))
                    {
                        d.Position = new Vector2(utils.MathRnd(20, (int)MapSize.X - 20), 1);
                    }
                    MainGame.Camera.SetCameraOnActor(d, true, false);
                    OnTourTimerEnd?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    if (IATrainingMode)
                    {
                        _counter = TIME_PER_TOUR_IA_TRAINING;
                    }
                    else
                    {
                        _counter = TIME_PER_TOUR;
                    }
                    _timerTextBox.ApplyColor(Color.Green, Color.Black);
                    _bigTimerTextBox.ApplyColor(Color.Orange, Color.Black);
                    Team t = Teams[IndexTeam];
                    t.NextTank();
                    t.RefreshCameraOnSelection();
                }
                _inTour = !_inTour;
            }
        }

        #region Forçage de fin de tour
        /// <summary>
        /// Force la fin du tour.
        /// </summary>
        public void FinnishTour(bool pFinnishNow = false)
        {
            _timerTextBox.ApplyColor(Color.Orange, Color.Black);
            if (!pFinnishNow)
            {
                if (_counter > TIME_AFTER_ACTION)
                    _counter = TIME_AFTER_ACTION;
            }
            else
            {
                _counter = 3;
            }
        }
        #endregion

        public void OnTankSelectionChange(object sender, byte before, byte value)
        {
            Team t = (Team)sender;
            _currentTankTextBox.Text = t.Tanks[t.IndexTank].Name;
        }
        #endregion

        #region Explosions
        /// <summary>
        /// Créé une explosion à l'emplacement passé par le missille.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pExplosionEventArgs"></param>
        public void CreateExplosion(object sender, ExplosionEventArgs pExplosionEventArgs)
        {
            OnExplosion?.Invoke(sender, pExplosionEventArgs);
            _sndexplosion.Play();
            Color empty = new Color();
            Vector2 pos = pExplosionEventArgs.ExplosionCircle.Location.ToVector2();
            int rad = (int)pExplosionEventArgs.ExplosionCircle.Radius;
            int force = pExplosionEventArgs.Force;
            Texture2D particles = new Texture2D(MainGame.graphics.GraphicsDevice, rad * 2, rad * 2);
            Color[] particlesData = new Color[particles.Width * particles.Height];
            bool beginWater = false;
            int beginWaterPosition = 0;
            for (int x = (int)(pos.X - rad); x <= (int)(pos.X + rad); x++)
            {
                if (x >= 0 && x < MapSize.X)
                {
                    for (int y = (int)(pos.Y - rad); y <= (int)(pos.Y + rad); y++)
                    {
                        if (y >= 0 && y < MapSize.Y)
                        {
                            if (Math.Pow(pos.X - x, 2) + Math.Pow(pos.Y - y, 2) < Math.Pow(rad, 2))
                            {
                                if (IsSolid(new Vector2(x, y)))
                                {
                                    int x2 = (int)(x + rad - pos.X);
                                    int y2 = (int)(y + rad - pos.Y);
                                    int index = (x + y * (int)MapSize.X);
                                    particlesData[x2 + y2 * rad] = MapColors[index];
                                    MapData[x + y * (int)MapSize.X] = 0;
                                    MapColors[x + y * (int)MapSize.X] = empty;
                                }
                            }
                        }
                    }
                    if (pos.Y + rad >= WaterLevel)
                    {
                        if (!IsSolid(new Vector2(x, WaterLevel - 1)))
                        {
                            if (!beginWater)
                            {
                                beginWater = true;
                                beginWaterPosition = x;
                            }
                        }
                        else
                        {
                            if (beginWater)
                            {
                                beginWater = false;
                                WaterPosition.Add(new Rectangle(beginWaterPosition, WaterLevel, x - beginWaterPosition, (int)MapSize.Y - WaterLevel));
                            }
                        }
                    }
                }
            }
            particles.SetData(particlesData);
            CreateMapParticles(particles, pos, force);
            _mapTexture.SetData(MapColors);
        }
        #endregion

        #region Fonctions d'interraction des éléments avec la map
        /// <summary>
        /// Renvoies true si l'acteur se situe en dehors de la map (le ciel est considéré comme dans la map)
        /// </summary>
        /// <param name="actor">Acteur concerné.</param>
        public bool OutOfMap(IActor actor)
        {
            return actor.Position.X < 0 || actor.Position.X > MapSize.X || actor.Position.Y > MapSize.Y;
        }

        /// <summary>
        /// Renvoies l'écart entre la position et le premier point le plus haut en Y sur la map à l'offset X.
        /// </summary>
        /// <param name="pPosition">Position du point qui doit être calculé.</param>
        /// <param name="pXOffset">Offset en X à vérifier par rapport à la position.</param>
        /// <returns>Renvoies un Vector2 correspondant à l'écart entre la position passée et le premier point le plus haut à l'offset X.</returns>
        public Vector2 FindHighestPoint(Vector2 pPosition, int pXOffset) //, int pHeightLimit, int pXOffset)
        {
            // Remonte pour trouver le pixel le plus haut à l'emplacement Position.X + XOffset.
            // Afin d'éviter de trop remonter, le paramètre HeightLimit correspond au nombre de pixel max à remonter
            int y = (int)pPosition.Y;
            while (y > 0) //pPosition.Y - pHeightLimit)
            {
                y--;
                if (MapData[(uint)(pPosition.X + pXOffset + y * MapSize.X)] == 0)
                {
                    break;
                }
            }
            // Redescend pour les cas où le pixel le plus haut est en dessous.
            while (y < MapSize.Y - 1)
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
                result = MapData[(int)(pPosition.X) + (int)(pPosition.Y) * (int)(MapSize.X)] > 0;
            }
            return result;
        }

        /// <summary>
        /// Vérifie qu'il y ai un solide entre les positions passées en paramètre.
        /// </summary>
        /// <param name="pPosition"></param>
        /// <returns></returns>
        public bool IsSolid(Vector2 pPosition, Vector2 pPreviousPosition)
        {
            bool result = false;
            float x = (pPosition.X + pPreviousPosition.X) / 2;
            List<int> yData = new List<int> { (int)pPosition.Y, (int)pPreviousPosition.Y};
            for (int y = yData.Min() - 1; y < yData.Max(); y++)
            {
                if (IsSolid(new Vector2(x, y)))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public Vector2 GetPositionOnMinimap(float pX)
        {
            Vector3 camPos = MainGame.Camera.Position;
            int x = (int)(_gameBarImage.Position.X - _gameBarImage.Origin.X + _gameBarImage.Size.X * pX / MapSize.X);
            int y = (int)_cursorImage.Position.Y;
            return new Vector2(x, y);
        }

        private void CreateMapParticles(Texture2D particles, Vector2 pPosition, int pForce)
        {
            Color empty = new Color();
            int partWidth = particles.Width;
            int partHeight = particles.Height;
            int offsetX = 0;
            int offsetY = 0;
            int rndWidth = 0;
            int rndHeight = 0;
            int startX = (int)pPosition.X - partWidth / 2;
            int startY = (int)pPosition.Y - partHeight / 2;
            Color[] particlesData = new Color[partWidth * partHeight];
            particles.GetData(particlesData);
            for (int l = 0; l < partHeight; l += rndHeight)
            {
                offsetY += rndHeight;
                for (int c = 0; c < partWidth; c += rndWidth)
                {
                    if (offsetX > partWidth)
                    {
                        offsetX = 0;
                    }
                    else
                    {
                        offsetX += rndWidth;
                        rndWidth = utils.MathRnd(10, particles.Width);
                        rndHeight = utils.MathRnd(10, particles.Height);
                        Color[] data = new Color[rndWidth * rndHeight];
                        Texture2D texture = new Texture2D(MainGame.graphics.GraphicsDevice, rndWidth, rndHeight);
                        int minValX = rndWidth;
                        int maxValX = 0;
                        int minValY = rndHeight;
                        int maxValY = 0;
                        bool createParticles = false;
                        for (int x = 0; x < rndWidth; x++)
                        {
                            for (int y = 0; y < rndHeight; y++)
                            {
                                int particlesIndex = (c + x) + (l + y) * partWidth;
                                int index = x + y * rndWidth;
                                if (particlesIndex >= 0 && particlesIndex < particlesData.Length)
                                {
                                    if (particlesData[particlesIndex] != empty)
                                    {
                                        if (Math.Pow(x, 2) + Math.Pow(y, 2) < Math.Pow((rndWidth + rndHeight) / 2, 2))
                                        {
                                            data[index] = particlesData[particlesIndex];
                                            createParticles = true;
                                            if (maxValX < x)
                                            {
                                                maxValX = x;
                                            }
                                            if (minValX > x)
                                            {
                                                minValX = x;
                                            }
                                            if (maxValY < y)
                                            {
                                                maxValY = y;
                                            }
                                            if (minValY > y)
                                            {
                                                minValY = y;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (createParticles)
                        {
                            texture.SetData(data);
                            Vector2 p = new Vector2(startX + c * rndWidth, startY + l * rndHeight);
                            Particle particle = new Particle(this, texture, null, p, new Vector2((maxValX - minValX) / 2, maxValY), Vector2.One);
                            float angle = (float)utils.MathAngle(p, pPosition);
                            particle.Velocity = new Vector2(utils.MathRnd(-10,10), utils.MathRnd(-1, 1));
                            //particle.Velocity = new Vector2((float)Math.Cos(angle) * pForce, (float)Math.Sin(angle) * pForce);
                        }
                    }
                }
            }
        }
        #endregion

        #region Noms de tanks
        public string GetTankName()
        {
            int index = utils.MathRnd(0, _names.Count);
            string result = _names[index];
            _names.RemoveAt(index);
            return result;
        }
        #endregion

        #region Gestion du sac à loot

        /// <summary>
        /// Remplir le sac à loot.
        /// </summary>
        private void FillLootBag()
        {
            for (int i = 0; i < 5; i++)
            {
                _lootBag.Add(Action.eActions.GoldBullet);
                _lootBag.Add(Action.eActions.GoldBombshell);
            }
            for (int i = 0; i < 4; i++)
            {
                _lootBag.Add(Action.eActions.Grenada);
            }
            for (int i = 0; i < 3; i++)
            {
                _lootBag.Add(Action.eActions.GrayMissile);
                _lootBag.Add(Action.eActions.GreenMissile);
                _lootBag.Add(Action.eActions.HelicoTank);
            }
            for (int i = 0; i < 1; i++)
            {
                _lootBag.Add(Action.eActions.SaintGrenada);
                _lootBag.Add(Action.eActions.DropHealth);
                //_lootBag.Add(Action.eActions.Drilling);
            }
        }

        /// <summary>
        /// Récupère le loot sous forme d'un tuple (Type de loot, Quantité).
        /// </summary>
        /// <returns>Retourne un tuple représentant (Type de loot, Quantité).</returns>
        public Tuple<Action.eActions, byte> GetLoot()
        {
            int index = utils.MathRnd(0, _lootBag.Count);
            Action.eActions action = _lootBag[index];
            _lootBag.RemoveAt(index);
            if (_lootBag.Count == 0)
            {
                FillLootBag();
            }
            byte qty = 0;
            switch (action)
            {
                case Action.eActions.GoldBullet:
                case Action.eActions.GoldBombshell:
                    qty = 5;
                    break;
                case Action.eActions.GrayMissile:
                case Action.eActions.GreenMissile:
                    qty = 2;
                    break;
                case Action.eActions.Grenada:
                    qty = 3;
                    break;
                case Action.eActions.SaintGrenada:
                case Action.eActions.HelicoTank:
                case Action.eActions.Drilling:
                    qty = 1;
                    break;
                default:
                    break;
            }
            return new Tuple<Action.eActions, byte> (action, qty);
        }

        #endregion

        #region Vérification qu'un acteur peut apparaitre sur la carte (sans tomber dans l'eau à son apparition).
        public bool CanAppear(IActor pActor)
        {
            bool result = true;
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor other = lstActors[i];
                if (other is Tank)
                {
                    if (utils.Collide(pActor, other))
                    {
                        result = false;
                        break;
                    }
                }
            }
            if (result)
            {
                for (int i = 0; i < lstBuffer.Count; i++)
                {
                    IActor other = lstBuffer[i];
                    if (other is Tank)
                    {
                        if (utils.Collide(pActor, other))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            if (result)
            {
                // Permet d'empêcher d'apparaitre au dessus de l'eau.
                result = FindHighestPoint(pActor.Position, 0).Y < MapSize.Y - 5;
            }
            return result;
        }
        #endregion

        #region Timer de fin de partie
        private void TimerEnd_OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (IATrainingMode)
            {
                Population.Export("Population");
                MainGame.ChangeScene(SceneType.Menu);
            }
            if (Teams.Count == 1)
            {
                MainGame.Winner = Teams[0].ToString();
                MainGame.ChangeScene(SceneType.Victory);
            }
            else
            {
                MainGame.ChangeScene(SceneType.Gameover);
            }
            ((Timer)sender).Elapsed -= TimerEnd_OnElapsed;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Collisions
            // Gère les collisions entre IActors
            List<IActor> lstCollisionnable = lstActors.FindAll(actor => actor is ICollisionnable);
            for (int i = 0; i < lstCollisionnable.Count; i++)
            {
                IActor actor = lstCollisionnable[i];

                for (int j = 0; j < lstCollisionnable.Count; j++)
                {
                    IActor actor2 = lstCollisionnable[j];
                    ICollisionnable col = (ICollisionnable)actor;
                    ICollisionnable col2 = (ICollisionnable)actor2;
                    if (utils.Collide(actor, actor2))
                    {
                        col.TouchedBy(col2);
                        col2.TouchedBy(col);
                    }
                }
            }
            #endregion

            #region Teams
            for (int i = 0; i < Teams.Count; i++)
            {
                if (i == IndexTeam)
                {
                    Teams[i].Update(gameTime, _inTour);
                }
                else
                {
                    Teams[i].Update(gameTime, false);
                }
            }

            Teams.RemoveAll(t => t.Remove);
            #endregion

            #region Victoire / Défaite
            if (Teams.Count < 2 && !_gameFinnished)
            {
                _gameFinnished = true;
                Timer timerEnd = new Timer(3000);
                timerEnd.Enabled = true;
                timerEnd.Elapsed += TimerEnd_OnElapsed;
            }
            #endregion

            #region Arrêt manuel de la partie
            if (Input.OnReleased(Keys.Escape))
            {
                if (IATrainingMode)
                {
                    Population.Export("Population");
                }
                MainGame.ChangeScene(SceneType.Menu);
            }
            #endregion

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_skyTexture, new Vector2(0, MainGame.Screen.Height - _skyHeight), Color.White);
            spriteBatch.Draw(_mapTexture, Vector2.Zero, Color.White);
            base.Draw(spriteBatch, gameTime);
            // TODO dessiner les explosions ici pour que ce soit en premier plan.
        }
        #endregion
    }
}