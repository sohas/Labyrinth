using System;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxGameNumber = 1;
            Console.WriteLine($"it is labyrinth game. put number 1-{maxGameNumber} to choose labyrinth.\n\n" +
                $"you can explore it by walking (put arrows). firstly you see only X in every cell.\n" +
                $"your cell is marked as i. when you walk, you can see path. there are some holes in every labyrinth.\n" +
                $"if you get hole, you are transfer to another point. so you get fully unknown map.\n\n" +
                $"you can see all your previous walks py putting P.\n\n" +
                $"your aim is to explore labyrinth and undersand it full view.\n" +
                $"to quit game put escape.\n");

            int gameNumber = 0;
            bool isParsed = false;

            while (!isParsed || gameNumber < 1 || gameNumber > maxGameNumber)
            {
                Console.WriteLine($"put number 1-{maxGameNumber} to choose labyrinth or escape to quit.");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    return;
                }

                isParsed = int.TryParse(key.KeyChar + Console.ReadLine(), out gameNumber);
            }
            Console.WriteLine();

            string directory = Environment.CurrentDirectory + "\\Maps\\";
            string mapName = $"maze{gameNumber}.txt";
            Map map = new Map(directory + mapName);
            Explore explore = new Explore(map);

            explore.Move();
        }
    }
}
