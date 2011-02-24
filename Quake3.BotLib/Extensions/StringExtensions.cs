namespace Quake3.BotLib.Extensions
{
    public static class StringExtensions
    {
        public const char DoubleQuote = '\"';

        public static string StripDoubleQuotes(this string value)
        {
            int start = 0;
            int end = value.Length - 1;

	        if (value[0] == StringExtensions.DoubleQuote)
	        {
		        start++;
	        }

	        if (value[end] == '\"')
	        {
		        end--;
	        }

            return value.Substring(start, end - start + 1);
        }

        public static bool HasCharacters(this string value, char[] characters)
        {
            var internalCharacters = value.ToCharArray();

            if (internalCharacters.Length != characters.Length)
            {
                return false;
            }

            for (int i = 0; i < internalCharacters.Length; i++)
            {
                if (internalCharacters[i] != characters[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}