namespace Quake3.BotLib.IO
{
    public class NumericToken : Token
    {
        public NumericToken() : base(TokenType.Number)
        {
        }

        public ulong IntegerValue { get; set; }

        public float FloatValue { get; set; }

        public NumericTokenSubType SubType { get; set; }
    }
}