using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public class SnakeEngine
    {
        #region Constanst
        private const int DEFAULT_DELAY = 75;
        private const double DEFAULT_VERTICAL_DELAY_RATIO = 0.6;
        private const double delay = DEFAULT_DELAY;
        private const double delayBoost = DEFAULT_DELAY / 2;

        private const ConsoleColor FOOD_COLOR = ConsoleColor.Green;
        private const ConsoleColor SNAKE_COLOR = ConsoleColor.Yellow;
        private const char DEFAULT_FOOD_CHAR = '*';
        private const int DEFAULT_SNAKE_LENGHT = 5;
        private const char DEFAULT_SNAKE_CHAR = 'O';

        private const Direction DEFAULT_DIRECTION = Direction.Right;

        #endregion

        private LinkedList<Point> _segments;
        private HashSet<Point> _emptyPoints;
        private Point food;
        private GameBoard _board;
        private bool speedboosted = false, isPaused = false;
        private int footEaten;

        public char FoodChar { get; set; }
        public Point SnakeStartingPoint { get; set; }
        public Direction CurrentDirection { get; set; }
        public int InitialSnakeLenght { get; set; }
        public char SnakeChar { get; set; }

        public SnakeEngine()
        {
            // Default ayarları yapılandır.
            SetDefaults();
        }

        private void SetDefaults()
        {
            FoodChar = DEFAULT_FOOD_CHAR;
            SnakeStartingPoint = new(1, 1);
            CurrentDirection = DEFAULT_DIRECTION;
            InitialSnakeLenght = DEFAULT_SNAKE_LENGHT;
            SnakeChar = DEFAULT_SNAKE_CHAR;

            _board = new(20, 8, 70, 20);
        }

        public async Task Run(CancellationToken token = default)
        {
            Adjust();
            _board.DrawBoard();
            PrintFood(food.X, food.Y);

            while (!token.IsCancellationRequested)
            {
                WaitKeyPressAndSetDirectionAndDelay();
                PrintStatusBar();

                var oldTail = _segments.Last.Value;
                var oldHead = _segments.First.Value;
                    
                var x = CurrentDirection == Direction.Right ? 1 : CurrentDirection == Direction.Left ? -1 : 0;
                var y = CurrentDirection == Direction.Down ? 1 : CurrentDirection == Direction.Up ? -1 : 0;
                var head = new Point(oldHead.X + x, oldHead.Y + y);
                _segments.AddFirst(head);
                
                if (head == food)
                {
                    await CreateFood();
                    PrintFood(food.X, food.Y);

                    footEaten++;
                    PrintStatusBar();
                }
                else
                {
                    _segments.RemoveLast();
                    ConsoleHelper.ClearText(oldTail.X, oldTail.Y);
                }

                DrawSnakeHead(head, CurrentDirection);
            
                ConsoleHelper.ResetCursorPosition(oldHead.X, oldHead.Y);
                Console.Write(SnakeChar);

                bool hasCollision = HasCollision(head.X, head.Y);
                if (hasCollision) 
                { 
                    await PrintGameOver();
                    break;
                }

                await Task.Delay((int)GetDelay(), token);
            }
        }

        private bool HasCollision(int x, int y)
        {
            return x == _board.BorderRec.X
                || x == _board.BorderRec.Right
                || y == _board.BorderRec.Y
                || y == _board.BorderRec.Bottom
                || _segments.Skip(1).Any(i => i.X == x && i.Y == y);
        }

        private void WaitKeyPressAndSetDirectionAndDelay()
        {
            if (!Console.KeyAvailable)
                return;

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.P)
            {
                isPaused = !isPaused;

                if (isPaused)
                {
                    string pauseMessage = "PAUSED. Press any key to continue.";
                    PrintStatusBarWithMessage(pauseMessage);
                    Console.ReadKey(true);
                    isPaused = false;
                }
            }
            else if (key == ConsoleKey.UpArrow)
            {
                CurrentDirection = Direction.Up;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                CurrentDirection = Direction.Down;
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                CurrentDirection = Direction.Left;
            }
            else if (key == ConsoleKey.RightArrow)
            {
                CurrentDirection = Direction.Right;
            }
            else if(key == ConsoleKey.Spacebar)
            {
                speedboosted = !speedboosted;
            }
        }

        public void Adjust()
        {
            // Yılan gövdesi için bir linked list tanımlıyoruz.
            _segments = new LinkedList<Point>();

            for (int i = 0; i < InitialSnakeLenght; i++)
            {
                _segments.AddLast(new Point(_board.BorderRec.Left + SnakeStartingPoint.X - i, _board.BorderRec.Top + SnakeStartingPoint.Y));
            }

            _emptyPoints = new HashSet<Point>(_board.BorderRec.Height * _board.BorderRec.Width);
            // Board Matris bir yapıda olduğu için n^2 dolaşmak zorundayız.
            for (int i = _board.BorderRec.Left + 1; i < _board.BorderRec.Right; i++)
            {
                for (int j = _board.BorderRec.Top + 1; j < _board.BorderRec.Bottom; j++)
                {
                    _emptyPoints.Add(new Point(i, j));
                }
            }
            // Yılanın gövdesinin olduğu yerleri çıkartıyoruz
            _emptyPoints.ExceptWith(_segments);

            CreateFood().GetAwaiter().GetResult();
        }

        private async Task CreateFood(CancellationToken token = default)
        {
            if (_emptyPoints.Count == 0)
            {
                // Game Completed
                await PrintComplete(token);
                return;
            }

            // Boş bir nokta bulma
            var randomIndex = Random.Shared.Next(0, _emptyPoints.Count);
            food = _emptyPoints.ElementAt(randomIndex);
            _emptyPoints.Remove(food);
        }

        private static void DrawSnakeHead(Point position, Direction currentDirection)
        {
            ConsoleHelper.ResetCursorPosition(position.X, position.Y);

            var headChar = currentDirection switch
            {
                Direction.Up => '^',
                Direction.Down => 'v',
                Direction.Left => '<',
                Direction.Right => '>',
                _ => throw new NotImplementedException()
            };

            Console.Write(headChar);
        }

        private void PrintFood(int x, int y)
        {
            ConsoleHelper.ResetCursorPosition(x, y);
            Console.Write(FoodChar);
        }

        public void PrintStatusBar()
        {
            var message = string.Format($"Eaten: {footEaten}, L: {_segments.Count}, S: {_board.BorderRec.Width + "x" + _board.BorderRec.Height}, Dir: {CurrentDirection.ToString()}, Delay: {GetDelay().ToString("#")}, Food: {_board.BorderRec.X + food.X}, {_board.BorderRec.Y + food.Y}, Head: {_segments.First.Value.X}, {_segments.First.Value.Y}");

            if (isPaused)
                message += " Status: PAUSED";

            if (speedboosted)
                message += " SPEED BOOSTED";

            PrintStatusBarWithMessage(message);
        }

        private void PrintStatusBarWithMessage(string message)
        {
            ConsoleHelper.ClearLine(0);
            ConsoleHelper.ResetCursorPosition(_board.BorderRec.Left);
            Console.Write(message);
        }

        public async Task PrintComplete(CancellationToken token = default)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            _board.DrawBoard();
            PrintStatusBar();

            string message = "Completed";
            var midPoint = GetCenterOfBoard(message.Length / 2);

            await ConsoleHelper.PrintBlinkingText(message, midPoint, 600, token);
        }

        private async Task PrintGameOver(CancellationToken token = default)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            _board.DrawBoard();
            PrintStatusBar();

            string message = "Game Over";
            var midPoint = GetCenterOfBoard(message.Length / 2);

            await ConsoleHelper.PrintBlinkingText(message, midPoint, 400, token);
        }

        private Point GetCenterOfBoard(int xOffset = 0, int yOffset = 0)
        {
            return new Point(((_board.BorderRec.Left + _board.BorderRec.Right) / 2) - xOffset, ((_board.BorderRec.Top + _board.BorderRec.Bottom) / 2) - yOffset);
        }

        private double GetDelay()
        {
            var defaultDelay = speedboosted ? delayBoost : delay;
            return CurrentDirection == Direction.Left || CurrentDirection == Direction.Right ? defaultDelay : defaultDelay / DEFAULT_VERTICAL_DELAY_RATIO;
        }
    }
}
