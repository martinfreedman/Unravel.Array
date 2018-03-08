#region License and Terms
// Unravel.Array - Enumerable extensions to regular two dimensional arrays
// Copyright (c) 2018 Martin Freedman. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]


namespace Unravel.Array
{
    /// <summary>
    /// Returns a deconstructable struct of value (V) with row (r) and column (c) indices
    /// </summary>
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

        /// <summary>
        /// Returns a sequence cells in the matrix
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence cells in the matrix</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
        public static IEnumerable<T> EnumerateCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateCells(true,rowSkip, rowTake, colSkip, colTake);
        }

        /// <summary>
        /// Returns a a sequence of transposed cells in the matrix
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// Row and column skip/take parameters refer to source not transposed matrix
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a a sequence of transposed cells in the matrix</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
        public static IEnumerable<T> TransposeCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateCells(false, rowSkip, rowTake, colSkip, colTake);
        }

        /// <summary>
        /// Returns an sequence of indexed cells in the matrix
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns an sequence of indexed cells in the matrix</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
        public static IEnumerable<ICell<T>> IndexedCells<T>(this T[,] matrix, int rowSkip = 0, int rowTake = 0, int colSkip = 0, int colTake = 0)
        {
            matrix.ThrowIfNull(nameof(matrix));
            ((rowSkip, rowTake), (colSkip, colTake)) = matrix.SetSliceOrThrow(rowSkip, rowTake, colSkip, colTake);

            return matrix.IterateIndexedCells(true, rowSkip, rowTake, colSkip, colTake);
        }

        /// <summary>
        /// Returns a sequence of transposed indexed cells in the matrix
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// Row and column skip/take parameters refer to source not transposed matrix
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of transposed indexed cells in the matrix</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of row sequences, keyed by row index and with 2D indexed cells, from the matrix 
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of row sequences, keyed by row index and with 2D indexed cells</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of row sequences with 2D indexed cells, from the matrix 
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of row sequences with 2D indexed cells</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of row sequences from the matrix 
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of row sequences </returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of column sequences from the matrix 
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of column sequences </returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of column sequences with 2D indexed cells, from the matrix 
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of column sequences with 2D indexed cells</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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

        /// <summary>
        /// Returns a sequence of column sequences, keyed by column index and with 2D indexed cells, from the matrix 
        /// , with optional slicing in skip/take format for either rows, columns or both.
        /// </summary>
        /// <typeparam name="T">The type of the cells <paramref name="matrix"/></typeparam>
        /// <param name="matrix">The regular 2D array to turn into a sequence of row sequences</param>
        /// <param name="rowSkip">How many rows to skip (0 based), leave at zero for all rows</param>
        /// <param name="rowTake">How many rows to take after skipped rows, leave at zero for all rows </param>
        /// <param name="colSkip">How many columns to skip (0 based), leave at zero for all columns</param>
        /// <param name="colTake">How many columns to take after skipped columns, leave at zero for all columns</param>
        /// <returns>Returns a sequence of column sequences, keyed by row index and with 2D indexed cells</returns>
        /// <remarks>This uses eager argument evaluation but deferred execution to stream its results.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When any of the skip or take (including skip) parameters are outside the range of the matrix</exception>///
        /// <exception cref="ArgumentNullException">When the matrix suppplied is null</exception>///
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
