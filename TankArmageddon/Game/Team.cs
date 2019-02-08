using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TankArmageddon
{
    public class Team
    {
        #region Evènements
        public event onByteChange OnTankSelectionChange;
        #endregion


        private byte _missileForce;

        private byte _indexTank;

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

        public Team(Gameplay pParent, Texture2D pImage, int pNumberOfTanks, int pTeamNumber)
        {
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
                int index = utils.MathRnd(0, Parent.Names.Count);
                Tank t = new Tank(this, TeamColor, Parent.Names[i], pImage, imgTank, imgCannon, imgWheel, new Vector2(utils.MathRnd(40, (int)Parent.MapSize.X - 40), 1), new Vector2(imgTank.Width / 2, imgTank.Height / 2), Vector2.One * 0.5f);
                Tanks.Add(t);
                Parent.Names.RemoveAt(i);
            }
        }

        public void RefreshCameraOnSelection()
        {
            if (Tanks.Count > 0 && IndexTank < Tanks.Count)
                MainGame.Camera.CenterOn(Tanks[IndexTank]);
        }

        public void Update(GameTime gameTime, bool pCanPlay)
        {
            if (Tanks.Count > 0)
            {
                // Sélectionne le tank suivant
                if (Input.OnPressed(Keys.N) && pCanPlay)
                {
                    IndexTank++;
                }
                IndexTank = (byte)(IndexTank % Tanks.Count);

                // Si le joueur peut jouer (si c'est son tour) gère les entrées claviers.
                Tank CurrentTank = Tanks[IndexTank];
                if (pCanPlay)
                {
                    if (Input.IsDown(Keys.Space))
                    {
                        _missileForce++;
                    }
                    else if (_missileForce >= 100 || Input.OnReleased(Keys.Space))
                    {
                        CurrentTank.Shoot(_missileForce, Tank.eBulletType.GrayBullet);
                        _missileForce = 0;
                    }
                }

                CurrentTank.Left = Input.IsDown(Keys.Left) && pCanPlay;
                CurrentTank.Right = Input.IsDown(Keys.Right) && pCanPlay;
                CurrentTank.Up = Input.IsDown(Keys.Up) && pCanPlay;
                CurrentTank.Down = Input.IsDown(Keys.Down) && pCanPlay;

                Tanks.RemoveAll(t => t.Remove);
            }
        }

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
    }
}
