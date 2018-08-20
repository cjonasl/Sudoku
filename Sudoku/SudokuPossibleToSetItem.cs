using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    /// <summary>
    /// Item is one of 1, 2, 3, 4, 5, 6, 7, 8, 9
    /// </summary>
    public class SudokuPossibleToSetItem
    {
        public int[,,] rows, columns, squares;
        public int[,] numberOfPossibleItemsRows, numberOfPossibleItemsColumns, numberOfPossibleItemsSquares;
        public int totalNumberOfItemsPossibleToSet;

        public SudokuPossibleToSetItem()
        {
            rows = new int[9, 9, 9];
            columns = new int[9, 9, 9];
            squares = new int[9, 9, 9];
            numberOfPossibleItemsRows = new int[9, 9];
            numberOfPossibleItemsColumns = new int[9, 9];
            numberOfPossibleItemsSquares = new int[9, 9];
        }

        public void Process(SudokuBoard sudokuBoard)
        {
            int i, j, k, squareIndex, squareSequenceIndex;

            totalNumberOfItemsPossibleToSet = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    numberOfPossibleItemsRows[i, j] = -1;
                    numberOfPossibleItemsColumns[i, j] = -1;
                    numberOfPossibleItemsSquares[i, j] = -1;
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    squareIndex = (3 * (i / 3)) + (j / 3);
                    squareSequenceIndex = (3 * (i % 3)) + (j % 3);

                    if (!sudokuBoard.NumberIsSet(i, j))
                    {
                        numberOfPossibleItemsRows[i, j] = 0;
                        numberOfPossibleItemsColumns[j, i] = 0;
                        numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex] = 0;

                        for (k = 1; k <= 9; k++)
                        {
                            if (sudokuBoard.CanSetNumber(i, j, k))
                            {
                                rows[i, j, numberOfPossibleItemsRows[i, j]] = k;
                                numberOfPossibleItemsRows[i, j]++;

                                columns[j, i, numberOfPossibleItemsColumns[j, i]] = k;
                                numberOfPossibleItemsColumns[j, i]++;

                                squares[squareIndex, squareSequenceIndex, numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex]] = k;
                                numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex]++;

                                totalNumberOfItemsPossibleToSet++;
                            }
                        }
                    }
                }
            }
        }
    }
}
