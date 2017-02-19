using System;
using static SudokuSolver;

namespace Sudoku
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var grid1 = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
            Console.WriteLine("grid1");
            Console.WriteLine(prettyprint(grid1));
            Console.WriteLine(prettyprint(solve(grid1)));

            var grid2 = "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......";
            Console.WriteLine("grid2");
            Console.WriteLine(prettyprint(grid2));
            Console.WriteLine(prettyprint(solve(grid2)));

            var hard = ".....6....59.....82....8....45........3........6..3.54...325..6..................";
            Console.WriteLine("hard");
            Console.WriteLine(prettyprint(hard));
            Console.WriteLine(prettyprint(solve(hard)));
        }
    }
}
