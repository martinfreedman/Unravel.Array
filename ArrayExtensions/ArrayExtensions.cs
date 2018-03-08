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
        void Deconstruct(out T v, out int r, out int c);
    }
     
 
    public static class Array
    {
      
        //.................................. Cells.. ...................................................
        public static IEnumerable<T> EnumerateCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateCells(true,rowSkip, rowTake, colSkip, colTake);
        }

        public static IEnumerable<T> TransposeCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateCells(false, rowSkip, rowTake, colSkip, colTake);
        }

        public static IEnumerable<ICell<T>> IndexedCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateIndexedCells(true, rowSkip, rowTake, colSkip, colTake);
        }

        public static IEnumerable<ICell<T>> IndexedTransposeCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateIndexedCells(false, rowSkip, rowTake, colSkip, colTake);
        }

        internal static IEnumerable<T> IterateCells<T>(this T[,] matrix, bool isRowMajor, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            return isRowMajor ? rows() : cols();

            IEnumerable<T> rows() => matrix.CellByRowIterator(rowSkip, rowTake, colSkip, colTake);
            IEnumerable<T> cols() => matrix.CellByColIterator(rowSkip, rowTake, colSkip, colTake);
        }

        static internal IEnumerable<ICell<T>> IterateIndexedCells<T>(this T[,] matrix, bool isRowMajor, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            return isRowMajor ? rows() : cols();

            IEnumerable<ICell<T>> cols() => matrix.IndexedCellByColIterator(rowSkip, rowTake, colSkip, colTake);
            IEnumerable<ICell<T>> rows() => matrix.IndexedCellByRowIterator(rowSkip, rowTake, colSkip, colTake);
        }

        //.................................. Rows ......................................................

        static public IEnumerable<IGrouping<int, ICell<T>>> GroupedRows<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return _(); IEnumerable<IGrouping<int, ICell<T>>> _()
            {
                for (var row = rowSkip; row < rowTake; row++)
                {
                    yield return new Grouping<int, ICell<T>> { Key = row, Values = matrix.IndexedColIterator(row, colSkip, colTake) };
                }
            }
        }

        static public IEnumerable<IEnumerable<ICell<T>>> IndexedRows<T>(this T[,] matrix, int rowSkip=0, int rowTake=0, int colSkip=0, int colTake=0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip,rowTake,colSkip,colTake);

            return _(); IEnumerable<IEnumerable<ICell<T>>> _()
            {
                for (var row = rowSkip; row < rowTake; row++)
                {
                    yield return matrix.IndexedColIterator(row, colSkip, colTake);
                }
            }
        }

        static public IEnumerable<IEnumerable<T>> EnumerateRows<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return _(); IEnumerable<IEnumerable<T>> _()
            {
                for (var row =rowSkip; row <  rowTake; row++)
                {
                    yield return matrix.ColIterator(row, colSkip, colTake);
                }
            }
        }

        //.................................. Cols ......................................................

        static public IEnumerable<IEnumerable<T>> EnumerateCols<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return _(); IEnumerable<IEnumerable<T>> _()
            {
                for (var col = colSkip; col <  colTake; col++)
                {
                    yield return matrix.RowIterator(col, rowSkip ,rowTake);
                }
            }
        }

        static public IEnumerable<IEnumerable<ICell<T>>> IndexedCols<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return _(); IEnumerable<IEnumerable<ICell<T>>> _()
            {
                for (var col = colSkip; col <  colTake; col++)
                {
                    yield return matrix.IndexedRowIterator(col, rowSkip, rowTake);
                }
            }
        }

        static public IEnumerable<IGrouping<int,ICell<T>>> GroupedCols<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return _(); IEnumerable<IGrouping<int,ICell<T>>> _()
            {
                for (var col = colSkip; col <  colTake; col++)
                {
                    yield return new Grouping<int, ICell<T>> { Key = col, Values = matrix.IndexedRowIterator(col, rowSkip, rowTake) };
                }
            }
        }

        //................................... Iterators ................................................

        internal static IEnumerable<T>CellByColIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var j = startCol; j<  lengthCol; j++)
                for (var i = startRow; i <  lengthRow; i++)
                    yield return matrix[i, j];
        }

        internal static IEnumerable<T> CellByRowIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var i = startRow; i <  lengthRow; i++)
                for (var j = startCol; j <  lengthCol; j++)
                    yield return matrix[i, j];
        }

        internal static IEnumerable<ICell<T>> IndexedCellByColIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var j = startCol; j <  lengthCol; j++)
                for (var i = startRow; i <  lengthRow; i++)
                    yield return new Cell<T> { V = matrix[i, j], R = i, C = j } ;
        }

        internal static IEnumerable<ICell<T>> IndexedCellByRowIterator<T>(this T[,] matrix, int startRow, int lengthRow, int startCol, int lengthCol)
        {
            for (var i = startRow; i <  lengthRow; i++)
                for (var j = startCol; j <  lengthCol; j++)
                    yield return new Cell<T> { V = matrix[i, j], R = i, C = j };
        }

        internal static IEnumerable<T> RowIterator<T>(this T[,] matrix, int col, int skip, int take)
        {
            for (var i = skip; i < take; i++)
                yield return matrix[i,col];
        }

        internal static IEnumerable<ICell<T>> IndexedRowIterator<T>(this T[,] matrix, int col, int skip, int take)
        {
            for (var i = skip; i < take; i++)
                yield return new Cell<T> {V = matrix[i,col],  R = i, C= col };
        }

        internal static IEnumerable<T> ColIterator<T>(this T[,] matrix, int row, int skip, int take)
        {
            for (var j = skip; j < take; j++)
                yield return matrix[row, j];
        }

        internal static IEnumerable<ICell<T>> IndexedColIterator<T>(this T[,] matrix, int row, int skip, int take)
        {
            for (var j = skip; j < take; j++)
                yield return new Cell<T> { V = matrix[row,j], R= row, C = j} ;
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

        public void Deconstruct(out T v, out int r, out int c)
        {
            v = V;
            r = R;
            c = C;
        }
    }

    internal class Grouping<K, T> : IGrouping<K, T>
    {
        public K Key { get; internal set; }

        public IEnumerable<T> Values { get; internal set; }

        public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
    }
}
