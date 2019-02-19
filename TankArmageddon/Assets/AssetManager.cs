﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TankArmageddon
{
    class AssetManager
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
        #endregion

        #region Font
        public static SpriteFont MainFont { get; private set; }
        #endregion

        #region Textures
        public static Texture2D GameBottomBar { get; private set; }
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Parachute { get; private set; }
        public static List<Rectangle> ParachutesImgBox { get; private set; }
        public static Texture2D TanksSpriteSheet { get; private set; }
        public static XmlTextureAtlas TanksAtlas { get; private set; }
        #endregion

        #region Load
        public static void Load(ContentManager pContent)
        {
            //mscMenu = pContent.Load<Song>("cool");
            mscGameplay = pContent.Load<Song>("_Musics/Grandioso");
            /*mscGameover = pContent.Load<Song>("cool");
            mscVictory = pContent.Load<Song>("cool");*/
            sndShoot = pContent.Load<SoundEffect>("_Game/Shoot");
            sndExplosion = pContent.Load<SoundEffect>("_Game/Explosion");
            MainFont = pContent.Load<SpriteFont>("_Font/MainFont");
            GameBottomBar = pContent.Load<Texture2D>("_GUI/GameBottomBar");
            Cursor = pContent.Load<Texture2D>("_GUI/Cursor");
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

            TanksSpriteSheet = pContent.Load<Texture2D>("_Game/tanks_spritesheetRetina");
            XmlSerializer TankSpriteSheetSer = new XmlSerializer(typeof(XmlTextureAtlas));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes("Content/_Game/tanks_spritesheetRetina.xml"));
            TanksAtlas = (XmlTextureAtlas)TankSpriteSheetSer.Deserialize(stream);
        }
        #endregion
    }
}
