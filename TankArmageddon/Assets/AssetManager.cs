using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TankArmageddon
{
    public class AssetManager
    {
        #region Musiques
        public static Song mscMenu { get; private set; }
        public static Song mscGameplay { get; private set; }
        public static Song mscGameover { get; private set; }
        public static Song mscVictory { get; private set; }
        #endregion

        #region Sons
        public static SoundEffect sndShoot { get; private set; }
        public static SoundEffect sndExplosion { get; private set; }
        public static SoundEffect sndSaintGrenada { get; private set; }
        #endregion

        #region Font
        public static SpriteFont MainFont { get; private set; }
        public static SpriteFont MenuFont { get; private set; }
        #endregion

        #region Textures
        public static Texture2D Menu { get; private set; }
        public static Texture2D HowToPlay { get; private set; }
        public static Texture2D Victory { get; private set; }
        public static Texture2D Gameover { get; private set; }
        public static Texture2D GameBottomBar { get; private set; }
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Crosshair { get; private set; }
        public static Texture2D Parachute { get; private set; }
        public static Texture2D Grenada { get; private set; }
        public static Texture2D SaintGrenada { get; private set; }
        public static List<Rectangle> ParachutesImgBox { get; private set; }
        public static Texture2D TanksSpriteSheet { get; private set; }
        public static XmlTextureAtlas TanksAtlas { get; private set; }
        public static Texture2D IconsSpriteSheet { get; private set; }
        #endregion

        #region Load
        public static void Load(ContentManager pContent)
        {
            #region Musiques
            mscMenu = pContent.Load<Song>("_Musics/Heros");
            mscGameplay = pContent.Load<Song>("_Musics/Grandioso");
            mscVictory = pContent.Load<Song>("_Musics/Heros");
            mscGameover = pContent.Load<Song>("_Musics/Heros");
            #endregion

            #region Sons
            sndShoot = pContent.Load<SoundEffect>("_Game/Shoot");
            sndExplosion = pContent.Load<SoundEffect>("_Game/Explosion");
            sndSaintGrenada = pContent.Load<SoundEffect>("_Game/Hallelujah");
            #endregion

            #region Font
            MainFont = pContent.Load<SpriteFont>("_Font/MainFont");
            MenuFont = pContent.Load<SpriteFont>("_Font/MenuFont");
            #endregion

            #region Textures
            Menu = pContent.Load<Texture2D>("_Background/Menu");
            HowToPlay = pContent.Load<Texture2D>("_Background/HowToPlay");
            Gameover = pContent.Load<Texture2D>("_Background/Defeat");
            Victory = pContent.Load<Texture2D>("_Background/Victory");
            GameBottomBar = pContent.Load<Texture2D>("_GUI/GameBottomBar");
            Cursor = pContent.Load<Texture2D>("_GUI/Cursor");
            Crosshair = pContent.Load<Texture2D>("_Game/crosshair184");
            Parachute = pContent.Load<Texture2D>("_Game/Parachute");
            ParachutesImgBox = new List<Rectangle>();
            Rectangle img;
            int nbCol = 3;
            int nbLines = 3;
            int pWidth = Parachute.Width / nbCol;
            int pHeight = Parachute.Height / nbLines;
            for (int x = 0; x < nbCol; x++)
            {
                for (int y = 0; y < nbLines; y++)
                {
                    img = new Rectangle(x * pWidth, y * pHeight, pWidth, pHeight);
                    ParachutesImgBox.Add(img);
                }
            }
            Grenada = pContent.Load<Texture2D>("_Game/Grenada");
            SaintGrenada = pContent.Load<Texture2D>("_Game/SaintGrenada");
            TanksSpriteSheet = pContent.Load<Texture2D>("_Game/tanks_spritesheetRetina");
            XmlSerializer TankSpriteSheetSer = new XmlSerializer(typeof(XmlTextureAtlas));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes("Content/_Game/tanks_spritesheetRetina.xml"));
            TanksAtlas = (XmlTextureAtlas)TankSpriteSheetSer.Deserialize(stream);
            IconsSpriteSheet = pContent.Load<Texture2D>("_Game/Icons");
            #endregion
        }
        #endregion
    }
}