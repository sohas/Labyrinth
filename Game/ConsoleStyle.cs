using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class ConsoleStyle
    {
        const ConsoleColor dark = ConsoleColor.DarkRed;
        const ConsoleColor middle = ConsoleColor.DarkGray;
        const ConsoleColor light = ConsoleColor.White;
        public static void Print(char ch) 
        {
            Console.BackgroundColor = dark;
            switch (ch) 
            {
                case 'i':
                case 'o':
                case ' ':
                    Console.BackgroundColor = middle;
                    break;
            }
            switch (ch)
            {
                case '+':
                    Console.ForegroundColor = middle;
                    break;
                case '~':
                case ':':
                case 'X':
                    Console.ForegroundColor = dark;
                    break;
                default:
                    Console.ForegroundColor = light;
                    break;
            }
            Console.Write(ch);
        }


    }
}
