using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Xml.Serialization;

namespace TankArmageddon
{
    class AssetManager
    {
        #region Musiques
        public static Song sndMusicMenu { get; private set; }
        public static Song sndMusicGameplay { get; private set; }
        public static Song sndMusicGameover { get; private set; }
        public static Song sndMusicVictory { get; private set; }
        #endregion

        #region Font
        public static SpriteFont MainFont { get; private set; }
        #endregion

        #region Textures
        public static Texture2D GameBottomBar { get; private set; }
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Button { get; private set; }
        public static Texture2D Parachute { get; private set; }
        public static Texture2D TanksSpriteSheet { get; private set; }
        public static XmlTextureAtlas TanksAtlas { get; private set; }
        #endregion

        #region Load
        public static void Load(ContentManager pContent)
        {
            /*sndMusicMenu = pContent.Load<Song>("cool");
            sndMusicGameplay = pContent.Load<Song>("techno");
            sndMusicGameover = pContent.Load<Song>("cool");
            sndMusicVictory = pContent.Load<Song>("cool");*/
            MainFont = pContent.Load<SpriteFont>("_Font/MainFont");
            //Button = pContent.Load<Texture2D>("button");
            GameBottomBar = pContent.Load<Texture2D>("_GUI/GameBottomBar");
            Cursor = pContent.Load<Texture2D>("_GUI/Cursor");
            // Parachute = pContent.Load<Texture2D>("_Game/Parachute");
            TanksSpriteSheet = pContent.Load<Texture2D>("_Game/tanks_spritesheetRetina");
            XmlSerializer TankSpriteSheetSer = new XmlSerializer(typeof(XmlTextureAtlas));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes("Content/_Game/tanks_spritesheetRetina.xml"));
            TanksAtlas = (XmlTextureAtlas)TankSpriteSheetSer.Deserialize(stream);
        }
        #endregion
    }
}
