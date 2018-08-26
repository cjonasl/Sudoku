using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SolveSudoku
    {
        private string _debugDirectory;
        private int _maxNumberOfTries;
        private SudokuBoard _sudokuBoard;

        public SolveSudoku(string debugDirectory, int maxNumberOfTries, SudokuBoard sudokuBoard, ArrayList originalData)
        {
            _debugDirectory = debugDirectory;
            _maxNumberOfTries = maxNumberOfTries;
            _sudokuBoard = sudokuBoard;
        } 
    }
}
