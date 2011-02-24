namespace Quake3.BotLib.IO
{
    public class Token
    {
        public Token(TokenType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the underlying string value of the token
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of this token
        /// </summary>
	    public TokenType Type { get; private set; }

        /// <summary>
        /// Gets or sets the line in the file the token was found on
        /// </summary>
        public int Line { get; set; }
	    
        /// <summary>
        /// Gets or sets the nubmer of lines of whitespace preceding this token
        /// </summary>
        public int LinesCrossed { get; set; }
    }
}