using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Quake3.BotLib;
using Quake3.BotLib.Extensions;

namespace Quake3.BotLib.IO
{
    public class BspParser
    {
        private const int MaxBspEntities = 2048;

        private readonly List<Entity> entities;
        private readonly ILog log;
        private readonly ScriptOptions options;
        private readonly string fileName;

        public BspParser(string fileName, Stream stream, ILog log)
        {
            this.entities = new List<Entity>();
            this.log = log;
            this.options = ScriptOptions.NoStringWhiteSpaces | ScriptOptions.NoStringEscapeCharacters;
            this.fileName = fileName;

            this.ParseBspEntities(stream);
        }

        public IEnumerable<Entity> Entities
        {
            get
            {
                return this.entities;
            }
        }

        private void ParseBspEntities(Stream stream)
        {
            using (var tokenizer = new ScriptTokenizer(this.fileName, this.options, stream, this.log))
            {
                Token token = null;

                while (tokenizer.ReadToken(out token))
                {
                    if (token.Value != "{")
                    {
                        this.ScriptError(token.Line, "invalid {0}", token.Value);

                        return;
                    }

                    if (this.entities.Count >= BspParser.MaxBspEntities)
                    {
                        Console.WriteLine("Too many entities in BSP file");
                        break;
                    }

                    var entity = new Entity();

                    while (tokenizer.ReadToken(out token))
                    {
                        if (token.Value == "}")
                        {
                            break;
                        }

                        var pair = new EntityPair();

                        if (token.Type != TokenType.String)
                        {
                            this.ScriptError(token.Line, "invalid {0}", token.Value);

                            return;
                        }

                        pair.Key = token.Value.StripDoubleQuotes();

                        if (!tokenizer.ExpectTokenType(TokenType.String, out token))
                        {
                            return;
                        }

                        pair.Value = token.Value.StripDoubleQuotes();

                        entity.Pairs.Add(pair);
                    }

                    if (token.Value != "}")
                    {
                        this.ScriptError(token.Line, "missing }");

                        return;
                    }

                    this.entities.Add(entity);
                }
            }
        }

        private void ScriptError(int line, string str, params object[] args)
        {
            if (this.options == ScriptOptions.NoErrors)
            {
                return;
            }

            String message = String.Format(CultureInfo.CurrentCulture, str, args);
            message = String.Format(CultureInfo.CurrentCulture, "File {0}, line {1}: {2}", this.fileName, line, message);

            this.log.Error(message);
        }

        private void ScriptWarning(int line, string str, params object[] args)
        {
            if (this.options == ScriptOptions.NoWarnings)
            {
                return;
            }

            String message = String.Format(CultureInfo.CurrentCulture, str, args);
            message = String.Format(CultureInfo.CurrentCulture, "File {0}, line {1}: {2}", this.fileName, line, message);

            this.log.Warning(message);
        }
    }
}