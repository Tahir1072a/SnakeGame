using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public static class ConsoleHelper
    {
        public static void ClearText(int x, int y)
        {
            ResetCursorPosition(x, y);
            Console.Write(' ');
        }

        public static void ClearLine(int y)
        {
            ResetCursorPosition(y: y);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        public static void ResetCursorPosition(int x = 0, int y = 0)
        {
            Console.SetCursorPosition(x, y);
        }

        public static async Task PrintBlinkingText(string message, Point point, int delay = 500, CancellationToken token = default)
        {
            // Buraya bitiş ekranına yanıp sönen oyunu kazandınız gibi bir şey yazdırılacak.
            while (true) 
            { 
                ResetCursorPosition(point.X, point.Y);
                Console.WriteLine(message);
                await Task.Delay(delay);

                ResetCursorPosition(point.X, point.Y);
                Console.WriteLine(new string(' ', message.Length));
                
                await Task.Delay(delay);
            }
        }
    }
}
