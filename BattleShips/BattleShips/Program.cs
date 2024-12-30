using System;
using System.Buffers;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BattleShips
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true) { Do_Choice(MenuScreen()); }
        }

        //Main Procedures#
        static void PlayGame(SaveGame newgame)
        {
            newgame = stage_one(newgame);
            save_game(newgame);
            title(10);
            Console.Write($"\n\nSaved game as SaveGame{newgame.ID}.bin\n\nPress enter to return to menu");
            Console.ReadLine();
            return;
        }
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
        static string[] Fetch_Names()
        {
            title(20);
            Console.WriteLine(
                "Enter player names, if you would like \n" +
                "to play against a computer, call one of \n" +
                "the players 'computer'");

            Console.Write("\n\nEnter First Player: ");
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
            title(10);
            string ?choice = "";
            while (choice == "" | choice == null)
            {
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
            try
            {
                return int.Parse(choice);
            }
            catch
            {
                return -1;
            }
        }
        static void Do_Choice(int choice)
        {
            switch (choice)
            {
                //Instructions
                case 0:
                    Instructions();
                    break;

                //Start a new game
                case 1:
                    SaveGame newgame = new_game(Fetch_Names());
                    PlayGame(newgame);
                    break;

                //Load an old game
                case 2:
                    List<string> games = fetch_games();
                    if (games.Any())
                    {
                        Console.WriteLine();
                        foreach (string game in games)
                        {
                            Console.WriteLine(game);
                        }
                        Console.Write("\n\nWhich game number to play? ");
                        string game_number = Console.ReadLine();

                        if (int.TryParse(game_number, out int num))
                        {
                            PlayGame(load_game(num));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No saved games");
                        Console.ReadLine();
                    }
                    break;

                //Delete an old game
                case 3:
                    games = fetch_games();
                    if (games.Any())
                    {
                        Console.WriteLine();
                        foreach (string game in games)
                        {
                            Console.WriteLine(game);
                        }
                        Console.Write("\n\nWhich game number to delete? ");
                        string game_number = Console.ReadLine();
                        if (game_number != null)
                        {
                            File.Delete($"SaveGame{game_number}.bin");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nNo saved games");
                        Console.ReadLine();
                    }
                    break;

                //Exit
                case 4:
                    System.Environment.Exit(0);
                    break;

                //Clear all games (secret)
                case 101:
                    Console.WriteLine("Are you sure???");
                    if (Console.ReadLine() == "yes")
                    { clear_games(); }
                    break;

                //invalid input
                case -1:
                    break;
            }
        }
        static void Instructions()
        {
            title(5);
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

            //high tier
        static SaveGame stage_one(SaveGame game)
        {
            if (game.started) { return stage_two(game); }
            foreach (string player in game.players)
            {
                int num = Array.IndexOf(game.players, player) + 1;
                place_boats(game, num);

                title();
                show_board(game, num);
                Console.Write($"\n\n{player}'s boats placed" +

                    $"\n\nenter q to quit or nothing to continue: ");
                if (Console.ReadLine() == "q") { return game; }
            }
            game.started = true;
            game = stage_two(game);
            return game;
        }
        static SaveGame stage_two(SaveGame game)
        {
            if (!game.started) { return stage_one(game); }
            int totalships = 0; foreach (char c in game.gameboard) { if (c == '@' || c == '!') { totalships++; } }

            while (true)
            {
                foreach (string player in game.players)
                {
                    Console.WriteLine($"Turn {(game.turn / 2) + 1}");

                    if (player.ToLower() == "computer")
                    {
                        int player_number = Array.IndexOf(game.players, player) + 1;
                        game = computer_turn(game, player_number);
                        game.turn++;
                        title();
                        Console.Write("\nComputer's turn complete\n\nenter q to quit or nothing to continue: ");
                        if (Console.ReadLine() == "q") { return game; }
                    }
                    else
                    {
                        int player_number = Array.IndexOf(game.players, player) + 1;
                        game = player_turn(game, player_number);
                        game.turn++;
                        title();
                        Console.Write($"\n{player}'s turn complete\n\nenter q to quit or nothing to continue: ");
                        if (Console.ReadLine() == "q") { return game; }
                    }
                }
            }
        }

            //medium tier
        static SaveGame place_boats(SaveGame game, int player)
        {
            if (game.players[player - 1].ToLower() != "computer")
            {
                //ask the user to place dingy
                game = place_boat(game, player, 1, "dingy");

                //ask the user to place sub
                game = place_boat(game, player, 2, "submarine");

                //ask the user to place destroyer
                game = place_boat(game, player, 3, "destroyer");

                //ask the user to place carrier
                game = place_boat(game, player, 4, "carrier");
            }
            else
            {
                //random place dingy
                game = random_boat(game, player, 1);

                //random place sub
                game = random_boat(game, player, 2);

                //random place destroyer
                game = random_boat(game, player, 3);

                //random place carrier
                game = random_boat(game, player, 4);
            }
            return game;
        }
        static SaveGame computer_turn(SaveGame game, int player)
        {
                
                
            return game;
        }
        static SaveGame player_turn(SaveGame game, int player)
        {
            int[] pos = get_pos(game, player);

            hit_square(game, player, pos[0], pos[1]);
            
            show_board(game, player);

            game.turn++;
            return game;
        }

            //low tier
        static SaveGame hit_square(SaveGame game, int board, int i, int j)
        {
            if (game.gameboard[board - 1, i, j] == '~')
            {
                game.gameboard[board - 1, i, j] = 'X';
            }
            if (game.gameboard[board - 1, i, j] == '@')
            {
                game.gameboard[board - 1, i, j] = '!';
            }
            return game;
        }
        static SaveGame place_boat(SaveGame game, int player, int length, string boat)
        {
            bool up = false;
            string? pos = "";
            bool validPlacement = false;

            while (!validPlacement)
            {
                Console.Clear();
                title();
                Console.WriteLine($"Player {game.players[player - 1]} placing boats");
                show_board(game, player);
                Console.WriteLine($"Placing {boat} ({length}x1 tiles)\n");
                Console.Write("Enter coordinates for the boat (e.g., A1): ");
                pos = Console.ReadLine();

                if (!is_valid(pos))
                {
                    Console.WriteLine("Invalid coordinates. Please try again.");
                    continue;
                }

                int[] converted_pos = pos_to_int(pos);

                if (length > 1)
                {
                    Console.Write("What direction should the boat go in? (up or right): ");
                    string? direction = Console.ReadLine();
                    up = direction?.ToLower() == "up";
                }

                if (can_place_boat(game, player, converted_pos, length, up))
                {
                    place_boat_on_board(game, player, converted_pos, length, up);
                    validPlacement = true;
                }
                else
                {
                    Console.WriteLine("Invalid placement. The boat does not fit or overlaps with another boat. Press any key to try again.");
                    Console.ReadKey();
                }
            }

            return game;
        }
        static bool can_place_boat(SaveGame game, int player, int[] pos, int length, bool up)
        {
            int boardSize = game.gameboard.GetLength(1); // Assuming square board
            int row = pos[0], col = pos[1];

            for (int k = 0; k < length; k++)
            {
                int newRow = up ? row - k : row;
                int newCol = up ? col : col + k;

                if (newRow < 0 || newRow >= boardSize || newCol < 0 || newCol >= boardSize || game.gameboard[player - 1, newRow, newCol] != '~')
                {
                    return false;
                }
            }
            return true;
        }
        static void place_boat_on_board(SaveGame game, int player, int[] pos, int length, bool up)
        {
            int row = pos[0], col = pos[1];

            for (int k = 0; k < length; k++)
            {
                int newRow = up ? row - k : row;
                int newCol = up ? col : col + k;
                game.gameboard[player - 1, newRow, newCol] = '@';
            }
        }
        static int[] get_pos(SaveGame game, int player)
        {
            title(0);
            show_board(game, player);
            Console.Write("\nAim for:  ");
            try
            {
                return pos_to_int(Console.ReadLine());
            }
            catch
            {
                return get_pos(game, player);
            }
        }
        static SaveGame random_boat(SaveGame game, int player, int length)
        {
            int[] converted_pos = new int[2];
            bool up;
            Random random = new Random();
            up = random.Next(2) == 1;

            while (true)
            {
                converted_pos[0] = random.Next(10);
                converted_pos[1] = random.Next(10);

                try
                {
                    for (int k = 0; k < length; k++)
                    {
                        if (up) // Vertical placement
                        {
                            if (converted_pos[0] - k < 0 || game.gameboard[player - 1, converted_pos[0] - k, converted_pos[1]] != '~')
                                throw new Exception();
                        }
                        else // Horizontal placement
                        {
                            if (converted_pos[1] + k >= 10 || game.gameboard[player - 1, converted_pos[0], converted_pos[1] + k] != '~')
                                throw new Exception();
                        }
                    }

                    // Place the boat
                    for (int k = 0; k < length; k++)
                    {
                        if (up)
                        {
                            game.gameboard[player - 1, converted_pos[0] - k, converted_pos[1]] = '@';
                        }
                        else
                        {
                            game.gameboard[player - 1, converted_pos[0], converted_pos[1] + k] = '@';
                        }
                    }

                    return game; 
                }
                catch
                {
                    continue;
                }
            }
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
            Console.WriteLine("\n   1 2 3 4 5 6 7 8 9 0");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(GetAlphabetLetter(i + 1) + "  ");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(game.gameboard[player - 1, i, j] + " ");
                }
                Console.Write("\n");
            }
            Console.WriteLine();
        }


        //Filing Procedures
        static void save_game(SaveGame game)
        {
            string filename = $"SaveGame{game.ID}.bin";

            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                // Save turn
                writer.Write(game.turn);

                // Save player names
                foreach (string player in game.players)
                {
                    writer.Write(player);
                }

                // Save the gameboard
                for (int n = 0; n < game.gameboard.GetLength(0); n++)
                {
                    for (int i = 0; i < game.gameboard.GetLength(1); i++)
                    {
                        for (int j = 0; j < game.gameboard.GetLength(2); j++)
                        {
                            writer.Write(game.gameboard[n, i, j]);
                        }
                    }
                }

                // Save additional properties
                writer.Write(game.started);
                writer.Write(game.ID);
            }
        }
        static SaveGame load_game(int game_number)
        {
            string filename = $"SaveGame{game_number}.bin";

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    SaveGame game = new SaveGame();

                    game.turn = reader.ReadInt32();

                    for (int n = 0; n < game.players.Length; n++)
                    {
                        game.players[n] = reader.ReadString();
                    }

                    for (int n = 0; n < game.gameboard.GetLength(0); n++)
                    {
                        for (int i = 0; i < game.gameboard.GetLength(1); i++)
                        {
                            for (int j = 0; j < game.gameboard.GetLength(2); j++)
                            {
                                game.gameboard[n, i, j] = reader.ReadChar();
                            }
                        }
                    }

                    game.started = reader.ReadBoolean();
                    game.ID = reader.ReadInt32();

                    return game;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Save file not found: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
            }

            return default(SaveGame);
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
                filename = $"SaveGame{ID}.bin";
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
                return (letter - 'a');
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
            return [GetAlphabetNumber(pos[0]), int.Parse(pos[1].ToString()) - 1];
        }
    }
    struct SaveGame()
    {
        public int turn = 0;
        public string[] players = new string[2];
        public char[,,] gameboard = new char[2, 10, 10];
        public bool started;
        public int ID;
    }
}