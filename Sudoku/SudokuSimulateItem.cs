using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace Sudoku
{
    public class SudokuSimulateItem
    {
        private CollectionSudokuPossibleHolder[] _collectionSudokuPossibleHolder;

        private int _numberOfCallsToReturnItem;

        public SudokuSimulateItem()
        {
            _collectionSudokuPossibleHolder = new CollectionSudokuPossibleHolder[8];
            _numberOfCallsToReturnItem = 0;

            for (int i = 0; i < 8; i++)
            {
                _collectionSudokuPossibleHolder[i] = new CollectionSudokuPossibleHolder(2 + i);
            }
        }

        private int[] ReturnIntArray(int[,,] u, int i, int j)
        {
            int[] v = new int[9];

            for (int k = 0; k < 9; k++)
            {
                v[k] = u[i, j, k];
            }

            return v;
        }

        public ThreeTupleOfIntegers ReturnItem(Random r, SudokuPossibleToSetItem sudokuPossibleToSetItem, out string debugString)
        {
            int minNumberOfPossibleItemsToSet = 9;
            StringBuilder sb;
            ThreeTupleOfIntegers item;

            _numberOfCallsToReturnItem++;

            for (int i = 0; i < 8; i++)
            {
                _collectionSudokuPossibleHolder[i].Reset();
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] != -1)
                    {
                        if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] < minNumberOfPossibleItemsToSet)
                        {
                            minNumberOfPossibleItemsToSet = sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j];
                        }

                        if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] >= 2)
                        {
                            _collectionSudokuPossibleHolder[sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] - 2].Add(i, j, ReturnIntArray(sudokuPossibleToSetItem.rows, i, j));
                        }
                    }
                }
            }

            item = _collectionSudokuPossibleHolder[minNumberOfPossibleItemsToSet - 2].ReturnItem(r);

            sb = new StringBuilder(string.Format("Simulate item {0}\r\n\r\n", item.ToString()));

            for (int i = 0; i < 8; i++)
            {
                sb.Append(_collectionSudokuPossibleHolder[i].ToString() + "\r\n");
            }

            debugString = sb.ToString().TrimEnd();

            return item;
        }
    }
}
