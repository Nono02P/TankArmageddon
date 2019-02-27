namespace TankArmageddon
{
    public interface ICollisionnable : IActor
    {
        void TouchedBy(ICollisionnable collisionnable);
    }
}