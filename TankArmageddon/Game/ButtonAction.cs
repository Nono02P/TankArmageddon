using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    #region Enumérations
    public enum eActions : byte
    {
        None,
        iGrayBullet,
        iGrayBombshell,
        GoldBullet,
        GoldBombshell,
        GrayMissile,
        GreenMissile,
        Mine,
        Grenada,
        SaintGrenada,
        iTankBaseBall,
        Helicotank,
        Drilling,
        iDropFuel,
        iWhiteFlag,
    }
    #endregion

    /// <summary>
    /// Boutons d'actions intégrés à la GUI du gameplay.
    /// </summary>
    public class ButtonAction : Button
    {
        /// <summary>
        /// Type d'action que réalise le bouton
        /// </summary>
        public eActions ActionType { get; private set; }

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
                    case eActions.None:
                        infoBulle = "";
                        break;
                    case eActions.iGrayBullet:
                        infoBulle = "Mitraillette : Faible dégâts, faible portée.";
                        break;
                    case eActions.iGrayBombshell:
                        infoBulle = "Obus : Faible dégâts, portée moyenne.";
                        break;
                    case eActions.GoldBullet:
                        infoBulle = "Mitraillette doré : Dégâts moyens, faible portée.";
                        break;
                    case eActions.GoldBombshell:
                        infoBulle = "Obus doré : Dégâts moyens, portée moyenne.";
                        break;
                    case eActions.GrayMissile:
                        infoBulle = "Missile : Dégâts moyens, portée moyenne.";
                        break;
                    case eActions.GreenMissile:
                        infoBulle = "Missile vert : Rend de la vie, grande portée.";
                        break;
                    case eActions.Mine:
                        infoBulle = "Mine : Dégâts moyens, faible portée.";
                        break;
                    case eActions.Grenada:
                        infoBulle = "Grenade : Dégâts moyens, portée moyenne.";
                        break;
                    case eActions.SaintGrenada:
                        infoBulle = "Sainte Grenade : Ne vous ratez pas, ça va faire mal !";
                        break;
                    case eActions.iTankBaseBall:
                        infoBulle = "BaseTank : Transforme le canon en batte de baseball.";
                        break;
                    case eActions.Helicotank:
                        infoBulle = "Hélicotank : Transforme le tank en hélicoptère.";
                        break;
                    case eActions.Drilling:
                        infoBulle = "Foreuse : Permet de creuser sous terre.";
                        break;
                    case eActions.iDropFuel:
                        infoBulle = "Envoi un baril de carburant à l'emplacement spécifié.";
                        break;
                    case eActions.iWhiteFlag:
                        infoBulle = "Equipe qui déclare forfait.";
                        break;
                    default:
                        break;
                }
                return infoBulle;
            }
        }

        public ButtonAction(eActions pActionType, Vector2 pPosition, Vector2 pOrigin, float pScale = 1f, bool pVisible = true) : base(pPosition, pOrigin, pScale, pVisible)
        {
            ActionType = pActionType;
            /*switch (ActionType)
            {
                case eActions.None:
                    break;
                case eActions.iGrayBullet:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet1.png").ImgBox;
                    break;
                case eActions.iGrayBombshell:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet2.png").ImgBox;
                    break;
                case eActions.GoldBullet:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet5.png").ImgBox;
                    break;
                case eActions.GoldBombshell:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet6.png").ImgBox;
                    break;
                case eActions.GrayMissile:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet4.png").ImgBox;
                    break;
                case eActions.GreenMissile:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tank_bullet3.png").ImgBox;
                    break;
                case eActions.Mine:
                    ImgBox = AssetManager.TanksAtlas.Textures.Find(t => t.Name == "tanks_mineOn.png").ImgBox;
                    break;
                case eActions.Grenada:
                    break;
                case eActions.SaintGrenada:
                    break;
                case eActions.iTankBaseBall:
                    break;
                case eActions.Helicotank:
                    break;
                case eActions.Drilling:
                    break;
                case eActions.iDropFuel:
                    break;
                case eActions.iWhiteFlag:
                    break;
                default:
                    break;
            }*/
            ImageDefault = new Texture2D(MainGame.graphics.GraphicsDevice, 30, 30);
            Size = new Vector2(30, 30);
            //Texture2D pImageHover, Texture2D pImagePressed
        }
    }
}
