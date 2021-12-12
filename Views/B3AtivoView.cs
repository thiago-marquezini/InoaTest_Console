using System;
using System.Text.RegularExpressions;
using InoaTest_Console.Models;

namespace InoaTest_Console.Views
{
    interface IB3AtivoView
    {
        void WriteColor(string message, ConsoleColor color, bool linebreak);
        void Print(APIObjectItem Object);
    }

    class B3AtivoView : IB3AtivoView
    {
        /* Fonte WriteColor(): https://stackoverflow.com/a/60492990 */
        public void WriteColor(string message, ConsoleColor color, bool linebreak)
        {
            var pieces = Regex.Split(message, @"(\[[^\]]*\])");
            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];
                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }
                Console.Write(piece);
                Console.ResetColor();
            }
            if (linebreak)
                Console.WriteLine();
        }

        public void Print(APIObjectItem Object)
        {
            WriteColor(string.Format("[{0}] ", Object.symbol), ConsoleColor.Yellow, false);
            WriteColor(string.Format("[{0}{1}] ", Object.currency, Object.price), ConsoleColor.Red, false);
            WriteColor(string.Format("([{0}%]) ", Object.change_percent), (Object.change_percent > 0) ? ConsoleColor.Blue : ConsoleColor.Red, false);
            WriteColor(string.Format("[{0}] ", Object.updated_at), ConsoleColor.Green, false);
            WriteColor(string.Format("> [{0}]", Object.Action), ConsoleColor.Cyan, true);
        }
    }
}