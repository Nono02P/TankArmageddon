namespace TankArmageddon
{
    public partial class Tank
    {
        public class Drill : Sprite, ISentByTank
        {
            public Tank Sender { get; private set; }

            public Drill(Tank pParent)
            {
                Sender = pParent;
            }
        }
    }
}
