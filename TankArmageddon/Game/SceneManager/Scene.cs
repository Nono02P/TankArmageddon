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
            #region Remet dans l'ordre la liste suivant l'index de Layer (position X ajoutée pour éviter les clignotements)
            lstActors.Sort(delegate (IActor a1, IActor a2)
            {
                float z1 = a1.Layer * 10000 - a1.Position.X;
                float z2 = a2.Layer * 10000 - a2.Position.X;
                return z1.CompareTo(z2);
            });
            #endregion

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

        public virtual void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime)
        {
            for (int i = 0; i < lstActors.Count; i++)
            {
                IActor actor = lstActors[i];
                actor.Draw(primitiveBatch, gameTime);
            }
        }
        #endregion
    }
}