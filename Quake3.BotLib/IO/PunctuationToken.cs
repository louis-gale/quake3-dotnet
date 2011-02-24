namespace Quake3.BotLib.IO
{
    public class PunctuationToken : Token
    {
        public PunctuationToken() : base(TokenType.Punctuation)
        {
        }

        public PunctuationSubType SubType { get; set; }
    }
}