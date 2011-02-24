using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Quake3.BotLib.Extensions;

namespace Quake3.BotLib.IO
{
    // TODO: Use a buffer to make navigating around the 'stream' much more efficient than manipulating the stream position
    public class ScriptTokenizer : IDisposable
    {
        private const int MaxTokenLength = 1024;
        private const char DoubleQuote = '\"';
        private const char Null = '\0';
        private const char Newline = '\n';
        private const char Escape = '\\';
        private const char SingleQuote = '\'';

        private readonly BinaryReader reader;
        private readonly ScriptOptions options;
        private readonly IDictionary<int, Punctuation[]> punctuations;
        private readonly ILog log;
        private int line;
        private int lastLine;

        public ScriptTokenizer(string fileName, ScriptOptions options, Stream stream, ILog log, IEnumerable<Punctuation> punctuations = null)
        {
            this.FileName = fileName;
            this.options = options;
            this.reader = new BinaryReader(stream);
            this.log = log;
            this.line = 1;
            this.lastLine = 1;

            // Use the default set of punctuations if none are provided
            punctuations = punctuations ?? Punctuation.DefaultPunctuations;

            // Group the punctuations by their first character, for efficient matching and then sort the punctuations by their length to ensure longer puncutations get matched first
            this.punctuations = punctuations.GroupBy(p => p.Value[0]).ToDictionary(g => (int)g.Key, g => g.OrderByDescending(p => p.Value.Length).ToArray());
        }

        public ScriptOptions Options
        {
            get
            {
                return this.options;
            }
        }

        public string FileName
        {
            get;
            private set;
        }

        public bool ExpectNumericToken(NumericTokenSubType subType, out NumericToken token)
        {
            token = null;
            Token t;

            if (!this.ReadToken(out t))
            {
                this.ScriptError("couldn't read expected token");

                return false;
            }

            if (t.Type != TokenType.Number)
            {
                this.ScriptError("expected a number, found {0}", t.Value);

                return false;
            }

            token = t as NumericToken;

            if (!token.SubType.Has(subType))
            {
                string str = String.Empty;

                if (subType.Has(NumericTokenSubType.Decimal)) str = "decimal";
                if (subType.Has(NumericTokenSubType.Hex)) str = "hex";
                if (subType.Has(NumericTokenSubType.Octal)) str = "octal";
                if (subType.Has(NumericTokenSubType.Binary)) str = "binary";

                if (subType.Has(NumericTokenSubType.Long)) str += " long";
                if (subType.Has(NumericTokenSubType.Unsigned)) str += " unsigned";
                if (subType.Has(NumericTokenSubType.Float)) str += " float";
                if (subType.Has(NumericTokenSubType.Integer)) str += " integer";

                this.ScriptError("expected {0}, found {1}", str, token.Value);

                return false;
            }

            return true;
        }

        public bool ExpectPunctuationToken(PunctuationSubType subType, out PunctuationToken token)
        {
            token = null;
            Token t;

            if (!this.ReadToken(out t))
            {
                this.ScriptError("couldn't read expected token");

                return false;
            }

            if (t.Type != TokenType.Punctuation)
            {
                this.ScriptError("expected a punctuation, found {0}", t.Value);

                return false;
            }

            token = t as PunctuationToken;

            if (subType < 0)
            {
                this.ScriptError("BUG: wrong punctuation subtype");

                return false;
            }

            if (token.SubType != subType)
            {
                // Search the punctuation table for the string representation of the punctuation
                var punctuation = this.punctuations.Values.SelectMany(a => a.Where(p => p.SubType == subType)).First();
                this.ScriptError("expected {0}, found {1}", punctuation.Value, token.Value);

                return false;
            }

            return true;
        }

        public bool ExpectTokenType(TokenType type, out Token token)
        {
            if (!this.ReadToken(out token))
            {
                this.ScriptError("couldn't read expected token");

                return false;
            }

            if (token.Type != type)
            {
                string str = String.Empty;

                if (type == TokenType.String) str = "string";
                if (type == TokenType.Literal) str = "literal";
                if (type == TokenType.Name) str = "name";
                if (type == TokenType.Punctuation) str = "punctuation";

                this.ScriptError("expected a {0}, found {1}", str, token.Value);

                return false;
            }

            return true;
        }

