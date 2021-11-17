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
            String _Symbol   = String.Format("[{0}] ", Object.symbol);
            String _Price    = String.Format("[{0}{1}] ", Object.currency, Object.price);
            String _Change   = String.Format("([{0}%]) ", Object.change_percent);
            String _LastUpd  = String.Format("[{0}] ", Object.updated_at);
            String _Action   = String.Format("> [{0}]", Object.Action);

            WriteColor(_Symbol, ConsoleColor.Yellow, false);
            WriteColor(_Price, ConsoleColor.Red, false);
            WriteColor(_Change, (Object.change_percent > 0) ? ConsoleColor.Blue : ConsoleColor.Red, false);
            WriteColor(_LastUpd, ConsoleColor.Green, false);
            WriteColor(_Action, ConsoleColor.Cyan, true);
        }
    }
}