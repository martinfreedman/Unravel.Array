﻿using FsCheck;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static System.Linq.Enumerable;

namespace Unravel.Array
{
    public class Tests
    {
        static int[,] _data;
        static IEnumerable<IEnumerable<int>> _array;
        static IEnumerable<int> _flat;

        IEnumerable<T> ToFlat<T>(IEnumerable<IEnumerable<T>> a) => a.SelectMany(x => x);
        IEnumerable<IEnumerable<T>> ToJaggedColMajor<T>(T[,] r) => Range(0, r.GetLength(1)).Select(i => Range(0, r.GetLength(0)).Select(j => r[j, i]));
        IEnumerable<IEnumerable<T>> ToJaggedRowMajor<T>(T[,] r) => Range(0, r.GetLength(0)).Select(i => Range(0, r.GetLength(1)).Select(j => r[i, j]));
        IEnumerable<IEnumerable<ICell<T>>> ToJaggedRowMajorIdx<T>(T[,] r) =>
            Range(0, r.GetLength(1)).Select(i => Range(0, r.GetLength(0)).Select(j => new Cell<T> { Val = r[j, i], Y = j, X = i }).Cast<ICell<T>>());
        IEnumerable<IEnumerable<ICell<T>>> ToJaggedColMajorIdx<T>(T[,] r) =>
            Range(0, r.GetLength(0)).Select(i => Range(0, r.GetLength(1)).Select(j => new Cell<T> { Val = r[i, j], Y = i, X = j }).Cast<ICell<T>>());

        public Tests(ITestOutputHelper testOutputHelper)
        {
            _data = new int[3, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };
            _array = ToJaggedColMajor(_data); 
            _flat = ToFlat(_array);
            _TestOutputHelper = testOutputHelper;
        }

        private readonly ITestOutputHelper _TestOutputHelper;
        //..................................... Cells..........................................
        [Fact]
        public void EnumerateCellsDataGuards()
        {
            TestMatrixGuards<int?>((x) => x.EnumerateCells());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void EnumerateCellsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.EnumerateCells(v, x, y, z));
        }

