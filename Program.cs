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
class ConnectFourGame
{
    private Board board;
    private Player[] players;
    private int currentPlayerIndex;

    public ConnectFourGame()
    {
        board = new Board();
        players = new Player[2];

        for (int i = 0; i < 2; i++)
        {
            Console.Write($"Enter name for Player {i + 1} (Symbol {(i == 0 ? 'X' : 'O')}): ");
            string name = Console.ReadLine();
            players[i] = new HumanPlayer(i == 0 ? 'X' : 'O', name);
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
            ConnectFourGame game = new ConnectFourGame();
            game.Play();

            Console.Write("Do you want to play again? (y/n): ");
            string response = Console.ReadLine().ToLower();
            playAgain = (response == "y");
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