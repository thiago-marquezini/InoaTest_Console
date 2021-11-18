using System;
using System.Text.RegularExpressions;

namespace InoaTest_Console
{
    public interface IB3AtivoView
    {
        void WriteColor(string message, ConsoleColor color, bool linebreak);
        void Print(APIObject Object, string Symbol);
    }

    public class B3AtivoView : IB3AtivoView
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

        public void Print(APIObject Object, string Symbol)
        {
            WriteColor(String.Format("[{0}] ", Object.results[Symbol].symbol), ConsoleColor.Yellow, false);
            WriteColor(String.Format("[{0}{1}] ", Object.results[Symbol].currency, Object.results[Symbol].price), ConsoleColor.Red, false);
            WriteColor(String.Format("([{0}%]) ", Object.results[Symbol].change_percent), (Object.results[Symbol].change_percent > 0) ? ConsoleColor.Blue : ConsoleColor.Red, false);
            WriteColor(String.Format("[{0}] ", Object.results[Symbol].updated_at), ConsoleColor.Green, false);
            WriteColor(String.Format("> [{0}] ", Object.results[Symbol].Action), ConsoleColor.Cyan, false);
            WriteColor(String.Format("([{0}ms])", Object.execution_time), ConsoleColor.Cyan, true);
        }
    }
}