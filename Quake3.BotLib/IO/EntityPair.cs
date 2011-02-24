using System;
using System.Globalization;

namespace Quake3.BotLib.IO
{
    public struct EntityPair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0} = {1}", this.Key, this.Value);
        }
    }
}