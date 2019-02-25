using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TankArmageddon
{
    public class Team
    {
        #region Evènements
        public event onByteChange OnTankSelectionChange;
        #endregion

        #region Variables privées
        private byte _indexTank;
        #endregion

        #region Propriétés
        public Dictionary<Action.eActions, int> Inventory { get; private set; }
        public byte IndexTank
        {
            get { return _indexTank; }
            private set
            {
                if (Tanks.Count > 0 && _indexTank != value)
                {
                    byte before = _indexTank;
                    _indexTank = (byte)(value % Tanks.Count);
                    RefreshCameraOnSelection();
                    OnTankSelectionChange?.Invoke(this, before, value);
                }
            }
        }

        public Gameplay Parent { get; private set; }
        public List<Tank> Tanks { get; private set; }
        public Color TeamColor { get; private set; }
        public bool Remove { get; private set; }
        #endregion

        #region Constructeur
        public Team(Gameplay pParent, Texture2D pImage, int pNumberOfTanks, int pTeamNumber)
        {
            Inventory = new Dictionary<Action.eActions, int>();
            for (int i = 1; i < Enum.GetValues(typeof(Action.eActions)).GetLength(0); i++)
            {
                Inventory.Add((Action.eActions)i, 10);
            }
            Inventory[Action.eActions.iGrayBullet] = -1;
            Inventory[Action.eActions.iGrayBombshell] = -1;
            Inventory[Action.eActions.iTankBaseBall] = -1;
            Inventory[Action.eActions.iDropFuel] = -1;

            Parent = pParent;
            Tanks = new List<Tank>();
            string tankName = "";
            string tankCannon = "tanks_turret2.png";
            string tankWheel = "tanks_tankTracks1.png";
            switch (pTeamNumber)
            {
                case 0:
                    TeamColor = Color.Red;
                    tankName = "tanks_tankDesert_body1.png";
                    break;
                case 1:
                    TeamColor = Color.Green;
                    tankName = "tanks_tankGreen_body1.png";
                    break;
                case 2:
                    TeamColor = Color.Gray;
                    tankName = "tanks_tankGrey_body1.png";
                    break;
                case 3:
                    TeamColor = Color.Yellow;
                    tankName = "tanks_tankNavy_body1.png";
                    break;
                default:
                    break;
            }
            Rectangle imgTank = AssetManager.TanksAtlas.Textures.Find(t => t.Name == tankName).ImgBox;
            Rectangle imgCannon = AssetManager.TanksAtlas.Textures.Find(t => t.Name == tankCannon).ImgBox;
            Rectangle imgWheel = AssetManager.TanksAtlas.Textures.Find(t => t.Name == tankWheel).ImgBox;

            for (int i = 0; i < pNumberOfTanks; i++)
            {
                Tank t = new Tank(this, TeamColor, Parent.GetTankName(), pImage, imgTank, imgCannon, imgWheel, new Vector2(utils.MathRnd(40, (int)Parent.MapSize.X - 40), 1), new Vector2(imgTank.Width / 2, imgTank.Height / 2), Vector2.One * 0.5f);
                while (!Parent.CanAppear(t))
                {
                    t.Position = new Vector2(utils.MathRnd(40, (int)Parent.MapSize.X - 40), 1);
                }
                Tanks.Add(t);
            }
        }
        #endregion
        
        #region Acquisition de Loot
        public void OpenLoot()
        {
            Tuple<Action.eActions, byte> loot = Parent.GetLoot();
            Inventory[loot.Item1] += loot.Item2;
        }
        #endregion

        #region Déplace la caméra
        public void RefreshCameraOnSelection()
        {
            if (Tanks.Count > 0 && IndexTank < Tanks.Count)
                MainGame.Camera.SetCameraOnActor(Tanks[IndexTank]);
        }
        #endregion

        #region Nom de team
        public override string ToString()
        {
            string result = "";
            if (TeamColor == Color.Red)
            {
                result = "rouges";
            }
            if (TeamColor == Color.Green)
            {
                result = "verts";
            }
            if (TeamColor == Color.Gray)
            {
                result = "gris";
            }
            if (TeamColor == Color.Yellow)
            {
                result = "jaunes";
            }
            return "Equipe des " + result; 
        }
        #endregion

        #region Sélection de l'action.
        public void SelectAction(Action.eActions actions)
        {
            Tank CurrentTank = Tanks[IndexTank];
            CurrentTank.SelectedAction = actions;
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime, bool pCanPlay)
        {
            if (Tanks.Count > 0)
            {
                Tank CurrentTank = Tanks[IndexTank];
                // Sélectionne le tank suivant
                if (Input.OnPressed(Keys.N) && pCanPlay)
                {
                    CurrentTank.IsControlled = false;
                    IndexTank++;
                }
                IndexTank = (byte)(IndexTank % Tanks.Count);

                // Si le joueur peut jouer (si c'est son tour) gère les entrées claviers.
                CurrentTank = Tanks[IndexTank];
                CurrentTank.IsControlled = pCanPlay;
                CurrentTank.Space = Input.IsDown(Keys.Space);
                CurrentTank.Left = Input.IsDown(Keys.Left);
                CurrentTank.Right = Input.IsDown(Keys.Right);
                CurrentTank.Up = Input.IsDown(Keys.Up);
                CurrentTank.Down = Input.IsDown(Keys.Down);

                Tanks.RemoveAll(t => t.Remove);
                if (Tanks.Count == 0)
                {
                    Remove = true;
                }
            }
        }
        #endregion
    }
}