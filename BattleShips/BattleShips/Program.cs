using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace BattleShips
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        //Main Procedures
        static SaveGame Initialise_Game(string[] players)
        {
            SaveGame game = new SaveGame();
            for (int n = 0; n < 2; n++) {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        game.gameboard[n, i, j] = '~';
                    }
                }
            }
            game.started = false;
            game.players = players;
            return game;
        }
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
                 "3. Remove Save Game\n" +
                 "4. Exit\n\n");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();
            //Secret choice 101 to wipe save games
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
        static void StartGame()
        {

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
        static void show_board(SaveGame game, int boardnumber)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(game.gameboard[boardnumber + 1, i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //Filing Procedures
        static int save_game(SaveGame game)
        {
            int filenumber = 0;
            // Stored in the bin/Debug folder by default
            string filename = $"SaveGame{filenumber}.bin";

            while (File.Exists(filename))
            {
                filenumber ++;
                filename = $"SaveGame{filenumber}.bin";
            }

            // Declare and initialise a BinaryWriter in Create mode
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                // Write each value of the Item object to the binary file
                for (int n = 0; n < 2; n++) 
                {
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            writer.Write(game.gameboard[n, i, j]);
                        }
                    }
                }
                foreach (string player in game.players)
                {
                    writer.Write(player);
                }
                writer.Write(game.started);
            }
            return filenumber;
        }
        static SaveGame load_game(int game_number)
        {
            string filename = $"SaveGame{game_number}.bin";
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                // Declare an Item object of type SaveGame
                SaveGame game = new SaveGame();

                // Read each value of the game object from the binary file
                for (int n = 0; n < 2; n++)
                {
                    for (int i = 0;i < 10; i++)
                    {
                        for (int j = 0;j < 10; j++)
                        {
                            game.gameboard[n, i, j] = reader.ReadChar();
                        }
                    }
                }
                for (int n = 0; n < 2; n++) { game.players[n] = reader.ReadString(); }
                game.started = reader.ReadBoolean();

                // Return the game object
                return game;
            }
        }
    }
    class SaveGame()
    {
        public string[] players = new string[2];
        public char[,,] gameboard = new char[2, 10, 10];
        public bool started;
    }
}

