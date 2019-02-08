﻿using Microsoft.Xna.Framework;
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
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Element element = Elements[i];
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
                for (int i = 0; i < Elements.Count; i++)
                {
                    Element element = Elements[i];
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
            for (int i = 0; i < Elements.Count; i++)
            {
                Element element = Elements[i];
                element.Update(gameTime);
            }
            Elements.RemoveAll(element => element.Remove);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Element element = Elements[i];
                element.Draw(spriteBatch, gameTime);
            }
        }
        #endregion
        
        #endregion
    }
}
