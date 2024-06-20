abstract class Player
{
    public char Symbol { get; private set; }
    public string Name { get; private set; }

    protected Player(char symbol, string name)
    {
        Symbol = symbol;
        Name = name;
    }

   
}
class HumanPlayer
{

}
class ConnectFourGame
{
    private Board board;
    private Player[] player;
    private int currentPlayerIndex;

    static void Main(string[] args)
    {
        
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
}