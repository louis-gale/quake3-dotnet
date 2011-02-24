using System;

namespace Quake3.BotLib.IO
{
    [Flags]
    public enum ScriptOptions
    {
        NoErrors = 0x0001,
        NoWarnings = 0x0002,
        NoStringWhiteSpaces = 0x0004,
        NoStringEscapeCharacters = 0x0008,
        Primitive = 0x0010,
        NoBinaryNumbers = 0x0020,
        NoNumberValues = 0x0040
    }
}