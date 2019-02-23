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
        protected List<IActor> lstBuffer;
        #endregion

        #region Constructeur
        public Scene()
        {
            lstBuffer = new List<IActor>();
            lstActors = new List<IActor>();
        }
        #endregion

        #region Load/Unload
        public virtual void Load(){}
        public virtual void UnLoad(){}
        #endregion

        #region Acteurs
        public void AddActor(IActor actor)
        {
            lstBuffer.Add(actor);
        }
        #endregion

        #region Update 
        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor actor = lstActors[i];
                actor.Update(gameTime);
            }
            lstActors.AddRange(lstBuffer);
            lstBuffer.Clear();
            lstActors.RemoveAll(actor => actor.Remove);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor actor = lstActors[i];
                actor.Draw(spriteBatch, gameTime);
            }
        }
        #endregion
    }
}
