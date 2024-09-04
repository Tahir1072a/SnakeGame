using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public class GameBoard
    {
        #region Constants
        private const char DEFAULT_BORDER_CHAR = '#';

        private static Rectangle DEFAULT_BORDER_REC = new(20, 8, 70, 20);
        #endregion

        #region Properties
        public char BorderChar { get; set; } = DEFAULT_BORDER_CHAR;
        // Bize bir diktörtgen sağlar. (Sanal)
        public Rectangle BorderRec { get; private set; } = DEFAULT_BORDER_REC;
        #endregion

        #region Constroctures
        public GameBoard() { }  

        public GameBoard(Rectangle rectangle)
        {
            BorderRec = rectangle;
        }

        public GameBoard(int height, int widht)
        {

            BorderRec = new Rectangle(0, 0, height, widht);
        }

        public GameBoard(int x, int y, int height, int widht)
        {
            BorderRec = new Rectangle(x, y, height, widht);
        }
        #endregion

        #region Methods
        // O(n)
        public void DrawBoard()
        {
          
            for (int i = BorderRec.X; i < BorderRec.Right; i++)
            {
                Console.SetCursorPosition(i, BorderRec.Top);
                Console.Write(BorderChar);
                
                Console.SetCursorPosition(i, BorderRec.Bottom);
                Console.Write(BorderChar);
               
            }

            for (int i = BorderRec.Y; i <= BorderRec.Bottom; i++)
            {
                Console.SetCursorPosition(BorderRec.X, i);
                Console.Write(BorderChar);
                System.Threading.Thread.Sleep(10);
                Console.SetCursorPosition(BorderRec.Right, i);
                Console.Write(BorderChar);
            }
        }

       

        #endregion
    }
}
