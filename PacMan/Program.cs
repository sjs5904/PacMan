using System;
using System.Diagnostics;
using System.Media;
using System.Threading;

namespace PacMan
{
    class Program
    {
        public static SoundPlayer sp;
        static Thread thread;
        static Stopwatch myclock;
        public static long timeStamp { get; private set; }
        public static long startTime { get; private set; }
        public static int framePerSec = 60;
        public static GameManager game { get; private set; }

        static void Main(string[] args)
        {
            RunStartUp();
        }

        static void RunStartUp()
        {
            Init();
            while (!game.isOver)
            {
                Render();
                Update();
            }
            Release();
        }

        static void Init()
        {
            sp = new SoundPlayer();
            sp.SoundLocation = Environment.CurrentDirectory + @"\pacman_beginning.wav";
            sp.Play();
            timeStamp = 0;
            startTime = 0;
            game = new GameManager();
            ConsoleHelper.SetCurrentFont("Lucida Console", 6);
            Console.SetWindowSize(237, 138);
            Console.Clear();
            Console.CursorVisible = false;
            myclock = new Stopwatch();
            game.Rendering();
            while (Console.ReadKey().Key != ConsoleKey.Enter && Console.ReadKey().Key != ConsoleKey.Spacebar) ;
            myclock.Start();
            thread = new Thread(() => Input());
            thread.Start();
            startTime = myclock.ElapsedMilliseconds;
        }

        static void Render()
        {
            game.Rendering();
            timeStamp = myclock.ElapsedMilliseconds;
        }

        static void Update()
        {
            game.Update();
            while (myclock.ElapsedMilliseconds - timeStamp < 1000 / framePerSec) ;
        }

        static void Release()
        {
            Console.Clear();
            ConsoleHelper.SetCurrentFont("Lucida Console", 20);
            Console.SetWindowSize(82, 24);
            if (game.isVictory)
                game.PrintVictory();
            else 
                game.PrintDefeat();
            Console.WriteLine($"\t Your Score:  {game.Score - (timeStamp - startTime) / 200}");
            Console.WriteLine("\t Press R to Restart!! ");
            while (Console.ReadKey().Key != ConsoleKey.R) ;
            RunStartUp();
        }

        static void Input()
        {
            ConsoleKeyInfo c;
            while (!game.isOver)
            {
                c = Console.ReadKey(true);
                switch (c.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            game.PlayerSetDirection(Direction.UP);
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            game.PlayerSetDirection(Direction.DOWN);
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            game.PlayerSetDirection(Direction.RIGHT);
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            game.PlayerSetDirection(Direction.LEFT);
                            break;
                        }
                }
            }
        }
    }
}
