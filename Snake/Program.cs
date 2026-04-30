namespace Snake
{
    internal class Program
    {
        const char Wall = '\u2588';
        const char SnakeTail = 'O';
        const char SnakeHead = '0';
        const string Field = "  ";
        const string UpArrow = "UpArrow";
        const string DownArrow = "DownArrow";
        const string LeftArrow = "LeftArrow";
        const string RightArrow = "RightArrow";
        Coordinates FieldSize = new Coordinates(2, 1);

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            //Console.WriteLine("Input matrix size:");

            //int matrixSize = int.Parse(Console.ReadLine());
            Console.CursorVisible = false;
            int matrixSize = 20;

            CreateMatrix(matrixSize);

            SetCarriageToIntialState(matrixSize);

            Play(matrixSize);

            PrintGameOver(matrixSize);

            Console.ReadKey();
        }

        static void Play(int matrixSize)
        {
            bool initialState = true;
            bool shouldDropBody = false;
            string input = RightArrow;
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;
            string currentDirection = RightArrow;

            Coordinates headPosition = new Coordinates(Console.CursorLeft, Console.CursorTop);
            Coordinates nextHeadPosition = new Coordinates();
            Coordinates prevHeadPosition = new Coordinates(headPosition.x, headPosition.y);

            Coordinates droppedFood = new Coordinates();
            Queue<Coordinates> digestibleFood = new Queue<Coordinates>();

            List<Coordinates> tail = new List<Coordinates>();
            List<Coordinates> nextTailPos = new List<Coordinates>();
            List<Coordinates> prevTailPosition = new List<Coordinates>();

            while (IsGameOver(cursorLeft, cursorTop, matrixSize))
            {
                MoveHead(ref headPosition, nextHeadPosition, ref prevHeadPosition, initialState);

                if (tail.Count > 0 || digestibleFood.Count > 0)
                {
                    MoveTail(headPosition, ref tail, ref nextTailPos, ref prevTailPosition, ref digestibleFood);
                }

                if (shouldDropBody || initialState)
                {
                    DropFood(headPosition, matrixSize, ref droppedFood);

                    initialState = false;
                    shouldDropBody = false;
                }

                if (headPosition.x == droppedFood.x && headPosition.y == droppedFood.y)
                {
                    AddFoodToQueue(ref digestibleFood, droppedFood, ref shouldDropBody);
                }

                Thread.Sleep(200);

                cursorLeft = headPosition.x + 2;
                cursorTop = headPosition.y;

                HandleSnakeDirection(ref input, ref currentDirection, ref cursorLeft, ref cursorTop, headPosition, ref nextHeadPosition, prevHeadPosition, prevTailPosition);
            }
        }

        static void MoveHead(ref Coordinates headPosition, Coordinates nextHeadPosition, ref Coordinates prevHeadPosition, bool initialState)
        {
            if (!initialState)
            {
                headPosition = new Coordinates(nextHeadPosition.x, nextHeadPosition.y);
                prevHeadPosition = headPosition;
            }

            Console.SetCursorPosition(headPosition.x, headPosition.y);

            Console.Write($"{SnakeHead} ");
        }

        static void MoveTail(Coordinates headPosition, ref List<Coordinates> tail, ref List<Coordinates> nextTailPos, ref List<Coordinates> prevTailPos, ref Queue<Coordinates> digestibleFood)
        {
            if (digestibleFood.Count > 0)
            {
                IncreaseTail(digestibleFood, tail, nextTailPos, prevTailPos);
            }

            UpdateNextTailPos(headPosition, tail, nextTailPos);

            UpdateTailAndPrevTailPos(headPosition, tail, nextTailPos, prevTailPos);
        }

        static void UpdateNextTailPos(Coordinates headPosition, List<Coordinates> tail, List<Coordinates> nextTailPos)
        {
            for (int i = 0; i < nextTailPos.Count; i++)
            {
                if (i == 0)
                {
                    nextTailPos[0] = headPosition;
                }
                else
                {
                    nextTailPos[i] = tail[i - 1];
                }
            }
        }

        static void UpdateTailAndPrevTailPos(Coordinates headPosition, List<Coordinates> tail, List<Coordinates> nextTailPos, List<Coordinates> prevTailPos)
        {
            for (int i = 0; i < nextTailPos.Count; i++)
            {
                Console.SetCursorPosition(tail[i].x, tail[i].y);
                Console.Write($"{SnakeTail} ");

                if (i == 0)
                {
                    prevTailPos[0] = tail[0];
                    tail[0] = headPosition;
                }
                else
                {
                    prevTailPos[i] = tail[i];
                    tail[i] = nextTailPos[i];
                }
            }
        }

        static void IncreaseTail(Queue<Coordinates> digestibleFood, List<Coordinates> tail, List<Coordinates> nextTailPos, List<Coordinates> prevTailPos)
        {
            nextTailPos.Add(digestibleFood.Dequeue());
            tail.Add(nextTailPos[nextTailPos.Count - 1]);
            prevTailPos.Add(tail[tail.Count - 1]);
        }

        static void AddFoodToQueue(ref Queue<Coordinates> digestibleFood, Coordinates droppedBody, ref bool shouldDropBody)
        {
            shouldDropBody = true;
            digestibleFood.Enqueue(droppedBody);
        }

        static void HandleSnakeDirection(ref string input, ref string currentDirection, ref int cursorLeft, ref int cursorTop, Coordinates headPosition, ref Coordinates nextHeadPosition, Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            if (Console.KeyAvailable)
            {
                input = Console.ReadKey(intercept: true).Key.ToString();
            }

            switch (input)
            {
                case UpArrow:
                    if (currentDirection != DownArrow)
                    {
                        TurnTop(cursorLeft, cursorTop, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                        currentDirection = UpArrow;
                    }
                    else
                    {
                        TurnDown(cursorLeft, cursorTop, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                    }
                    break;
                case DownArrow:
                    if (currentDirection != UpArrow)
                    {
                        TurnDown(cursorLeft, cursorTop, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                        currentDirection = DownArrow;
                    }
                    else
                    {
                        TurnTop(cursorLeft, cursorTop, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                    }
                    break;
                case LeftArrow:
                    if (currentDirection != RightArrow)
                    {
                        TurnLeft(cursorLeft, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                        currentDirection = LeftArrow;
                    }
                    else
                    {
                        TurnRight(cursorLeft, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                    }
                    break;
                case RightArrow:
                    if (currentDirection != LeftArrow)
                    {
                        TurnRight(cursorLeft, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                        currentDirection = RightArrow;
                    }
                    else
                    {
                        TurnLeft(cursorLeft, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                    }
                    break;
                default:
                    TurnRight(cursorLeft, headPosition, ref nextHeadPosition, prevHeadPosition, tail);
                    break;
            }
        }

        static void TurnRight(int cursorLeft, Coordinates headPosition, ref Coordinates nextHeadPosition, Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            nextHeadPosition = new Coordinates(headPosition.x + 2, headPosition.y);
            ClearPrevField(prevHeadPosition, tail);
            cursorLeft += 2;
        }

        static void TurnLeft(int cursorLeft, Coordinates headPosition, ref Coordinates nextHeadPosition, Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            nextHeadPosition = new Coordinates(headPosition.x - 2, headPosition.y);
            ClearPrevField(prevHeadPosition, tail);
            cursorLeft -= 2;
            Console.SetCursorPosition(cursorLeft - 2, Console.CursorTop);
        }

        static void TurnDown(int cursorLeft, int cursorTop, Coordinates headPosition, ref Coordinates nextHeadPosition, Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            nextHeadPosition = new Coordinates(headPosition.x, headPosition.y + 1);
            ClearPrevField(prevHeadPosition, tail);
            cursorTop++;
            Console.SetCursorPosition(cursorLeft - 2, cursorTop);
        }

        static void TurnTop(int cursorLeft, int cursorTop, Coordinates headPosition, ref Coordinates nextHeadPosition, Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            nextHeadPosition = new Coordinates(headPosition.x, headPosition.y - 1);
            ClearPrevField(prevHeadPosition, tail);
            cursorTop--;
            Console.SetCursorPosition(cursorLeft - 2, cursorTop);
        }

        static void DropFood(Coordinates headPosition, int matrixSize, ref Coordinates droppedBody)
        {
            Random random = new Random();

            int leftPos = random.Next(2, matrixSize * 2 - 2);

            if (leftPos % 2 != 0)
            {
                if (leftPos > matrixSize * 2)
                {
                    leftPos--;
                }
                else
                {
                    leftPos++;
                }
            }

            droppedBody.x = leftPos;
            droppedBody.y = random.Next(1, matrixSize - 2);
            Console.SetCursorPosition(droppedBody.x, droppedBody.y);

            Console.Write($"{SnakeTail} ");

            Console.SetCursorPosition(headPosition.x + 1, headPosition.y);
        }

        static void ClearPrevField(Coordinates prevHeadPosition, List<Coordinates> tail)
        {
            Console.SetCursorPosition(prevHeadPosition.x, prevHeadPosition.y);
            Console.Write($"{Field}");

            if (tail.Count > 0)
            {
                for (int i = 0; i < tail.Count; i++)
                {
                    Console.SetCursorPosition(tail[i].x, tail[i].y);
                    Console.Write($"{Field}");
                }
            }
        }

        static void CreateMatrix(int size, bool disableWalls = false)
        {
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    CreateWall(row, column, size);

                    CreateField(row, column, size);
                }

                Console.WriteLine(" ");
            }
        }

        static void CreateWall(int row, int column, int size)
        {
            if (column == 0 || column == size - 1)
            {
                Console.Write($"{Wall}{Wall}");
            }
            if ((column > 0 && column < size - 1 && row == 0) || (column > 0 && column < size - 1 && row == 0))
            {
                Console.Write($"{Wall}{Wall}");
            }
            if ((column > 0 && column < size - 1 && row == size - 1) || (column > 0 && column < size - 1 && row == size - 1))
            {
                Console.Write($"{Wall}{Wall}");
            }
        }

        static void CreateField(int row, int column, int size)
        {
            if (column > 0 && column < size - 1 && row > 0 && row < size - 1)
            {
                Console.Write(Field);
            }
        }

        static void SetCarriageToIntialState(int matrixSize)
        {
            Console.SetCursorPosition(2, Console.CursorTop - matrixSize + 1);
        }

        static bool IsGameOver(int cursorLeft, int cursorTop, int matrixSize)
        {
            return cursorLeft > 1 && cursorLeft < matrixSize * 2 - 1 && cursorTop > 0 && cursorTop < matrixSize - 1;
        }

        static void PrintGameOver(int matrixSize)
        {
            Console.SetCursorPosition(matrixSize / 2, Console.CursorTop + matrixSize / 2);
            Console.WriteLine("Game Over!");
        }
    }

    public struct Coordinates
    {
        public int x;
        public int y;

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
