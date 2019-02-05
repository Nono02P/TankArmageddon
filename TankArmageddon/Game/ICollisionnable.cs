namespace TankArmageddon
{
    public interface ICollisionnable
    {
        void TouchedBy(ICollisionnable collisionnable);
    }
}
