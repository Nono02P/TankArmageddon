using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public IControl Control { get; private set; }
        #endregion

        #region Constructeur
        public Team(Gameplay pParent, Texture2D pImage, int pNumberOfTanks, int pTeamNumber, eControlType pControlType = eControlType.Player)
        {
            #region Création de l'inventaire
            Inventory = new Dictionary<Action.eActions, int>();
            for (int i = 1; i < Enum.GetValues(typeof(Action.eActions)).GetLength(0); i++)
            {
                Inventory.Add((Action.eActions)i, 0);
            }
            Inventory[Action.eActions.Grenada] = 6;
            Inventory[Action.eActions.GoldBullet] = 2;
            Inventory[Action.eActions.iGrayBullet] = -1;
            Inventory[Action.eActions.iGrayBombshell] = -1;
            Inventory[Action.eActions.iMine] = -1;
            //Inventory[Action.eActions.iTankBaseBall] = -1;
            Inventory[Action.eActions.iDropFuel] = -1;
            #endregion

            #region Initialisation des valeurs
            Parent = pParent;
            Tanks = new List<Tank>();
            string tankName = "";
            string tankCannon = "tanks_turret2.png";
            string tankWheel = "tanks_tankTracks1.png";
            switch (pTeamNumber % 4)
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
            #endregion

            #region Instanciation du type de contrôle
            switch (pControlType)
            {
                case eControlType.NeuralNetwork:
                    Control = new NeuralNetworkControl(this);
                    break;
                case eControlType.Player:
                    Control = new PlayerControl(this);
                    break;
                default:
                    break;
            }
            #endregion

            #region Création des tanks
            for (int i = 0; i < pNumberOfTanks; i++)
            {
                Tank t = new Tank(this, TeamColor, Parent.GetTankName(), pImage, imgTank, imgCannon, imgWheel, new Vector2(utils.MathRnd(40, (int)Parent.MapSize.X - 40), 1), new Vector2(imgTank.Width / 2, imgTank.Height / 2), Vector2.One * 0.5f);
                while (!Parent.CanAppear(t))
                {
                    t.Position = new Vector2(utils.MathRnd(40, (int)Parent.MapSize.X - 40), 1);
                }
                Tanks.Add(t);
            }
            #endregion
        }
        #endregion

        #region Acquisition de Loot
        public void OpenLoot()
        {
            Tuple<Action.eActions, byte> loot = Parent.GetLoot();
            Inventory[loot.Item1] += loot.Item2;
        }
        #endregion

        #region Déplace la caméra sur le tank sélectionné
        public void RefreshCameraOnSelection()
        {
            if (Tanks.Count > 0 && IndexTank < Tanks.Count)
            {
                Tank t = Tanks[IndexTank];
                Camera cam = MainGame.Camera;
                if (cam.Position.Y < 0 && t.Position.Y - t.BoundingBox.Height > 0)
                {
                    cam.SetCameraOnActor(t, HAlign.Center, VAlign.Bottom);
                }
                else
                {
                    cam.SetCameraOnActor(t, true, t.Position.Y - t.BoundingBox.Height < 0 || cam.Position.Y < 0);
                }
            }
        }
        #endregion

        #region Sélectionne le tank suivant
        public void NextTank()
        {
            IndexTank++;
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

        #region Sélection de l'action
        public bool SelectAction(Action.eActions actions)
        {
            if (Control is PlayerControl)
            {
                Tank CurrentTank = Tanks[IndexTank];
                CurrentTank.SelectedAction = actions;
                return true;
            }
            return false;
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime, bool pCanPlay)
        {
            if (Tanks.Count > 0)
            {
                IndexTank = (byte)(IndexTank % Tanks.Count);
                Tank CurrentTank = Tanks[IndexTank];
                
                Control.Update(pCanPlay);

                // Sélectionne le tank suivant
                if (Control.OnPressedN && pCanPlay)
                {
                    CurrentTank.IsControlled = false;
                    IndexTank++;
                }

                // Si le joueur peut jouer (si c'est son tour) gère les entrées claviers.
                CurrentTank = Tanks[IndexTank];
                CurrentTank.IsControlled = pCanPlay;
                CurrentTank.Space = Control.IsDownSpace;
                CurrentTank.Left = Control.IsDownLeft;
                CurrentTank.Right = Control.IsDownRight;
                CurrentTank.Up = Control.IsDownUp;
                CurrentTank.Down = Control.IsDownDown;

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