using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TankArmageddon
{
    [Serializable()]
    [XmlRoot("TextureAtlas")]
    public class XmlTextureAtlas
    {
        [XmlElement("SubTexture")]
        public List<XmlSubTexture> Textures { get; set; }

        [Serializable()]
        public class XmlSubTexture
        {
            [XmlAttribute("frameHeight")]
            public int FrameHeight { get; set; }

            [XmlAttribute("frameWidth")]
            public int FrameWidth { get; set; }

            [XmlAttribute("framex")]
            public int FrameX { get; set; }

            [XmlAttribute("framey")]
            public int FrameY { get; set; }

            [XmlAttribute("height")]
            public int Height { get; set; }

            [XmlAttribute("width")]
            public int Width { get; set; }

            [XmlAttribute("x")]
            public int X { get; set; }

            [XmlAttribute("y")]
            public int Y { get; set; }

            [XmlAttribute("name")]
            public string Name { get; set; }

            public Rectangle ImgBox { get { return new Rectangle(X,Y, Width, Height); } }
        }
    }
}
