using System;

namespace Quake3.BotLib.Extensions
{
    internal static class ConsoleExtensions
    {
        internal static void WriteLine( string text, ConsoleColor background, ConsoleColor foreground)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}