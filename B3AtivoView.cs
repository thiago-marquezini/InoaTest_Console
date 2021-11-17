using System;
using System.Text.RegularExpressions;

namespace InoaTest_Console
{
    class B3AtivoView
    {
        /* Fonte WriteColor(): https://stackoverflow.com/a/60492990 */
        static void WriteColor(string message, ConsoleColor color, bool linebreak)
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

        public void Print(APIObject.APIObjectItem Object)
        {
            WriteColor(String.Format("[{0}] ", Object.symbol), ConsoleColor.Yellow, false);
            WriteColor(String.Format("[{0}{1}] ", Object.currency, Object.price), ConsoleColor.Red, false);
            WriteColor(String.Format("([{0}%]) ", Object.change_percent), (Object.change_percent > 0) ? ConsoleColor.Blue : ConsoleColor.Red, false);
            WriteColor(String.Format("[{0}] ", Object.updated_at), ConsoleColor.Green, false);
            WriteColor(String.Format("> [{0}]", Object.Action), ConsoleColor.Cyan, true);
        }
    }
}