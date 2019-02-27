using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    /// <summary>
    /// Boutons d'actions intégrés à la GUI du gameplay.
    /// </summary>
    public class ButtonAction : Button
    {
        #region Variables privées
        private int _number;
        #endregion

        #region Propriétés
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                if (value != -1)
                { TextBox.Text = value.ToString(); }
                //else
                //{ TextBox.Text = "∞"; }
            }
        }

        /// <summary>
        /// Type d'action que réalise le bouton
        /// </summary>
        public Action.eActions ActionType { get; private set; }

        /// <summary>
        /// Texte à afficher dans l'infobulle sur survol de la souris
        /// </summary>
        public string InfoBulle
        {
            get
            {
                string infoBulle = "";
                switch (ActionType)
                {
                    case Action.eActions.None:
                        infoBulle = "";
                        break;
                    case Action.eActions.iGrayBullet:
                        infoBulle = "Mitraillette : Faible dégâts, faible portée.";
                        break;
                    case Action.eActions.iGrayBombshell:
                        infoBulle = "Obus : Faible dégâts, portée moyenne.";
                        break;
                    case Action.eActions.GoldBullet:
                        infoBulle = "Mitraillette doré : Dégâts moyens, faible portée.";
                        break;
                    case Action.eActions.GoldBombshell:
                        infoBulle = "Obus doré : Dégâts moyens, portée moyenne.";
                        break;
                    case Action.eActions.GrayMissile:
                        infoBulle = "Missile : Dégâts moyens, portée moyenne.";
                        break;
                    case Action.eActions.GreenMissile:
                        infoBulle = "Missile : Dégâts importants, portée moyenne.";
                        break;
                    case Action.eActions.iMine:
                        infoBulle = "Mine : Dégâts moyens, faible portée.";
                        break;
                    case Action.eActions.Grenada:
                        infoBulle = "Grenade : Dégâts moyens, portée moyenne.";
                        break;
                    case Action.eActions.SaintGrenada:
                        infoBulle = "Sainte Grenade : Ne vous ratez pas, ça va faire mal !";
                        break;
                    case Action.eActions.iTankBaseBall:
                        infoBulle = "BaseTank : Transforme le canon en batte de baseball.";
                        break;
                    case Action.eActions.HelicoTank:
                        infoBulle = "Hélicotank : Transforme le tank en hélicoptère.";
                        break;
                    case Action.eActions.Drilling:
                        infoBulle = "Foreuse : Permet de creuser sous terre.";
                        break;
                    case Action.eActions.iDropFuel:
                        infoBulle = "Envoi un baril de carburant à l'emplacement spécifié.";
                        break;
                    default:
                        break;
                }
                return infoBulle;
            }
        }

        public Gameplay Parent { get; private set; }
        #endregion

        #region Constructeur
        public ButtonAction(Gameplay pParent, Action.eActions pActionType, Vector2 pPosition, Vector2 pOrigin, SpriteFont pFont, string pText, float pScale = 1f, bool pVisible = true) : base(pPosition, pOrigin, pScale, pVisible, pFont, pText)
        {
            Parent = pParent;
            ActionType = pActionType;
            Size = new Vector2(30, 30);
            ImageDefault = new Texture2D(MainGame.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            switch (ActionType)
            {
                case Action.eActions.None:
                    Size = new Vector2(MainGame.Screen.Width, Parent.MapSize.Y);
                    break;
                case Action.eActions.iGrayBullet:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                    break;
                case Action.eActions.iGrayBombshell:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet2.png").ImgBox;
                    break;
                case Action.eActions.GoldBullet:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet5.png").ImgBox;
                    break;
                case Action.eActions.GoldBombshell:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                    break;
                case Action.eActions.GrayMissile:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                    break;
                case Action.eActions.GreenMissile:
                    Scale = 0.5f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
                    break;
                case Action.eActions.iMine:
                    Scale = 0.40f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_mineOn.png").ImgBox;
                    break;
                case Action.eActions.Grenada:
                    Scale = 0.12f;
                    ImageDefault = AssetManager.Grenada;
                    break;
                case Action.eActions.SaintGrenada:
                    Scale = 0.07f;
                    ImageDefault = AssetManager.SaintGrenada;
                    break;
                case Action.eActions.iTankBaseBall:
                    break;
                case Action.eActions.HelicoTank:
                    break;
                case Action.eActions.Drilling:
                    break;
                case Action.eActions.iDropFuel:
                    Scale = 0.25f;
                    ImageDefault = AssetManager.TanksSpriteSheet;
                    ImageBoxDefault = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_barrelRed.png").ImgBox;
                    break;
                default:
                    break;
            }
            ImageBoxHover = ImageBoxDefault;
            ImageBoxPressed = ImageBoxDefault;
            //ImageBoxSelected = ImageBoxDefault;
            //Texture2D pImageHover, Texture2D pImagePressed
        }
        #endregion
    }
}