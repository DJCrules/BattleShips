static string[] intro()
{
    Console.WriteLine("Welcome to BattleShips\n");
    Console.Write("Enter First Name: ");
    player1 = Console.ReadLine();
    Console.Write("\nEnter Second Name: ");
    player2 = Console.ReadLine();
    return [player1, player2];
}

intro();