        [Fact]
        public void EnumerateCellsExample()
        {
            var sut = _data;
            var expected = ToFlat(ToJaggedColMajor(sut));

            var actual = sut.EnumerateCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Theory]
        [MemberData(nameof(EmptyData))]
        public void EnumerateCellsEmpty(int[,]d)
        {
            var emptySut = d;
            var expected = ToFlat(ToJaggedColMajor(emptySut));

            var actual = d.EnumerateCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property]
        public Property EnumerateCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajor(sut));

            return (expected.TrySequenceEqual(sut.EnumerateCells())).ToProperty();
        }

        [Fact]
        public void EnumerateCellsSliceExample()
        {
            var sut = _data;
            var expected = sut[1,1];

            var actual = sut.EnumerateCells(0,2,1,2);

            Assert.Single(actual);
            Assert.Equal(expected, actual.First());
        }

        [Property]
        public Property EnumerateCellsSlice(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajor(sut));

            return (expected.TrySequenceEqual(sut.EnumerateCells())).ToProperty();
        }

        [Fact]
        public void TransposeCellsDataGuards()
        {
            TestMatrixGuards<int?>((x) => x.TransposeCells());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void TransposeCellsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.TransposeCells(v, x, y, z));
        }

        [Fact]
        public void TransposeCellsExample()
        {
            var sut = _data;
            var expected = ToFlat(ToJaggedRowMajor(sut));

            var actual = sut.TransposeCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property]
        public Property TransposeCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajor(sut));

            return (expected.TrySequenceEqual(sut.TransposeCells())).ToProperty();
        }

        [Fact]
        public void IndexedCellsDataGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedCells());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void IndexedCellsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.IndexedCells(v, x, y, z));
        }

        [Fact]
        public void IndexedCellsExample()
        {
            var sut = _data;
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            var actual = sut.IndexedCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property]
        public Property IndexedCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            return (expected.TrySequenceEqual(sut.IndexedCells())).ToProperty();
        }

        [Fact]
        public void IndexedTransposeCellsDataGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedTransposeCells());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void IndexedTransposeCellsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.IndexedTransposeCells(v, x, y, z));
        }

        [Fact]
        public void IndexedTransposeCellsExample()
        {
            var sut = _data;
            var expected = ToFlat(ToJaggedColMajorIdx(sut));

            var actual = sut.IndexedTransposeCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property]
        public Property IndexedTransposeCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajorIdx(sut));

            return (expected.TrySequenceEqual(sut.IndexedTransposeCells())).ToProperty();
        }

        //................................. Rows .....................................

        [Fact]
        public void EnumerateRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.EnumerateRows());
        }
  
        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void EnumerateRowsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.EnumerateRows(y, z));
        }

        [Fact]
        public void EnumerateRowsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajor(sut);

            var actual = sut.EnumerateRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));
        }

        [Fact]
        public void EnumerateRowsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedColMajor(sut);

            var actual = sut.EnumerateRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property(Verbose =true)]
        public Property EnumerateRows(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajor(sut));

            var actual = ToFlat(sut.EnumerateRows());

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void IndexedRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedRows());
        }

        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void IndexedRowsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.IndexedRows(y, z));
        }

        [Fact]
        public void IndexedRowsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajorIdx(sut);

            var actual = sut.IndexedRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));
        }

        [Fact]
        public void IndexedRowsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedRowMajorIdx(sut);

            var actual = sut.IndexedRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property(Skip = "to debug test")]
        public Property IndexedRows(int[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            var actual = ToFlat(sut.IndexedRows());

            return (expected.TrySequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void GroupedRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.GroupedRows());
        }

        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void GroupedRowsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.GroupedRows(y, z));
        }

        [Fact]
        public void GroupedRowsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajorIdx(sut);

            var actual = sut.GroupedRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));

            var grp = expected.Select((g,i) => (i,g.Sum(v=>v.Val)));
            Assert.True(grp.SequenceEqual(actual.Select(g => (g.Key, g.Sum(c=>c.Val)))));
        }

        [Fact]
        public void GroupedRowsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedRowMajorIdx(sut);

            var actual = sut.GroupedRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property(Skip = "to debug test")]
        public Property GroupedRows(int[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            var actual = ToFlat(sut.GroupedRows());

            return (expected.TrySequenceEqual(actual)).ToProperty();
        }
        // .......................................... Cols .....................................

        [Fact]
        public void EnumerateColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.EnumerateCols());
        }

        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void EnumerateColsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.EnumerateCols(y, z));
        }

        [Fact]
        public void EnumerateColsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajor(sut);

            var actual = sut.EnumerateCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));
        }

        [Fact]
        public void EnumerateColsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedRowMajor(sut);

            var actual = sut.EnumerateCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property]
        public Property EnumerateCols(int[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajor(sut));

            var actual = ToFlat(sut.EnumerateCols());

            return (expected.TrySequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void IndexedColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedCols());
        }

        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void IndexedColsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.IndexedCols(y, z));
        }

        [Fact]
        public void IndexedColsExample()
        {
            var sut = _data;
            var expected = ToJaggedColMajorIdx(sut);

            var actual = sut.IndexedCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));
        }

        [Fact]
        public void IndexedColsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedColMajorIdx(sut);

            var actual = sut.IndexedCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property(Skip = "to debug test")]
        public Property IndexedCols(int[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajorIdx(sut)).ToList();

            var actual = ToFlat(sut.IndexedCols().ToList());

            return (expected.TrySequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void GroupedColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.GroupedCols());
        }

        [Theory]
        [MemberData(nameof(Get1DRanges))]
        public void GroupedColsRangeGuards(int?[,] d, int axis, int start, int length)
        {
            TestRange(d, axis, start, length, (x, y, z) => x.GroupedCols(y, z));
        }

        [Fact]
        public void GroupedColsExample()
        {
            var sut = _data;
            var expected = ToJaggedColMajorIdx(sut);

            var actual = sut.GroupedCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().TrySequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().TrySequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().TrySequenceEqual(actual.Skip(2).First()));

            var grp = expected.Select((g, i) => (i, g.Sum(v => v.Val)));
            Assert.True(grp.TrySequenceEqual(actual.Select(g => (g.Key, g.Sum(c => c.Val)))));
        }

        [Fact]
        public void GroupedColsEmpty()
        {
            var sut = new int?[0, 0];
            var expected = ToJaggedColMajorIdx(sut);

            var actual = sut.GroupedCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.TrySequenceEqual(actual));
        }

        [Property(Skip = "to debug test")]
        public Property GroupedCols(int[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajorIdx(sut)).ToList();

            var actual = ToFlat(sut.GroupedCols().ToList());

            return (expected.TrySequenceEqual(actual)).ToProperty();
        }

        //............................ Helpers ...................................................

        static void TestRange<T>(T[,] matrix, int axis, int start, int length, Func<T[,], int, int, dynamic> fn)
        {
            var ex = Record.Exception(() => fn(matrix, start, length));

            if (ex != null)
                Assert.IsType<ArgumentOutOfRangeException>(ex);

            if (start < 0 || start > matrix.GetUpperBound(axis))
                Assert.Contains("start", ex.Message);
            else if (length < 0 || start + length > matrix.GetLength(axis))
                Assert.Contains("length", ex.Message);
        }

        static void TestRange<T>(T[,] matrix, int axis, int startRow, int lengthRow, int startCol, int lengthCol, Func<T[,], int, int, int, int, dynamic> fn)
        {
            var ex = Record.Exception(() => fn(matrix, startRow, lengthRow, startCol, lengthCol));

            if (ex != null)
                Assert.IsType<ArgumentOutOfRangeException>(ex);

            if ((startRow < 0 || startRow > matrix.GetUpperBound(0))
                || startCol < 0 || startCol > matrix.GetUpperBound(1))
                Assert.Contains("start", ex.Message);
            else if ((lengthRow < 0 || startRow + lengthRow > matrix.GetLength(axis))
                     || (lengthRow < 0 || startRow + lengthRow > matrix.GetLength(axis)))
                Assert.Contains("length", ex.Message);
        }

        static void TestMatrixGuards<T>(Func<T[,], dynamic> fun)
        {
            T[,] nullSut = null;
            T[,] emptySut = new T[0, 0];

            var nullEx = Record.Exception(() => fun(nullSut));
            var emptyEx = Record.Exception(() => fun(emptySut));

            Assert.IsType<ArgumentNullException>(nullEx);
            Assert.Contains("matrix", nullEx.Message);
            Assert.Null(emptyEx);
        }

        public static IEnumerable<object[]> Get1DRanges()
        {
            var data = new int?[3, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };

            yield return new object[] { data, 0, 0, 3 };
            yield return new object[] { data, 0, -1, 3 };
            yield return new object[] { data, 0, 3, 3 };
            yield return new object[] { data, 0, 0, -1 };
            yield return new object[] { data, 0, 0, 4 };
        }

        public static IEnumerable<object[]> Get2DRanges()
        {
            var data = new int?[3, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };

            yield return new object[] { data, 0, 0, 3, 0 ,3 };
            yield return new object[] { data, 0, -1, 3, 0, 3 };
            yield return new object[] { data, 0, 3, 3,0 ,3 };
            yield return new object[] { data, 0, 0, -1, 0 ,3 };
            yield return new object[] { data, 0, 0, 4, 0 , 3 };
            yield return new object[] { data, 0, 0, 3, -1, 3, };
            yield return new object[] { data, 0, 0,3, 3, 3, };
            yield return new object[] { data, 0, 0,3, 0, -1, };
            yield return new object[] { data, 0, 0,3, 0, 4, };
        }

        public static IEnumerable<object[]> EmptyData()
        {
            yield return new object[] { new int[0, 0] };
            yield return new object[] { new int[1, 0] };
            yield return new object[] { new int[0, 1] };
        }
    }

    internal static class Helpers
    {
        public static bool TrySequenceEqual<T>(this IEnumerable<IEnumerable<T>> expected, IEnumerable<IEnumerable<T>> actual)
        {
            if (expected.Any() == actual.Any())
            {
                if (expected.Any())
                {
                    using (var eIter = expected.GetEnumerator())
                    using (var aIter = actual.GetEnumerator())
                    {
                        while (eIter.MoveNext() && aIter.MoveNext())
                        {
                            if (!eIter.Current.SequenceEqual(eIter.Current))
                                return false;
                        }
                    }
                    return true;
                }
                else
                    return true;
            }
            return false;
        }

        public static bool TrySequenceEqual<T>(this IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected.Any() == actual.Any())
            {
                if (expected.Any())
                    return expected.SequenceEqual(actual);
                else
                    return true;
            }
            return false;
        }
    }
}