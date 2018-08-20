using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class CollectionSudokuPossibleHolder
    {
        private int _numberOfPossibleItems;
        private ArrayList _sudokuPossibleHolders;

        public CollectionSudokuPossibleHolder(int numberOfPossibleItems)
        {
            _numberOfPossibleItems = numberOfPossibleItems;
            _sudokuPossibleHolders = new ArrayList();
        }

        public void Add(int row, int column, int[] possibleItems)
        {
            _sudokuPossibleHolders.Add(new SudokuPossibleHolder(row, column, _numberOfPossibleItems, possibleItems));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0}:", _numberOfPossibleItems.ToString()));

            for (int i = 0; i < _sudokuPossibleHolders.Count; i++)
            {
                sb.Append("  " + ((SudokuPossibleHolder)_sudokuPossibleHolders[i]).ToString());
            }

            return sb.ToString().TrimEnd();
        }

        public int ReturnItem(Random r)
        {
            int n = r.Next(_sudokuPossibleHolders.Count);
            return ((SudokuPossibleHolder)_sudokuPossibleHolders[n]).ReturnItem(r);
        }

        public void Reset()
        {
            _sudokuPossibleHolders.Clear();
        }
    }
}
