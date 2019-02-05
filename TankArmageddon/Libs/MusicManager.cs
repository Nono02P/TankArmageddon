using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace TankArmageddon
{
    static class MusicManager
    {
        #region Variables privées
        private static int _nCurrentMusic;
        private static int _nNextMusic;
        private static List<Song> _lstMusics;
        #endregion

        #region Constructeur
        static MusicManager()
        {
            _lstMusics = new List<Song>();
            _nCurrentMusic = -1;
            MediaPlayer.Volume = 0;
        }
        #endregion

        #region Méthodes

        #region Gestion musiques
        public static int AddMusic(Song pSong)
        {
            _lstMusics.Add(pSong);
            return _lstMusics.Count - 1;
        }

        public static void PlayMusic(int nMusic)
        {
            if (_nNextMusic != nMusic)
            {
                _nNextMusic = nMusic;
                Debug.WriteLine("Change music from {0} to {1}", _nCurrentMusic, _nNextMusic);
            }
        }
        #endregion

        #region Update
        public static void Update(GameTime gameTime)
        {
            // Au lancement de la musique, augmente progressivement le volume
            if (_nCurrentMusic == _nNextMusic && MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume += 0.01f;
            }

            // Au lancement d'une autre musique, diminue progressivement le volume
            if (_nCurrentMusic != _nNextMusic)
            {
                MediaPlayer.Volume -= 0.01f;
                if (MediaPlayer.Volume <= 0)
                {
                    MediaPlayer.Volume = 0;
                    Song mySong = _lstMusics[_nNextMusic];

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(mySong);
                    _nCurrentMusic = _nNextMusic;
                    Debug.WriteLine("Start playing music " + _nNextMusic);
                }
            }
        }
        #endregion

        #endregion
    }
}
