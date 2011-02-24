using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Quake3.BotLib.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static void ConsumeWhile(this BinaryReader reader, Func<int, bool> condition, bool include = false)
        {
            while (condition(reader.PeekChar()))
            {
                reader.ReadChar();
            }

            if (include)
            {
                reader.ReadChar();
            }
        }

        public static IEnumerable<char> TakeWhile(this BinaryReader reader, Func<int, bool> condition)
        {
            while (condition(reader.PeekChar()))
            {
                yield return reader.ReadChar();
            }
        }

        public static IEnumerable<char> TakeUntil(this BinaryReader reader, Func<int, bool> condition, bool include = true)
        {
            foreach (char c in reader.TakeWhile(c => !condition(c)))
            {
                yield return c;
            }

            if (include)
            {
                yield return reader.ReadChar();
            }
        }

        public static IEnumerable<char> TakeUntil(this BinaryReader reader, string sequence, bool include = true)
        {
            int i = 0;

            while (i < sequence.Length)
            {
                int c = reader.PeekChar();

                // EOS
                if (c == -1)
                {
                    break;
                }

                yield return reader.ReadChar();

                i = (c != sequence[i]) ? 0 : ++i;
            }
        }

        public static bool EndOfStream(this BinaryReader reader)
        {
            return reader.PeekChar() == -1;
        }

        public static bool IsEndOfStream(int c)
        {
            return c == -1;
        }

        public static int PeekChar(this BinaryReader reader, long offset)
        {
            reader.BaseStream.Position += offset;
            int result = reader.PeekChar();
            reader.BaseStream.Position -= offset;

            return result;
        }

        // NOTE: Does NOT restore stream position
        public static bool StringCompare(this BinaryReader reader, string comparison)
        {
            bool result = true;

            foreach (char character in comparison)
            {
                // Make sure EOS has not been reached
                if (reader.PeekChar() == -1)
                {
                    result = false;
                    break;
                }

                // Comparison fails as soon as any character does not match
                if (reader.ReadChar() != character)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}