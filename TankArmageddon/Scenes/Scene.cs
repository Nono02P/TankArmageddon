using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace TankArmageddon
{
    public abstract class Scene
    {
        #region Variables Protected
        protected Song sndMusic;
        protected List<IActor> lstActors;
        #endregion

        #region Constructeur
        public Scene()
        {
            lstActors = new List<IActor>();
        }
        #endregion

        #region Méthodes

        #region Load/Unload
        public virtual void Load(){}
        public virtual void UnLoad(){}
        #endregion

        #region Acteurs
        public void AddActor(IActor actor)
        {
            lstActors.Add(actor);
        }
        #endregion

        #region Update 
        public virtual void Update(GameTime gameTime)
        {
            foreach (IActor actor in lstActors)
            {
                actor.Update(gameTime);
            }
            lstActors.RemoveAll(actor => actor.Remove);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (IActor actor in lstActors)
            {
                actor.Draw(spriteBatch, gameTime);
            }
        }
        #endregion

        #endregion
    }
}
