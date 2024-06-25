using System.Xml.Linq;

abstract class Player
{
    public char Symbol { get; private set; }
    public string Name { get; private set; }

    protected Player(char symbol, string name)
    {
        Symbol = symbol;
        Name = name;
    }

    public abstract void TakeTurn(Board board);
}

class HumanPlayer : Player
{
    public HumanPlayer(char symbol, string name) : base(symbol, name) { }

    public override void TakeTurn(Board board)
    {
        int column;
        while (true)
        {
            Console.Write($"{Name} (Player {Symbol}), enter column (0-6): ");
            if (int.TryParse(Console.ReadLine(), out column) && column >= 0 && column < 7)
            {
                if (board.PlacePiece(column, Symbol))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Column is full, try another one.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input, please enter a number between 0 and 6.");
            }
        }
    }
}
class AIPlayer : Player
{
    public AIPlayer(char symbol) : base(symbol, "AI") { }

    public override void TakeTurn(Board board)
    {
        Random random = new Random();
        int column;
        while (true)
        {
            column = random.Next(0, 7);
            if (board.PlacePiece(column, Symbol))
            {
                Console.WriteLine($"AI (Player {Symbol}) places piece in column {column}.");
                break;
            }
        }
    }
}

class ConnectFourGame
{
    private Board board;
    private Player[] players;
    private int currentPlayerIndex;

    public ConnectFourGame(bool singlePlayer)
    {
        board = new Board();
        players = new Player[2];

        Console.Write("Enter name for Player 1 (Symbol X): ");
        string name1 = Console.ReadLine();
        players[0] = new HumanPlayer('X', name1);

        if (singlePlayer)
        {
            players[1] = new AIPlayer('O');
        }
        else
        {
            Console.Write("Enter name for Player 2 (Symbol O): ");
            string name2 = Console.ReadLine();
            players[1] = new HumanPlayer('O', name2);
        }

        currentPlayerIndex = 0;
    }

    public void Play()
    {
        bool isRunning = true;
        while (isRunning)
        {
            Console.Clear();
            board.Display();
            players[currentPlayerIndex].TakeTurn(board);

            if (board.CheckForWin(players[currentPlayerIndex].Symbol))
            {
                Console.Clear();
                board.Display();
                Console.WriteLine($"{players[currentPlayerIndex].Name} (Player {players[currentPlayerIndex].Symbol}) wins!");
                isRunning = false;
            }
            else if (board.IsFull())
            {
                Console.Clear();
                board.Display();
                Console.WriteLine("It's a tie!");
                isRunning = false;
            }

            currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        }
    }

    static void Main(string[] args)
    {
        bool playAgain = true;
        while (playAgain)
        {
            try
            {
                bool validInput = false;
                bool singlePlayer = false;

                while (!validInput)
                {
                    Console.Write("Choose game mode: 1 for Single Player, 2 for Two Players: ");
                    string input = Console.ReadLine();

                    if (input == null)
                    {
                        throw new ArgumentNullException("Input cannot be null.");
                    }

                    if (int.TryParse(input, out int mode))
                    {
                        if (mode == 1)
                        {
                            singlePlayer = true;
                            validInput = true;
                        }
                        else if (mode == 2)
                        {
                            singlePlayer = false;
                            validInput = true;
                        }
                        else
                        {
                            throw new FormatException("Invalid choice. Please enter 1 for Single Player or 2 for Two Players.");
                        }
                    }
                    else
                    {
                        throw new FormatException("Invalid input. Please enter a number.");
                    }
                }

                ConnectFourGame game = new ConnectFourGame(singlePlayer);
                game.Play();

                bool flag;
                do
                {
                    Console.Write("Play Again? (y/n): ");
                    try
                    {
                        string answer = Console.ReadLine()?.ToLower();
                        if (answer == null)
                        {
                            throw new ArgumentNullException("Input cannot be null.");
                        }
                        if (answer == "y")
                        {
                            playAgain = true;
                            flag = false;
                        }
                        else if (answer == "n")
                        {
                            playAgain = false;
                            flag = false;
                        }
                        else
                        {
                            throw new FormatException("Invalid input. Please enter 'y' or 'n'.");
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Input error: {ex.Message}");
                        flag = true;
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine($"Input error: {ex.Message}");
                        flag = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        flag = true;
                    }
                } while (flag);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}



class Board
{
    private char[,] grid;
    private const int Rows = 6;
    private const int Columns = 7;

    public Board()
    {
        grid = new char[Rows, Columns];
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                grid[row, col] = ' ';
            }
        }
    }

    public bool PlacePiece(int column, char symbol)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (grid[row, column] == ' ')
            {
                grid[row, column] = symbol;
                return true;
            }
        }
        return false;
    }

    public bool CheckForWin(char symbol)
    {
        // Check horizontal, vertical and diagonal win conditions
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (grid[row, col] == symbol)
                {
                    if (CheckDirection(row, col, 1, 0, symbol) ||
                        CheckDirection(row, col, 0, 1, symbol) ||
                        CheckDirection(row, col, 1, 1, symbol) ||
                        CheckDirection(row, col, 1, -1, symbol))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckDirection(int startRow, int startCol, int rowDir, int colDir, char symbol)
    {
        int count = 0;
        int row = startRow;
        int col = startCol;
        while (row >= 0 && row < Rows && col >= 0 && col < Columns && grid[row, col] == symbol)
        {
            count++;
            if (count == 4)
            {
                return true;
            }
            row += rowDir;
            col += colDir;
        }
        return false;
    }

    public bool IsFull()
    {
        for (int col = 0; col < Columns; col++)
        {
            if (grid[0, col] == ' ')
            {
                return false;
            }
        }
        return true;
    }

    public void Display()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Console.Write("| " + grid[row, col]);
            }
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('-', Columns * 3));
    }
}

