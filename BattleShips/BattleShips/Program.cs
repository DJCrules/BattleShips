using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BattleShips
{
    class Program
    {
        static void Main(string[] args)
        {
            clear_games();
            SaveGame newgame = Initialise_Game(["joe", "jam"]);
            show_board(newgame, 1);
            newgame = hit_square(newgame, 1, 2, 3);
            show_board(newgame, 1);
        }

        //Main Procedures
        static SaveGame Initialise_Game(string[] players)
        {
            SaveGame game = new SaveGame();
            for (int n = 0; n < 2; n++)
            {
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


        //Gameplay Procedures
        static void StageOne(SaveGame game)
        {
            if (game.started) { StageTwo(game); }

        }
        static void StageTwo(SaveGame game)
        {
            if (!game.started) { StageOne(game); }
        }
        static SaveGame computer_turn(SaveGame game)
        {
            if (!game.started) { StageOne(game); }

            return game;
        }
        static SaveGame player_turn(SaveGame game, int player)
        {
            if (!game.started) { StageOne(game); }

            title(0);
            show_board(game, player);
            Console.Write("\nAim for:  ");

            string pos = Console.ReadLine();

            return game;
        }
        static SaveGame hit_square(SaveGame game, int n, int i, int j)
        {
            if (game.gameboard[n - 1, i, j] == '~')
            {
                game.gameboard[n - 1, i, j] = 'X';
            }
            if (game.gameboard[n - 1, i, j] == '@')
            {
                game.gameboard[n - 1, i, j] = '!';
            }
            return game;
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
            Console.WriteLine("   1 2 3 4 5 6 7 8 9 0");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(GetAlphabetLetter(i + 1) + "  ");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(game.gameboard[boardnumber - 1, i, j] + " ");
                }
                Console.Write("\n");
            }
        }


        //Filing Procedures
        static int save_game(SaveGame game)
        {
            int filenumber = 0;
            // Stored in the bin/Debug folder by default
            string filename = $"SaveGame{filenumber}.bin";

            while (File.Exists(filename))
            {
                filenumber++;
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
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
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
        static void clear_games()
        {
            int filenumber = 0;
            string filename = $"SaveGame{filenumber}.bin";
            while (File.Exists(filename))
            {
                File.Delete(filename);
                filenumber++;
                filename = $"SaveGame{filenumber}.bin";
            }
        }
        static List<string> fetch_games()
        {
            string fullpath = """C:\\Users\\dylan\\Documents\\GitHub\\BattleShips\\BattleShips\\BattleShips\\bin\\Debug\\net8.0""";
            string[] files = Directory.GetFiles(fullpath);
            List<string> names = new List<string>();
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension == ".bin")
                    names.Add(fi.Name);
            }
            return names;
        }

        //Other Procedures
        static string GetAlphabetLetter(int number)
        {
            if (number < 1 || number > 25)
            {
                return "Invalid input";
            }

            char letter = (char)('a' + (number - 1));
            return letter.ToString();
        }
        static int GetAlphabetNumber(char letter)
        {
            if (letter < 'a' || letter > 'z')
            {
                return 30;
            }

            int number = (letter - 'a') + 1;
            return number;
        }
    }
    class SaveGame()
    {
        public string[] players = new string[2];
        public char[,,] gameboard = new char[2, 10, 10];
        public bool started;
    }
}

