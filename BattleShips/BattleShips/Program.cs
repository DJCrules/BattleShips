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
            while (true)
            {
                Do_Choice(MenuScreen());
            }
        }

        //Main Procedures
        static SaveGame Initialise_Game(string[] players, int ID)
        {
            //Making a clean board
            SaveGame game = new SaveGame();
            game.turn = 0;
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
            game.ID = ID;
            return game;
        }
        static string[] Intro()
        {
            Console.Clear();
            title(20);
            Console.WriteLine(
                "Enter player names, if you would like \n" +
                "to play against a computer, call one of \n" +
                "the players 'computer'");

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
            // Show the user a menu of the following options:
            // 0 read instructions
            // 1 create a new game
            // 2 load an old game
            // 3 remove an old game
            // 4 exit from the game
            // 101 secret choice to wipe save games
            Console.Clear ();
            title(25);
            string choice = "";
            while (choice == "" | choice == null)
            {
                Console.Clear();
                title(0);
                Console.WriteLine
                ("==========Menu===========\n\n" +
                 "0. Show Instructions\n" +
                 "1. New Game\n" +
                 "2. Load Game\n" +
                 "3. Remove Save Game\n" +
                 "4. Exit\n\n");
                Console.Write("Enter choice: ");
                choice = Console.ReadLine();
            }
            return int.Parse(choice);
        }
        static void Do_Choice(int choice)
        {
            switch (choice)
            {
                case 0:
                    Instructions();
                    break;

                case 1:
                    StartGame();
                    break;               

                case 2:
                    List<string> games = fetch_games();
                    if (games != null)
                    {
                        foreach (string game in games)
                        {
                            Console.WriteLine(game);
                        }
                        Console.Write("\n\nWhich game number to play? ");
                        stage_one(load_game(int.Parse(Console.ReadLine())));
                    }
                    else
                    {
                        Console.WriteLine("No saved games");
                        Console.ReadLine();
                    }
                    break;

                case 3:
                    games = fetch_games();
                    if (games != null)
                    {
                        foreach (string game in games)
                        {
                            Console.WriteLine(game);
                        }
                        Console.Write("\n\nWhich game to delete? ");
                        File.Delete(Console.ReadLine());
                    }
                    else
                    {
                        Console.WriteLine("No saved games");
                        Console.ReadLine();
                    }
                    break;

                case 4:
                    System.Environment.Exit(0);
                    break;

                case 101:
                    Console.WriteLine("Are you sure???");
                    if (Console.ReadLine() == "yes")
                    { clear_games(); }
                    break;
            }
        }
        static SaveGame StartGame()
        {
            SaveGame newgame = new_game(Intro());
            newgame = stage_one(newgame);
            return newgame;
        }
        static void Instructions()
        {
            Console.Clear();
            title(20);
            Console.WriteLine(
                "Battleships is a game in which both you and your opponent have ships in your friendly waters\n" +
                "You can only see your friendly waters but you can attempt to strateigically bomb the enemy boats\n" +
                "The first player to have successfully bombed and sunk all of the enemy boats wins the game\n\n" +
                "You can play against a computer if you want, the easy mode is entirely random, and the harder\n" +
                "mode uses a simple algorithm.");
            Console.Write("\n\nGood luck, press enter to return to the menu.");
            Console.ReadLine();
        }


        //Gameplay Procedures
        static SaveGame stage_one(SaveGame game)
        {
            if (game.started) { stage_two(game); }

            if (game.players[0] != "computer")
            {
                place_boats(game, 1);
            }

            game.started = true;
            game = stage_two(game);
            return game;
        }
        static SaveGame stage_two(SaveGame game)
        {
            if (!game.started) { stage_one(game); }

            if (game.players[0] == "computer")
            {
                game = computer_turn(game);
            }
            else
            {
                game = player_turn(game, 1);
            }
            return game;
        }
        static SaveGame computer_turn(SaveGame game)
        {
            if (!game.started) { stage_one(game); }

            return game;
        }
        static SaveGame player_turn(SaveGame game, int player)
        {
            if (!game.started) { stage_one(game); }

            title(0);
            show_board(game, player);
            Console.Write("\nAim for:  ");

            int[] pos = pos_to_int(Console.ReadLine());

            hit_square(game, player, pos[0], pos[1]);

            show_board(game, player);

            game.turn++;
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
        static SaveGame place_boats(SaveGame game, int player)
        {
            //ask the user to place boats
            string pos = "";
            while (!is_valid(pos))
            {
                Console.Clear();
                title(30);
                show_board(game, player);
                Console.WriteLine(
                    "Placing dingy (1 tile)\n\n" +
                    "Enter Coordinates for boat: ");
                pos = Console.ReadLine();
            }
            
            return game;
        }


        //Pretty Procedures
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
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ______  ______  ______ ______ __      ______    \r\n/\\  == \\/\\  __ \\/\\__  _/\\__  _/\\ \\    /\\  ___\\   \r\n\\ \\  __<\\ \\  __ \\/_/\\ \\\\/_/\\ \\\\ \\ \\___\\ \\  __\\   \r\n \\ \\_____\\ \\_\\ \\_\\ \\ \\_\\  \\ \\_\\\\ \\_____\\ \\_____\\ \r\n  \\/_____/\\/_/\\/_/  \\/_/   \\/_/ \\/_____/\\/_____/ \r\n                                                 \r\n       ______  __  __  __  ______ ______         \r\n      /\\  ___\\/\\ \\_\\ \\/\\ \\/\\  == /\\  ___\\        \r\n      \\ \\___  \\ \\  __ \\ \\ \\ \\  _-\\ \\___  \\       \r\n       \\/\\_____\\ \\_\\ \\_\\ \\_\\ \\_\\  \\/\\_____\\      \r\n        \\/_____/\\/_/\\/_/\\/_/\\/_/   \\/_____/  \n\n", Console.ForegroundColor);
            loadbar(n);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void show_board(SaveGame game, int player)
        {
            Console.WriteLine("   1 2 3 4 5 6 7 8 9 0");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(GetAlphabetLetter(i + 1) + "  ");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(game.gameboard[player - 1, i, j] + " ");
                }
                Console.Write("\n");
            }
        }


        //Filing Procedures
        static void save_game(SaveGame game)
        {
            // Stored in the bin/Debug folder by default
            string filename = $"SaveGame{game.ID}.bin";

            // Declare and initialise a BinaryWriter in Create mode
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                writer.Write(game.turn);
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
                writer.Write(game.ID);
            }
        }
        static SaveGame load_game(int game_number)
        {
            string filename = $"SaveGame{game_number}.bin";
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                // Declare an Item object of type SaveGame
                SaveGame game = new SaveGame();

                game.turn = reader.ReadUInt16();
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
                game.ID = reader.ReadUInt16();
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
        static SaveGame new_game(string[] players)
        {
            int ID = 0;
            string filename = $"SaveGame{ID}.bin";

            while (File.Exists(filename))
            {
                ID++;
            }
            SaveGame game = Initialise_Game(players, ID);
            save_game(game);
            return game;
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
            if (char.IsLetter(letter))
            {
                return (letter - 'a') + 1;
            }
            else
            {
                return letter;
            }
        }
        public static bool is_valid(string pos)
        {
            if (string.IsNullOrEmpty(pos) || pos.Length < 2 || pos.Length > 3)
            {
                return false;
            }

            char row = char.ToUpper(pos[0]);
            string columnPart = pos.Substring(1);

            // Check if row is within 'A' to 'J'
            if (row < 'A' || row > 'J')
            {
                return false;
            }

            // Check if column is a valid integer and within 1 to 10
            if (!int.TryParse(columnPart, out int column) || column < 1 || column > 10)
            {
                return false;
            }

            return true;
        }
        static int[] pos_to_int(string pos)
        {
            return [GetAlphabetNumber(pos[0]), GetAlphabetNumber(pos[1])];
        }
    }
    class SaveGame()
    {
        public int turn = 0;
        public string[] players = new string[2];
        public char[,,] gameboard = new char[2, 10, 10];
        public bool started;
        public int ID;
    }
}