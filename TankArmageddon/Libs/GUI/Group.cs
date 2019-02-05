using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TankArmageddon.GUI
{
    public class Group
    {
        #region Propriétés

        private Vector2 _position = Vector2.Zero;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    foreach (Element element in Elements)
                    {
                        element.Position += value - _position;
                    }
                    _position = value;
                }
            }
        }


        /// <summary>
        /// Liste d'éléments de la GUI.
        /// </summary>
        public List<Element> Elements { get; private set; }
        
        public bool Visible
        {
            get { return Visible; }
            set
            {
                Visible = value;
                foreach (Element element in Elements)
                {
                    element.Visible = value;
                }
            }
        }
        #endregion

        #region Constructeur
        public Group()
        {
            Elements = new List<Element>();
        }
        #endregion

        #region Méthodes

        #region Gestion des éléments
        public void AddElement(Element pElement)
        {
            Elements.Add(pElement);
        }

        public void RemoveElement(Element pElement)
        {
            Elements.Remove(pElement);
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            foreach (Element element in Elements)
            {
                element.Update(gameTime);
            }
            Elements.RemoveAll(element => element.Remove);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Element element in Elements)
            {
                element.Draw(spriteBatch, gameTime);
            }
        }
        #endregion
        
        #endregion
    }
}
