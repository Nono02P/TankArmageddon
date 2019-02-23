using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
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
        private const int TIME_AFTER_ACTION = 3;
        private const int TIME_BETWEEN_TOUR = 10;
        private const byte NUMBER_OF_TEAMS = 4;
        private const byte NUMBER_OF_TANK_PER_TEAM = 5;
        #endregion

        #region Evènements
        public event ExplosionHandler OnExplosion;
        public event EventHandler OnTourTimerEnd;
        #endregion

        #region Variables privées
        private List<string> _names = new List<string>()
            {
                "Almex",
                "Anata",
                "Arnkil",
                "Asthegor",
                "Azharis",
                "Bertho",
                "BreakingBardo",
                "Cehem",
                "David",
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
        private Textbox _timerTextBox;
        private List<Team> _teams;
        private Textbox _currentTeamTextBox;
        private Textbox _currentTankTextBox;
        private Textbox _infoBulle;
        private Timer _timerSecond;
        private int _counter = TIME_PER_TOUR;
        private bool _inTour = false;
        private Texture2D _mapTexture;
        private float[] _perlinNoise;
        private int _indexTeam = 0;
        private Image _gameBarImage;
        private Image _cursorImage;
        private List<Action.eActions> _lootBag;
        private SoundEffect _sndexplosion;
        #endregion

        #region Propriétés
        public Group GUIGroup { get; private set; }
        public GroupSelection GUIGroupButtons { get; private set; }
        public int WaterLevel { get { return (int)(MapSize.Y - MapSize.Y * 0.05f); } }
        public Vector2 MapSize { get; private set; } = new Vector2(4096, MainGame.Screen.Height - AssetManager.GameBottomBar.Height);
        public byte[] MapData { get; private set; }
        public Color[] MapColors { get; private set; }
        public int IndexTeam
        {
            get { return _indexTeam; }
            private set
            {
                if (_teams.Count > 0 && _indexTeam != value)
                {
                    _indexTeam = value % _teams.Count;
                    _teams[_indexTeam].RefreshCameraOnSelection();
                    RefreshActionButton();
                }
            }
        }
        #endregion

        #region Constructeur
        public Gameplay() { }
        #endregion

        #region Load/Unload
        public override void Load()
        {
            #region Démarrage des musiques
            sndMusic = AssetManager.mscGameplay;
            MediaPlayer.Play(sndMusic);
            MediaPlayer.IsRepeating = true;
            #endregion

            #region Ajout des bruitages
            _sndexplosion = AssetManager.sndExplosion;
            #endregion

            #region Création de la map
            _perlinNoise = PerlinNoise.Generate1DMap((int)MapSize.X, 550f);
            //
            //float[] perlinLuminosity = PerlinNoise.Generate2DMap(MapSize, 550f);
            /*float[] perlinColor1 = PerlinNoise.Generate1DMap((int)MapSize.X, 100f);
            float[] perlinColor2 = PerlinNoise.Generate1DMap((int)MapSize.X, 300f);
            float[] perlinColor3 = PerlinNoise.Generate1DMap((int)MapSize.X, 5000f);*/

            _mapTexture = new Texture2D(MainGame.spriteBatch.GraphicsDevice, (int)MapSize.X, (int)MapSize.Y);
            MapData = new byte[(int)(MapSize.X * MapSize.Y)];
            MapColors = new Color[MapData.Length];
            for (ushort i = 0; i < _perlinNoise.Length; i++)
            {
                //
                float noiseVal = _perlinNoise[i];
                /*float noiseVal1 = perlinColor1[i];
                float noiseVal2 = perlinColor2[i];
                float noiseVal3 = perlinColor3[i];*/
                
                ushort x = (ushort)(i % MapSize.X);
                ushort h = (ushort)Math.Floor((noiseVal + 1) * MapSize.Y * 0.5f);
                h = (ushort)MathHelper.Clamp(h, 0, WaterLevel + 1);
                /*ushort h1 = (ushort)Math.Floor((noiseVal1 + 1) * MapSize.Y * 0.5f);
                ushort h2 = (ushort)Math.Floor((noiseVal2 + 1) * MapSize.Y * 0.5f);
                ushort h3 = (ushort)Math.Floor((noiseVal3 + 1) * MapSize.Y * 0.5f);*/
                //for (ushort y = h; y < MapSize.Y; y++)
                for (ushort y = h; y < MapSize.Y; y++)
                {
                    /*float max = (new List<float>() { h1, h2, h3 }).Max();
                    Color color;
                    if (max == h1)
                    {
                        color = Color.Lerp(Color.WhiteSmoke, Color.White, _perlinNoise[y]);
                    }
                    else if (max == h2)
                    {
                        color = Color.Lerp(Color.DarkGray, Color.Gray, _perlinNoise[y]);
                    }
                    else
                    {
                        color = Color.Lerp(Color.DarkGreen, Color.Green, _perlinNoise[y]);
                    }*/
                    uint index = (uint)(x + y * MapSize.X);
                    if (y <= WaterLevel)
                    {
                        MapColors[index] = Color.Green; //color;
                        MapData[index] = 1;
                    }
                    else
                    {
                        MapColors[index] = Color.DarkBlue;
                    }
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
            _counter = TIME_BETWEEN_TOUR;
            #endregion

            #region Création des éléments de GUI
            GUIGroup = new Group();
            Texture2D texture = AssetManager.GameBottomBar;
            _gameBarImage = new Image(texture, new Vector2(MainGame.Screen.Width / 2 , MainGame.Screen.Height - texture.Height / 2));
            _gameBarImage.SetOriginToCenter();
            GUIGroup.AddElement(_gameBarImage);

            texture = AssetManager.Cursor;
            _cursorImage = new Image(texture, new Vector2(_gameBarImage.Position.X - _gameBarImage.Origin.X + (_gameBarImage.Position.X / MapSize.X), _gameBarImage.Position.Y - texture.Height * 0.75f));
            _cursorImage.SetOriginToCenter();
            GUIGroup.AddElement(_cursorImage);

            SpriteFont font = AssetManager.MainFont;
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
            _teams = new List<Team>();
            Texture2D img = AssetManager.TanksSpriteSheet;
            Team t;
            for (byte i = 0; i < NUMBER_OF_TEAMS; i++)
            {
                t = new Team(this, img, NUMBER_OF_TANK_PER_TEAM, i);
                _teams.Add(t);
                t.OnTankSelectionChange += OnTankSelectionChange;
            }
            t = _teams[IndexTeam];
            t.RefreshCameraOnSelection();
            _currentTeamTextBox.ApplyColor(t.TeamColor, Color.Black);
            _currentTankTextBox.Text = t.Tanks[t.IndexTank].Name;
            RefreshActionButton();
            #endregion

            base.Load();
        }
        #endregion

        #region Boutons d'inventaire
        public void OnButtonHover(Element pSender)
        {
            ButtonAction btn = (ButtonAction)pSender;
            _infoBulle.Text = btn.InfoBulle;
        }

        public void OnButtonClicked(Element pSender, ClickType Clicks)
        {
            if (Clicks == ClickType.Left)
            {
                ButtonAction btn = (ButtonAction)pSender;
                if (btn.Number != 0)
                {
                    GUIGroupButtons.CurrentSelection = GUIGroupButtons.Elements.FindIndex(b => b == btn);
                    _teams[_indexTeam].SelectAction(btn.ActionType);
                }
            }
        }

        public void RefreshActionButton()
        {
            int nbBtn = GUIGroupButtons.Elements.Count;
            for (int i = 0; i < nbBtn; i++)
            {
                ButtonAction btn = (ButtonAction)GUIGroupButtons.Elements[i];
                Dictionary<Action.eActions, int> inv = _teams[_indexTeam].Inventory;
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
            int x = (int)(_gameBarImage.Position.X - _gameBarImage.Origin.X + _gameBarImage.Size.X * (actual.X + MainGame.Screen.Width / 2) / MapSize.X);
            int y = (int)_cursorImage.Position.Y;
            _cursorImage.Position = GetPositionOnMinimap(actual.X + MainGame.Screen.Width / 2);
            Vector2 newCamPos = new Vector2(actual.X, actual.Y);
            GUIGroup.Position = newCamPos;
            GUIGroupButtons.Position = newCamPos;
        }
        #endregion

        #region Gestion des tours et sélections de tanks
        /// <summary>
        /// Evènements du timer (1 sec) pour la gestion des tours.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="e"></param>
        public void OnTimerElapsed(object state, ElapsedEventArgs e)
        {
            _counter--;
            _timerTextBox.Text = _counter.ToString() + " sec";
            if (_counter <= 0)
            {
                if (_inTour)
                {
                    _counter = TIME_BETWEEN_TOUR;
                    IndexTeam++;
                    Team t = _teams[IndexTeam];
                    _currentTeamTextBox.Text = t.ToString();
                    _currentTeamTextBox.ApplyColor(t.TeamColor, Color.Black);
                    _timerTextBox.ApplyColor(Color.Red, Color.Black);
                    _currentTankTextBox.Text = t.Tanks[t.IndexTank].Name;
                    Drop d = new Drop(this, (Drop.eDropType)utils.MathRnd(0, 3), AssetManager.TanksSpriteSheet, new Vector2(utils.MathRnd(20, (int)MapSize.X - 20), 10), Vector2.Zero, Vector2.One);
                    MainGame.Camera.SetCameraOnActor(d);
                    OnTourTimerEnd?.Invoke(this, EventArgs.Empty);
                    // TODO : Vérifier que le Garbage Collector soit utile.
                    GC.Collect();
                }
                else
                {
                    _counter = TIME_PER_TOUR;
                    _timerTextBox.ApplyColor(Color.Green, Color.Black);
                    Team t = _teams[IndexTeam];
                    t.RefreshCameraOnSelection();
                }
                _inTour = !_inTour;
            }
        }

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
                } 
            }
            particles.SetData(particlesData);
            CreateMapParticles(particles, pos, force);
            _mapTexture.SetData(MapColors);
        }
        #endregion

        #region Fonctions d'interraction des éléments avec la map

        public Vector2 Normalisation(Vector2 pPosition)
        {
            Vector2 avg = new Vector2();
            for (int x = (int)pPosition.X - 5; x < pPosition.X + 5; x++)
            {
                for (int y = (int)pPosition.Y - 5; y < pPosition.Y + 5; y++)
                {
                    Vector2 p = new Vector2(x, y);
                    if (IsSolid(p))
                    {
                        avg -= p;
                    }
                }
            }
            int length = (int)Math.Sqrt(avg.X * avg.X + avg.Y * avg.Y);
            return avg / length;
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
                result = MapData[(uint)(pPosition.X) + (uint)(pPosition.Y) * (uint)(MapSize.X)] > 0;
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
                _lootBag.Add(Action.eActions.Drilling);
            }
            for (int i = 0; i < 4; i++)
            {
                _lootBag.Add(Action.eActions.Mine);
                _lootBag.Add(Action.eActions.Grenada);
            }
            for (int i = 0; i < 3; i++)
            {
                _lootBag.Add(Action.eActions.GrayMissile);
                _lootBag.Add(Action.eActions.GreenMissile);
            }
            for (int i = 0; i < 1; i++)
            {
                _lootBag.Add(Action.eActions.SaintGrenada);
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
                case Action.eActions.Mine:
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

        #region Forçage de fin de tour
        /// <summary>
        /// Force la fin du tour.
        /// </summary>
        public void FinnishTour(bool pFinnishNow = false)
        {
            if (!pFinnishNow)
            {
                if (_counter > TIME_AFTER_ACTION)
                    _counter = TIME_AFTER_ACTION;
            }
            else
            {
                _counter = TIME_BETWEEN_TOUR;
            }
        }
        #endregion

        #region Vérification de collision entre Tanks (pour le démarrage)
        public bool CanAppear(Tank pTank)
        {
            bool result = true;
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor actor = lstActors[i];
                if (actor is Tank)
                {
                    if (utils.Collide(pTank, actor))
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
                    IActor actor = lstBuffer[i];
                    if (actor is Tank)
                    {
                        if (utils.Collide(pTank, actor))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Collisions
            // Gère les collisions entre IActors
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor actor = lstActors[i];
                if (actor is ICollisionnable)
                {
                    for (int j = 0; j < lstActors.Count; j++)
                    {
                        IActor actor2 = lstActors[j];
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

            #region Teams
            for (int i = 0; i < _teams.Count; i++)
            {
                if (i == IndexTeam)
                {
                    _teams[i].Update(gameTime, _inTour);
                }
                else
                {
                    _teams[i].Update(gameTime, false);
                }
            }

            _teams.RemoveAll(t => t.Remove);
            #endregion

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_mapTexture, Vector2.Zero, Color.White);
            base.Draw(spriteBatch, gameTime);
            // TODO dessiner les explosions ici pour que ce soit en premier plan.
        }
        #endregion
    }
}