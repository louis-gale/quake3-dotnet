using System.Collections.ObjectModel;

namespace Quake3.BotLib.IO
{
    public class EntityPairCollection : KeyedCollection<string, EntityPair>
    {
        protected override string GetKeyForItem(EntityPair item)
        {
            return item.Key;
        }
    }
}