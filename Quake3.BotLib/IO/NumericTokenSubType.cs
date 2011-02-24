using System;

namespace Quake3.BotLib.IO
{
    [Flags]
    public enum NumericTokenSubType
    {
        Undefined = 0x0000,
        Decimal = 0x0008,
        Hex = 0x0100,
        Octal = 0x0200,
        Binary = 0x0400,
        Float = 0x0800,
        Integer = 0x1000,
        Long = 0x2000,
        Unsigned = 0x4000
    }
}