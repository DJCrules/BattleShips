using System.Buffers;
using System.Xml.Serialization;

namespace BattleShips
{
    class Program
    {
        static void Main(string[] args)
        {
            MenuScreen();
            Intro();
        }

        //Main Procedures
        static string[] Intro()
        {
            title(20);
            Console.Write("\n\nEnter First Player: ", Console.ForegroundColor);
            string? player1 = Console.ReadLine();
            Console.Write("\nEnter Second Player: ");
            string? player2 = Console.ReadLine();
            Console.Clear();
            if (player1 != null && player2 != null)
            {
                return [player1, player2];
            }
            else
            {
                return ["player 1", "player 2"];
            }
        }
        static int MenuScreen()
        {
            title(50);
            Console.WriteLine
                ("==========Menu===========\n\n" +
                 "1. New Game\n" +
                 "2. Load Game\n" +
                 "3. Exit\n\n");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            while (choice == "" | choice == null)
            {
                Console.Clear();
                title(0);
                Console.WriteLine
                ("==========Menu===========\n\n" +
                 "1. New Game\n" +
                 "2. Load Game\n" +
                 "3. Exit\n\n");
                Console.Write("Enter choice: ");
                choice = Console.ReadLine();
            }
            return int.Parse(choice);
        }

        //Aesthetic Procedures
        static void loadbar(int length)
        {
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(length);
                Console.Write("█");
            }
            Console.WriteLine("\n");
        }
        static void title(int n = 0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ______  ______  ______ ______ __      ______    \r\n/\\  == \\/\\  __ \\/\\__  _/\\__  _/\\ \\    /\\  ___\\   \r\n\\ \\  __<\\ \\  __ \\/_/\\ \\\\/_/\\ \\\\ \\ \\___\\ \\  __\\   \r\n \\ \\_____\\ \\_\\ \\_\\ \\ \\_\\  \\ \\_\\\\ \\_____\\ \\_____\\ \r\n  \\/_____/\\/_/\\/_/  \\/_/   \\/_/ \\/_____/\\/_____/ \r\n                                                 \r\n       ______  __  __  __  ______ ______         \r\n      /\\  ___\\/\\ \\_\\ \\/\\ \\/\\  == /\\  ___\\        \r\n      \\ \\___  \\ \\  __ \\ \\ \\ \\  _-\\ \\___  \\       \r\n       \\/\\_____\\ \\_\\ \\_\\ \\_\\ \\_\\  \\/\\_____\\      \r\n        \\/_____/\\/_/\\/_/\\/_/\\/_/   \\/_____/  \n\n", Console.ForegroundColor);
            loadbar(n);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
    }
}