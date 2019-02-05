namespace TankArmageddon
{
    public class GameState
    {
        #region Enumérations
        public enum SceneType
        {
            Menu,
            Gameplay,
            Gameover,
            Victory,
        }
        #endregion

        #region Propriétés
        public Scene CurrentScene { get; private set; }
        #endregion

        #region Constructeur
        public GameState() { }
        #endregion

        #region Méthodes

        #region Gestion des scènes
        public void ChangeScene(SceneType pSceneType)
        {
            if (CurrentScene != null)
            {
                CurrentScene.UnLoad();
                CurrentScene = null;
            }
            switch (pSceneType)
            {
                case SceneType.Menu:
                    CurrentScene = new Menu();
                    break;
                case SceneType.Gameplay:
                    CurrentScene = new Gameplay();
                    break;
                case SceneType.Gameover:
                    CurrentScene = new Gameover();
                    break;
                case SceneType.Victory:
                    CurrentScene = new Victory();
                    break;
                default:
                    break;
            }
            MainGame.Camera.Enable = false;
            CurrentScene.Load();
        }
        #endregion

        #endregion
    }
}
