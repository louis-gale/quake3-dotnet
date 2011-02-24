namespace Quake3.BotLib.IO
{
    public class Entity
    {
        public Entity()
        {
            this.Pairs = new EntityPairCollection();
        }

        public EntityPairCollection Pairs
        {
            get;
            private set;
        }
    }
}