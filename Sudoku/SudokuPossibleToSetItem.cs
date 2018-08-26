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
        public int totalNumberOfItemsPossibleToSetWithoutCausingConflict;

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

            totalNumberOfItemsPossibleToSetWithoutCausingConflict = 0;

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

                    if (!sudokuBoard.ItemIsSet(i, j))
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

                                totalNumberOfItemsPossibleToSetWithoutCausingConflict++;
                            }
                        }
                    }
                }
            }

            CheckProcess();
        }

        private void CheckProcess()
        {
            int squareIndex, squareSequenceIndex, a, b, c;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    squareIndex = (3 * (i / 3)) + (j / 3);
                    squareSequenceIndex = (3 * (i % 3)) + (j % 3);

                    if (numberOfPossibleItemsRows[i, j] != numberOfPossibleItemsColumns[i, j])
                    {
                        throw new Exception("Exception in CheckProcess: (numberOfPossibleItemsRows[i, j] != numberOfPossibleItemsColumns[i, j])");
                    }
                    else
                    {
                        if (numberOfPossibleItemsRows[i, j] != numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex])
                        {
                            throw new Exception("Exception in CheckProcess: (numberOfPossibleItemsRows[i, j] != numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex])");
                        }
                    }

                    if (numberOfPossibleItemsRows[i, j] > 0)
                    {
                        for(int k = 0; k < numberOfPossibleItemsRows[i, j]; k++)
                        {
                            a = rows[i, j, k];
                            b = columns[j, i, k];
                            c = squares[squareIndex, squareSequenceIndex, k];

                            if (a != b)
                            {
                                throw new Exception("Exception in CheckProcess: (a != b)");
                            }
                            else
                            {
                                if (a != c)
                                {
                                    throw new Exception("Exception in CheckProcess: (a != c)");
                                }
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("SudokuPossibleToSetItem:\r\n\r\n");

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (numberOfPossibleItemsColumns[i, j] == -1)
                    {
                        sb.Append("Item already set\r\n");
                    }
                    else
                    {
                        sb.Append(string.Format("[{0}, {1}]: ", i + 1, j + 1));
                        sb.Append("{");

                        for (int k = 0; k < numberOfPossibleItemsColumns[i, j]; k++)
                        {
                            if (k == 0)
                            {
                                sb.Append(rows[i, j, k].ToString());
                            }
                            else
                            {
                                sb.Append(", " + rows[i, j, k].ToString());
                            }

                            if (k == (numberOfPossibleItemsColumns[i, j] - 1))
                            {
                                sb.Append("}\r\n");
                            }
                        }
                    }
                }

                if (i < 8)
                {
                    sb.Append("\r\n");
                }
            }

            return sb.ToString();
        }
    }
}