        public bool ReadToken(out Token token)
        {
            token = null;

            this.lastLine = this.line;

            // read unuseful stuff
            if (!this.ReadWhiteSpace())
            {
                return false;
            }

            // line the token is on
            int tokenLine = this.line;

            // number of lines crossed before token
            int tokenLinesCrossed = this.line - this.lastLine;

            int c = this.reader.PeekChar();

            // if there is a leading double quote
            if (c == ScriptTokenizer.DoubleQuote)
            {
                if (!this.ReadString(out token, StringExtensions.DoubleQuote))
                {
                    return false;
                }
            }
            else if (c == ScriptTokenizer.SingleQuote)
            {
                // if an literal
                if (!this.ReadString(out token, ScriptTokenizer.SingleQuote))
                {
                    return false;
                }
            }
            else if (IsDigit(c) || (c == '.' && IsDigit(this.reader.PeekChar(1))))
            {
                NumericToken t;

                // if there is a number
                if (!this.ReadNumber(out t))
                {
                    token = t;

                    return false;
                }

                token = t;
            }
            else if (this.options.Has(ScriptOptions.Primitive))
            {
                // if this is a primitive script
                return this.ReadPrimitive(out token);
            }
            else if (IsLetter(c) || c == '_')
            {
                // if there is a name
                if (!this.ReadName(out token))
                {
                    return false;
                }
            }
            else
            {
                PunctuationToken t;

                if (!this.ReadPunctuation(out t))
                {
                    // check for punctuations
                    this.ScriptError("can't read token");
                    token = t;

                    return false;
                }

                token = t;
            }

            if (token != null)
            {
                token.Line = tokenLine;
                token.LinesCrossed = tokenLinesCrossed;
            }

            return true;
        }

