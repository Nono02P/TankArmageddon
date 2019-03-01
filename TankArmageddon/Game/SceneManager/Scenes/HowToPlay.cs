namespace TankArmageddon
{
    public class HowToPlay : Scene
    {
        public override void Load()
        {
            MainGame.ChangeScene(SceneType.Menu);
            base.Load();
        }
    }
}
