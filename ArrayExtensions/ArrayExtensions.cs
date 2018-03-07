using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Unravel.Array
{
    public interface ICell<T>
    {
        T V { get; }
        int R { get; }
        int C { get; }
    }
     
 
    public static class Array
    {
        public enum MajorAxis { ByRows, ByCols };

        const int _row = 0;
        const int _col = 1;

        static readonly string[] _axes = new string[] { "row", "col" };
        //.................................. Cells.. ...................................................
        public static IEnumerable<T> EnumerateCells<T>(this T[,] matrix)
        {
            matrix.ThrowIfNull(nameof(matrix));

            var rowStart = matrix.GetLowerBound(0);
            var rowLength = matrix.GetLength(0);
            var colStart = matrix.GetLowerBound(1);
            var colLength = matrix.GetLength(1);

            return matrix.IterateCells(true,rowStart, rowLength, colStart, colLength);
        }

        public static IEnumerable<T> EnumerateCells<T>(this T[,] matrix, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));

            return matrix.IterateCells(true, rowStart, rowLength, colStart, colLength);
        }

        public static IEnumerable<T> TransposeCells<T>(this T[,] matrix)
        {
            matrix.ThrowIfNull(nameof(matrix));

            var rowStart = matrix.GetLowerBound(0);
            var rowLength = matrix.GetLength(0);
            var colStart = matrix.GetLowerBound(1);
            var colLength = matrix.GetLength(1);

            return matrix.IterateCells(false, rowStart, rowLength, colStart, colLength);
        }

        public static IEnumerable<T> TransposeCells<T>(this T[,] matrix, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));

            return matrix.IterateCells(false, rowStart, rowLength, colStart, colLength);
        }

        static public IEnumerable<ICell<T>> IndexedCells<T>(this T[,] matrix)
        {
            matrix.ThrowIfNull(nameof(matrix));

            var rowStart = matrix.GetLowerBound(0);
            var rowLength = matrix.GetLength(0);
            var colStart = matrix.GetLowerBound(1);
            var colLength = matrix.GetLength(1);

            return matrix.IterateIndexedCells(true, rowStart, rowLength, colStart, colLength);
        }

        public static IEnumerable<ICell<T>> IndexedCells<T>(this T[,] matrix, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));
            return matrix.IterateIndexedCells(true, rowStart, rowLength, colStart, colLength);
        }

        public static IEnumerable<ICell<T>> IndexedTransposeCells<T>(this T[,] matrix, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));
            return matrix.IterateIndexedCells(false, rowStart, rowLength, colStart, colLength);
        }

        static public IEnumerable<ICell<T>> IndexedTransposeCells<T>(this T[,] matrix)
        {
            matrix.ThrowIfNull(nameof(matrix));

            var rowStart = matrix.GetLowerBound(0);
            var rowLength = matrix.GetLength(0);
            var colStart = matrix.GetLowerBound(1);
            var colLength = matrix.GetLength(1);

            return matrix.IterateIndexedCells(false, rowStart, rowLength, colStart, colLength);
        }

        internal static IEnumerable<T> IterateCells<T>(this T[,] matrix, bool isRowMajor, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ThrowIfOutOfRange(matrix, _col, colStart, colLength);
            ThrowIfOutOfRange(matrix, _row, rowStart, rowLength);

            return isRowMajor ? rows() : cols();

            IEnumerable<T> rows() => matrix.CellByRowIterator(rowStart, rowLength, colStart, colLength);
            IEnumerable<T> cols() => matrix.CellByColIterator(rowStart, rowLength, colStart, colLength);
        }

        static internal IEnumerable<ICell<T>> IterateIndexedCells<T>(this T[,] matrix, bool isRowMajor, int rowStart, int rowLength, int colStart, int colLength)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ThrowIfOutOfRange(matrix, _col, colStart, colLength);
            ThrowIfOutOfRange(matrix, _row, rowStart, rowLength);

            return isRowMajor ? rows() : cols();

            IEnumerable<ICell<T>> cols() => matrix.IndexedCellByColIterator(rowStart, rowLength, colStart, colLength);
            IEnumerable<ICell<T>> rows() => matrix.IndexedCellByRowIterator(rowStart, rowLength, colStart, colLength);
        }

        //.................................. Rows ......................................................

        static public IEnumerable<IGrouping<int, ICell<T>>> GroupedRows<T>(this T[,] matrix, int start = 0, int length = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            (start,length) = SetSliceOrThrow(matrix, _row, start, length);
  
            return _(); IEnumerable<IGrouping<int, ICell<T>>> _()
            {
                for (var r = matrix.GetLowerBound(_row); r < matrix.GetLength(_row); r++)
                {
                    yield return new Grouping<int, ICell<T>> { Key = r, Values = matrix.IndexedColIterator(r, start, length) };
                }
            }
        }

        static public IEnumerable<IEnumerable<ICell<T>>> IndexedRows<T>(this T[,] matrix, int rowStart = 0, int rowLength = 0, int colStart = 0, int colLength = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            if (rowLength != 0 && colLength != 0)
            {
                ThrowIfOutOfRange(matrix, _col, colStart, colLength);
                ThrowIfOutOfRange(matrix, _row, rowStart, rowLength);
            }
            else
            {
                rowStart = matrix.GetLowerBound(_row);
                rowLength = rowStart + matrix.GetLength(_row);
                colStart = matrix.GetLowerBound(_col);
                colLength = colStart + matrix.GetLength(_col);
            }

            return _(); IEnumerable<IEnumerable<ICell<T>>> _()
            {
                for (var r = rowStart; r < rowLength; r++)
                {
                    yield return matrix.IndexedColIterator(r, colStart, colLength);
                }
            }
        }

        static public IEnumerable<IEnumerable<T>> EnumerateRows<T>(this T[,] matrix, int rowStart=0, int rowLength=0, int colStart=0, int colLength=0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            if (rowLength != 0 && colLength != 0)
            {
                ThrowIfOutOfRange(matrix, _col, colStart, colLength);
                ThrowIfOutOfRange(matrix, _row, rowStart, rowLength);
            }
            else
            {
                rowStart = matrix.GetLowerBound(_row);
                rowLength = rowStart + matrix.GetLength(_row);
                colStart = matrix.GetLowerBound(_col);
                colLength = colStart + matrix.GetLength(_col);
            }

            return _(); IEnumerable<IEnumerable<T>> _()
            {
                for (var r =rowStart; r < rowLength; r++)
                {
                    yield return matrix.ColIterator(r, colStart, colLength);
                }
            }
        }

        //.................................. Cols ......................................................

        static public IEnumerable<IEnumerable<T>> EnumerateCols<T>(this T[,] matrix, int start = 0, int length = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            (start, length) = SetSliceOrThrow(matrix, _row, start, length);


            return _(); IEnumerable<IEnumerable<T>> _()
            {
                for (var c = matrix.GetLowerBound(_col); c <= matrix.GetUpperBound(_col); c++)
                {
                    yield return matrix.RowIterator(c, start ,length);
                }
            }
        }

        static public IEnumerable<IEnumerable<ICell<T>>> IndexedCols<T>(this T[,] matrix, int start = 0, int length = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            (start, length) = SetSliceOrThrow(matrix, _col, start, length);

            return _(); IEnumerable<IEnumerable<ICell<T>>> _()
            {
                for (var c = 0; c < matrix.GetLength(_col); c++)
                {
                    yield return matrix.IndexedRowIterator(c, start, length);
                }
            }
        }

        static public IEnumerable<IGrouping<int,ICell<T>>> GroupedCols<T>(this T[,] matrix, int start = 0, int length = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            (start, length) = SetSliceOrThrow(matrix, _col, start, length);

            return _(); IEnumerable<IGrouping<int,ICell<T>>> _()
            {
                for (var c = 0; c < matrix.GetLength(_col); c++)
                {
                    yield return new Grouping<int, ICell<T>> { Key = c, Values = matrix.IndexedRowIterator(c, start, length) };
                }
            }
        }

        //.................................. Row .......................................................

        //public static IEnumerable<T> EnumerateRow<T>(this T[,] matrix, int row, int start=0, int length=0)
        //{
        //    matrix.ThrowIfNull(nameof(matrix));
        //  //  (start,length) = SetSliceOrThrow(matrix, _row, row, start, length);

        //    return matrix.RowIterator(row, start, length);
        //}

        //public static IEnumerable<ICell<T>> IndexedRow<T>(this T[,] matrix, int row, int start=0, int length=0)
        //{
        //    matrix.ThrowIfNull(nameof(matrix));
        //    ThrowIfOutOfRange(matrix, _row, row, start, length);
        //    //  (start,length) = SetSliceOrThrow(matrix, _row, row, start, length);

        //    return matrix.IndexedRowIterator(row, start, length);
        //}

        ////.................................. Col .......................................................

        //public static IEnumerable<T> EnumerateCol<T>(this T[,] matrix, int col,int start=0, int length=0)
        //{
        //    matrix.ThrowIfNull(nameof(matrix));
        //    ThrowIfOutOfRange(matrix, _col, col, start, length);

        //    return matrix.ColIterator(col, start, length);
        //}

        //public static IEnumerable<ICell<T>> IndexedCol<T>(this T[,] matrix, int col,int start, int length)
        //{
        //    matrix.ThrowIfNull(nameof(matrix));
        //    ThrowIfOutOfRange(matrix, _col, col, start, length);

        //    return matrix.IndexedColIterator(col,start,length);
        //}

        //................................... Iterators ................................................


        internal static IEnumerable<T>CellByRowIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var j = startCol; j< startCol+ lengthCol; j++)
                for (var i = startRow; i < startRow+ lengthRow; i++)
                {
                    yield return matrix[i, j];
                }
        }

        internal static IEnumerable<T> CellByColIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var i = startRow; i < startRow + lengthRow; i++)
                for (var j = startCol; j < startCol + lengthCol; j++)
                {
                    yield return matrix[i, j];
                }
        }

        internal static IEnumerable<ICell<T>> IndexedCellByRowIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var j = startCol; j < startCol + lengthCol; j++)
                for (var i = startRow; i < startRow + lengthRow; i++)
                {
                    yield return new Cell<T> { V = matrix[i, j], C = i, R = j } ;
                }
        }

        internal static IEnumerable<ICell<T>> IndexedCellByColIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var i = startRow; i < startRow + lengthRow; i++)
                for (var j = startCol; j < startCol + lengthCol; j++)
                {
                    yield return new Cell<T> { V = matrix[i, j], C = i, R = j };
                }
        }

        internal static IEnumerable<T> RowIterator<T>(this T[,] matrix, int col, int start, int length)
        {
            for (var i = start; i < start + length; i++)
            {
                yield return matrix[i,col];
            }
        }

        internal static IEnumerable<ICell<T>> IndexedRowIterator<T>(this T[,] matrix, int col, int start, int end)
        {
            for (var i = start; i <  end; i++)
            {
                yield return new Cell<T> {V = matrix[i,col],  C = i, R= col };
            }
        }

        internal static IEnumerable<T> ColIterator<T>(this T[,] matrix, int row, int start, int length)
        {
            for (var j = start; j < start + length; j++)
            {
                yield return matrix[row, j];
            }
        }

        internal static IEnumerable<ICell<T>> IndexedColIterator<T>(this T[,] matrix, int row, int start, int end)
        {
            for (var j = start; j <end; j++)
            {
                yield return new Cell<T> { V = matrix[row,j], C= row, R = j} ;
            }
        }

        //.................................... Helpers ..................................................
        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int axe)
        {
            if (axe < matrix.GetLowerBound(axis) || axe > matrix.GetUpperBound(axis))
                throw new ArgumentOutOfRangeException(_axes[axe]);
        }

        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int start, int length)
        {
            if (matrix.GetLength(axis) == 0)
                return;

            if (start < 0 || start > matrix.GetUpperBound(axis))
                throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 1 || start + length > matrix.GetLength(axis))
                throw new ArgumentOutOfRangeException(nameof(length));
        }

        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int axe, int start, int length)
        {
            ThrowIfOutOfRange(matrix, axis, axe);
            ThrowIfOutOfRange(matrix, axis, start, length);
        }

        internal static (int, int) SetSliceOrThrow<T>(T[,] matrix, int axis, int start, int length)
        {
            if (length != 0) ThrowIfOutOfRange(matrix, axis, start, length);
            return length ==0 ? (start = matrix.GetLowerBound(axis), matrix.GetLength(axis)) :
                                (start, length);
        }
    }

    internal static class Guards
    {
        internal static void ThrowIfNull<T>(this T m, string message) where T:class
        {
            if (m == null) throw new ArgumentNullException(message);
        }
    }

    internal struct Cell<T> : ICell<T>
    {
        public T V { get; internal set; }

        public int R { get; internal set; }

        public int C { get; internal set; }
    }

    internal class Grouping<K, T> : IGrouping<K, T>
    {
        public K Key { get; internal set; }

        public IEnumerable<T> Values { get; internal set; }

        public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
    }
}