        public void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
            }
        }

        private bool ReadPunctuation(out PunctuationToken token)
        {
            token = new PunctuationToken();

            int firstChar = this.reader.PeekChar();

            if (!this.punctuations.ContainsKey(firstChar))
            {
                return false;
            }

            foreach (var punctuation in this.punctuations[firstChar])
            {
                using (var transaction = new StreamTransaction(this.reader.BaseStream))
                {
                    if (punctuation.Value.HasCharacters(this.reader.ReadChars(punctuation.Value.Length)))
                    {
                        token.Value = punctuation.Value;
                        token.SubType = punctuation.SubType;

                        transaction.Commit();

                        return true;
                    }
                }
            }

            return false;
        }

        private bool ReadName(out Token token)
        {
            token = new Token(TokenType.Name);

            var builder = new StringBuilder();
            int c;

            do
            {
                builder.Append(this.reader.ReadChar());

                if (builder.Length >= ScriptTokenizer.MaxTokenLength)
                {
                    this.ScriptError("name longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                    return false;
                }

                c = this.reader.PeekChar();
            }
            while (IsLetter(c) || IsDigit(c) || c == '_');

            // The sub type is the length of the name
            token.Value = builder.ToString();

            return true;
        }

        private bool ReadPrimitive(out Token token)
        {
            token = new Token(TokenType.Primitive);

            var builder = new StringBuilder();

            while (this.reader.PeekChar() > ' ' && this.reader.PeekChar() != ';')
            {
                if (builder.Length >= ScriptTokenizer.MaxTokenLength)
                {
                    this.ScriptError("primitive token longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                    return false;
                }

                builder.Append(this.reader.ReadChar());
            }

            token.Value = builder.ToString();

            return true;
        }

        private bool ReadNumber(out NumericToken token)
        {
            token = new NumericToken();

            var builder = new StringBuilder();
            int c = this.reader.PeekChar();
            NumericTokenSubType subType = NumericTokenSubType.Undefined;

            if (c == '0' && this.reader.PeekChar(1) == 'x' || this.reader.PeekChar(1) == 'X')
            {
                // hexadecimal
                builder.Append(this.reader.ReadChars(2));
                c = this.reader.PeekChar();

                while (IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'A'))
                {
                    builder.Append(this.reader.ReadChar());

                    if (builder.Length >= ScriptTokenizer.MaxTokenLength)
                    {
                        this.ScriptError("hexadecimal number longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                        return false;
                    }

                    c = this.reader.PeekChar();
                }

                subType = subType.Add(NumericTokenSubType.Hex);
            }
            else if (c == '0' && this.reader.PeekChar(1) == 'b' || this.reader.PeekChar(1) == 'B')
            {
                // binary number
                builder.Append(this.reader.ReadChars(2));
                c = this.reader.PeekChar();

                while (c == '0' || c == '1')
                {
                    builder.Append(this.reader.ReadChar());

                    if (builder.Length >= ScriptTokenizer.MaxTokenLength)
                    {
                        this.ScriptError("binary number longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                        return false;
                    }

                    c = this.reader.PeekChar();
                }

                subType = subType.Add(NumericTokenSubType.Binary);
            }
            else
            {
                // decimal or octal integer or floating point number
                bool octal = false;
                bool dot = false;

                if (this.reader.PeekChar() == '0')
                {
                    octal = true;
                }

                while (true)
                {
                    c = this.reader.PeekChar();

                    if (c == '.')
                    {
                        dot = true;
                    }
                    else if (c == '8' || c == '9')
                    {
                        octal = false;
                    }
                    else if (c < '0' || c > '9')
                    {
                        break;
                    }

                    builder.Append(this.reader.ReadChar());

                    if (builder.Length >= ScriptTokenizer.MaxTokenLength - 1)
                    {
                        this.ScriptError("number longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                        return false;
                    }
                }

                subType = subType.Add(octal ? NumericTokenSubType.Octal : NumericTokenSubType.Decimal);
                subType = subType.Add(dot ? NumericTokenSubType.Float : NumericTokenSubType.Integer);
            }

            for (int i = 0; i < 2; i++)
            {
                c = this.reader.PeekChar();

                if ((c == 'l' || c == 'L') && !subType.Has(NumericTokenSubType.Long))
                {
                    // check for a LONG number
                    // bk001204 - brackets
                    this.reader.ReadChar();
                    subType = subType.Add(NumericTokenSubType.Long);
                }
                else if ((c == 'u' || c == 'U') && !subType.Has(NumericTokenSubType.Unsigned | NumericTokenSubType.Float))
                {
                    // check for an UNSIGNED number
                    // bk001204 - brackets 
                    this.reader.ReadChar();
                    subType = subType.Add(NumericTokenSubType.Unsigned);
                }
            }

            token.Value = builder.ToString();
            token.IntegerValue = UInt64.Parse(token.Value);
            token.FloatValue = Single.Parse(token.Value);
            token.SubType = subType;

            return true;
        }

        private bool ReadWhiteSpace()
        {
            while (true)
            {
                // Skip white space
                this.reader.ConsumeWhile(IsWhiteSpace);

                // Skip comments
                if (this.reader.PeekChar() == '/')
                {
                    this.reader.Read();

                    int nextChar = this.reader.PeekChar();

                    if (nextChar == '/')
                    {
                        // comments marked with //
                        this.reader.ConsumeWhile(c => c != ScriptTokenizer.Newline, true);
                        this.line++;

                        if (this.reader.EndOfStream())
                        {
                            return false;
                        }

                        continue;
                    }

                    if (nextChar == '*')
                    {
                        // comments marked with /* */
                        this.reader.Read();
                        this.line += this.reader.TakeUntil("*/").Count(c => c == ScriptTokenizer.Newline);

                        if (this.reader.EndOfStream())
                        {
                            return false;
                        }

                        continue;
                    }
                }

                break;
            }

            return true;
        }

        private bool ReadString(out Token token, char quote)
        {
            token = new Token(quote == ScriptTokenizer.DoubleQuote ? TokenType.String : TokenType.Literal);

            var builder = new StringBuilder();

            // Leading quote
            builder.Append(this.reader.ReadChar());

            while (true)
            {
                // minus 2 because trailing double quote and zero have to be appended
                if (builder.Length >= ScriptTokenizer.MaxTokenLength - 2)
                {
                    this.ScriptError("string longer than MAX_TOKEN = {0}", ScriptTokenizer.MaxTokenLength);

                    return false;
                }

                // Check that EOS has not been reached
                if (this.reader.EndOfStream())
                {
                    this.ScriptError("unexpected end of stream encountered while reading string");

                    return false;
                }

                int c = this.reader.PeekChar();

                // if there is an escape character and if escape characters inside a string are allowed
                if (c == ScriptTokenizer.Escape && !this.options.Has(ScriptOptions.NoStringEscapeCharacters))
                {
                    builder.Append(this.ReadEscapeCharacter());
                }
                else if (c == quote)
                {
                    // if a trailing quote (string split over multiple lines)
                    // step over the double quote
                    this.reader.ReadChar();

                    // if white spaces in a string are not allowed
                    if (this.options.Has(ScriptOptions.NoStringWhiteSpaces))
                    {
                        break;
                    }

                    long position = this.reader.BaseStream.Position;
                    int line = this.line;

                    // read unuseful stuff between possible two following strings
                    if (!this.ReadWhiteSpace())
                    {
                        this.reader.BaseStream.Position = position;
                        this.line = line;
                        break;
                    }

                    // if there's no leading double qoute
                    if (this.reader.PeekChar() != quote)
                    {
                        this.reader.BaseStream.Position = position;
                        this.line = line;
                        break;
                    }

                    // step over the new leading double quote
                    this.reader.ReadChar();
                }
                else if (c == ScriptTokenizer.Null)
                {
                    this.ScriptError("missing trailing quote");

                    return false;
                }
                else if (c == ScriptTokenizer.Newline)
                {
                    this.ScriptError("newline inside string {0}", token.Value);

                    return false;
                }
                else
                {
                    builder.Append(this.reader.ReadChar());
                }
            }

            // trailing quote
            builder.Append(quote);

            // the sub type is the length of the string
            token.Value = builder.ToString();

            return true;
        }

        private char ReadEscapeCharacter()
        {
            int c, val, i;

            // Step over the leading '\\'
            this.reader.ReadChar();

            // Determine the escape character
            switch (this.reader.PeekChar())
            {
                case '\\': c = '\\'; break;
                case 'n': c = '\n'; break;
                case 'r': c = '\r'; break;
                case 't': c = '\t'; break;
                case 'v': c = '\v'; break;
                case 'b': c = '\b'; break;
                case 'f': c = '\f'; break;
                case 'a': c = '\a'; break;
                case '\'': c = '\''; break;
                case '\"': c = '\"'; break;
                case '?': c = '?'; break;
                case 'x':
                {
                    // Hex-valued character
                    this.reader.ReadChar();

                    for (i = 0, val = 0; ; i++, this.reader.ReadChar())
                    {
                        c = this.reader.PeekChar();

                        if (IsDigit(c))
                        {
                            c = c - '0';
                        }
                        else if (IsUpperLetter(c))
                        {
                            c = c - 'A' + 10;
                        }
                        else if (IsLowerLetter(c))
                        {
                            c = c - 'a' + 10;
                        }
                        else
                        {
                            break;
                        }

                        val = (val << 4) + c;
                    }

                    this.reader.BaseStream.Position--;

                    if (val > 0xFF)
                    {
                        this.ScriptWarning("too large value in escape character");
                        val = 0xFF;
                    }

                    c = val;
                    break;
                }
                default:
                {
                    // NOTE: decimal ASCII code, NOT octal
                    c = this.reader.PeekChar();

                    if (c < '0' || c > '9')
                    {
                        this.ScriptError("unknown escape char");
                    }

                    for (i = 0, val = 0; ; i++, this.reader.ReadChar())
                    {
                        c = this.reader.PeekChar();

                        if (IsDigit(c))
                        {
                            c = c - '0';
                        }
                        else
                        {
                            break;
                        }

                        val = val * 10 + c;
                    }

                    this.reader.BaseStream.Position--;

                    if (val > 0xFF)
                    {
                        this.ScriptWarning("too large value in escape character");
                        val = 0xFF;
                    }

                    c = val;
                    break;
                }
            }

            // Step over the escape character or the last digit of the number
            this.reader.ReadChar();

            // Store the escape character
            return (char)c;
        }

        // TODO: Possibly use unicode Char type methods instead of these
        private static bool IsDigit(int c)
        {
            return (c >= '0' && c <= '9');
        }

        private static bool IsLetter(int c)
        {
            return IsLowerLetter(c) || IsUpperLetter(c);
        }

        private static bool IsLowerLetter(int c)
        {
            return (c >= 'a' && c <= 'z');
        }

        private static bool IsUpperLetter(int c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        private static bool IsWhiteSpace(int c)
        {
            return c != -1 && (c <= ' ');
        }

        private void ScriptError(string str, params object[] args)
        {
            if (this.options == ScriptOptions.NoErrors)
            {
                return;
            }

            String message = String.Format(CultureInfo.CurrentCulture, str, args);
            message = String.Format(CultureInfo.CurrentCulture, "File {0}, line {1}: {2}\n", this.FileName, this.line, message);

            this.log.Error(message);
        }

        private void ScriptWarning(string str, params object[] args)
        {
            if (this.options == ScriptOptions.NoWarnings)
            {
                return;
            }

            String message = String.Format(CultureInfo.CurrentCulture, str, args);
            message = String.Format(CultureInfo.CurrentCulture, "File {0}, line {1}: {2}\n", this.FileName, this.line, message);

            this.log.Warning(message);
        }
    }
}