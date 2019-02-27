using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace TankArmageddon
{
    static class MusicManager
    {
        #region Variables privées
        private static int _currentMusic;
        private static int _nextMusic;
        private static List<Song> _musics;
        #endregion

        #region Constructeur
        static MusicManager()
        {
            _musics = new List<Song>();
            _currentMusic = -1;
            MediaPlayer.Volume = 0;
        }
        #endregion

        #region Méthodes

        #region Gestion musiques
        public static int AddMusic(Song pSong)
        {
            _musics.Add(pSong);
            return _musics.Count - 1;
        }

        public static void PlayMusic(int nMusic)
        {
            if (_nextMusic != nMusic)
            {
                _nextMusic = nMusic;
            }
        }
        #endregion

        #region Update
        public static void Update(GameTime gameTime)
        {
            // Au lancement de la musique, augmente progressivement le volume
            if (_currentMusic == _nextMusic && MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume += 0.01f;
            }

            // Au lancement d'une autre musique, diminue progressivement le volume
            if (_currentMusic != _nextMusic)
            {
                MediaPlayer.Volume -= 0.01f;
                if (MediaPlayer.Volume <= 0)
                {
                    MediaPlayer.Volume = 0;
                    Song mySong = _musics[_nextMusic];

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(mySong);
                    _currentMusic = _nextMusic;
                }
            }
        }
        #endregion

        #endregion
    }
}