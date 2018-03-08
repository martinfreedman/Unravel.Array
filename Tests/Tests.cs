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

using FsCheck;
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
        IEnumerable<IEnumerable<ICell<T>>> ToJaggedColMajorIdx<T>(T[,] r) =>
            Range(0, r.GetLength(1)).Select(i => Range(0, r.GetLength(0)).Select(j => new Cell<T> { V = r[j, i], R=j, C = i}).Cast<ICell<T>>());
        IEnumerable<IEnumerable<ICell<T>>> ToJaggedRowMajorIdx<T>(T[,] r) =>
            Range(0, r.GetLength(0)).Select(i => Range(0, r.GetLength(1)).Select(j => new Cell<T> { V = r[i, j],R=i, C = j }).Cast<ICell<T>>());

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
            var expected = ToFlat(ToJaggedRowMajor(sut));

            var actual = sut.EnumerateCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Property]
        public Property EnumerateCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajor(sut));

            return (expected.SequenceEqual(sut.EnumerateCells())).ToProperty();
        }

        [Fact]
        public void EnumerateCellsSliceExample()
        {
            /*
             *    0 1 2
             *  0 0,1,2
             *  1 3,4,5
             *  2 6,7,8
             */ 
            var sut = _data;
            var expected = new int[]{ 1, 2, 4, 5 };

            var actual = sut.EnumerateCells(0, 2, 1, 2);

            Assert.Equal(expected,actual);
        }

        [Property]
        public Property EnumerateCellsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedRowMajor(sut).Select(r => r.Skip(cs).Take(ct)).Skip(rs).Take(rt).SelectMany(x => x);

            var actual = sut.EnumerateCells(rs, rt, cs, ct);

            return (expected.SequenceEqual(actual)).ToProperty();
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
            var expected = ToFlat(ToJaggedColMajor(sut));

            var actual = sut.TransposeCells();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Property]
        public Property TransposeCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajor(sut));

            return (expected.SequenceEqual(sut.TransposeCells())).ToProperty();
        }

        [Property]
        public Property TransposeCellsSlice(int?[,] sut)
        {
            var (rs, rl, cs, cl) = Slicer(sut);

            var expected = ToJaggedColMajor(sut).Select(r => r.Skip(rs).Take(rl)).Skip(cs).Take(cl).SelectMany(x => x);

            var actual = sut.TransposeCells(rs, rl, cs, cl);

            return (expected.SequenceEqual(actual)).ToProperty();
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
            Assert.True(expected.SequenceEqual(actual));
        }

        [Property]
        public Property IndexedCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            return (expected.SequenceEqual(sut.IndexedCells())).ToProperty();
        }

        [Property]
        public Property IndexedCellsSlice(int?[,] sut)
        {
            var (rs, rl, cs, cl) = Slicer(sut);

            var expected = ToJaggedRowMajorIdx(sut).Select(r => r.Skip(cs).Take(cl)).Skip(rs).Take(rl).SelectMany(x => x);

            var actual = sut.IndexedCells(rs, rl, cs, cl);

            return (expected.SequenceEqual(actual)).ToProperty();
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

        [Property]
        public Property IndexedTransposeCells(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajorIdx(sut));

            return (expected.SequenceEqual(sut.IndexedTransposeCells())).ToProperty();
        }

        [Property]
        public Property IndexedTransposeCellsSlice(int?[,] sut)
        {
            var (rs, rl, cs, cl) = Slicer(sut);

            var expected = ToJaggedColMajorIdx(sut).Select(r => r.Skip(rs).Take(rl)).Skip(cs).Take(cl).SelectMany(x => x);

            var actual = sut.IndexedTransposeCells(rs, rl, cs, cl);

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        //................................. Rows .....................................

        [Fact]
        public void EnumerateRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.EnumerateRows());
        }
  
        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void EnumerateRowsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.EnumerateRows(v, x, y, z));
        }

        [Fact]
        public void EnumerateRowsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajor(sut);

            var actual = sut.EnumerateRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));
        }

        [Property]
        public Property EnumerateRows(int?[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajor(sut));

            var actual = ToFlat(sut.EnumerateRows());

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void EnumerateRowsSliceExample()
        {
            /*
             *    0 1 2
             *  0 0,1,2
             *  1 3,4,5
             *  2 6,7,8
             */
            var sut = _data;
            var expected = new int[] { 1, 2, 4, 5 };

            var actual = sut.EnumerateRows(0, 2, 1, 2).SelectMany(x=>x);

            Assert.Equal(expected, actual);
        }

        [Property]
        public Property EnumerateRowsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedRowMajor(sut).Select(r => r.Skip(cs).Take(ct)).Skip(rs).Take(rt).SelectMany(x => x);

            var actual = ToFlat(sut.EnumerateRows(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void IndexedRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedRows());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void IndexedRowsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.IndexedRows(v, x, y, z));
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
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));
        }

        [Property]
        public Property IndexedRows(int[,] sut)
        {
            var expected = ToFlat(ToJaggedRowMajorIdx(sut));

            var actual = ToFlat(sut.IndexedRows());

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Property]
        public Property IndexedRowsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedRowMajorIdx(sut).Select(r => r.Skip(cs).Take(ct)).Skip(rs).Take(rt).SelectMany(x => x);

            var actual = ToFlat(sut.IndexedRows(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Fact]
        public void GroupedRowsGuards()
        {
            TestMatrixGuards<int?>((x) => x.GroupedRows());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void GroupedRowsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.GroupedRows(v, x, y, z));
        }

        [Fact]
        public void GroupedRowsExample()
        {
            var sut = _data;
            var expected = ToJaggedRowMajorIdx(sut);

            var actual = sut.GroupedRows();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));

            var grp = expected.Select((g, i) => (i, g.Sum(v => v.V)));
            Assert.True(grp.SequenceEqual(actual.Select(g => (g.Key, g.Sum(c=>c.V)))));
        }

        [Property]
        public Property GroupedRows(int[,] sut)
        {
            var unflat = ToJaggedRowMajorIdx(sut);
            var grp = unflat.Select((g, i) => (i, g.Sum(v => v.V)));
            var expected = ToFlat(unflat);

            var actual = sut.GroupedRows();

            var res = expected.SequenceEqual(ToFlat(actual));
            var res1= grp.SequenceEqual(actual.Select(g => (g.Key, g.Sum(c => c.V))));
            return (res && res1).ToProperty();
        }

        [Property]
        public Property GroupedRowsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedRowMajorIdx(sut).Select(r => r.Skip(cs).Take(ct)).Skip(rs).Take(rt).SelectMany(x => x);

            var actual = ToFlat(sut.GroupedRows(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }
        // .......................................... Cols .....................................

        [Fact]
        public void EnumerateColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.EnumerateCols());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void EnumerateColsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.EnumerateCols(v, x, y, z));
        }

        [Fact]
        public void EnumerateColsExample()
        {
            var sut = _data;
            var expected = ToJaggedColMajor(sut);

            var actual = sut.EnumerateCols();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(expected, actual);
            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));
        }

        [Property]
        public Property EnumerateCols(int[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajor(sut));

            var actual = ToFlat(sut.EnumerateCols());

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Property]
        public Property EnumerateColsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedColMajor(sut).Select(r => r.Skip(rs).Take(rt)).Skip(cs).Take(ct).SelectMany(x => x);

            var actual = ToFlat(sut.EnumerateCols(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }
        [Fact]
        public void IndexedColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.IndexedCols());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void IndexedColsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.IndexedCols(v, x, y, z));
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
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));
        }

        [Property]
        public Property IndexedCols(int[,] sut)
        {
            var expected = ToFlat(ToJaggedColMajorIdx(sut));

            var actual = ToFlat(sut.IndexedCols());

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        [Property]
        public Property IndexedColsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedColMajorIdx(sut).Select(r => r.Skip(rs).Take(rt)).Skip(cs).Take(ct).SelectMany(x => x);

            var actual = ToFlat(sut.IndexedCols(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }


        [Fact]
        public void GroupedColsGuards()
        {
            TestMatrixGuards<int?>((x) => x.GroupedCols());
        }

        [Theory]
        [MemberData(nameof(Get2DRanges))]
        public void GroupedColsRangeGuards(int?[,] d, int axis, int rs, int rl, int cs, int cl)
        {
            TestRange(d, axis, rs, rl, cs, cl, (u, v, x, y, z) => u.GroupedCols(v, x, y, z));
        }

        [Fact(Skip ="")]
        public void GroupedColsExample()
        {
            var sut = _data;
            var expected = ToJaggedColMajorIdx(sut);

            var actual = sut.GroupedCols();

            Assert.Equal(expected.Count(), actual.Count());
         //   Assert.Equal(expected, actual);
          //  Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.True(expected.First().SequenceEqual(actual.First()));
            Assert.True(expected.Skip(1).First().SequenceEqual(actual.Skip(1).First()));
            Assert.True(expected.Skip(2).First().SequenceEqual(actual.Skip(2).First()));

            var grp = expected.Select((g, i) => (i, g.Sum(v => v.V)));
            Assert.True(grp.SequenceEqual(actual.Select(g => (g.Key, g.Sum(c => c.V)))));
        }

        [Property]
        public Property GroupedCols(int[,] sut)
        {
            var unflat = ToJaggedColMajorIdx(sut);
            var grp = unflat.Select((g, i) => (i, g.Sum(v => v.V)));
            var expected = ToFlat(unflat);

            var actual = sut.GroupedCols();

            var res = expected.SequenceEqual(ToFlat(actual));
            var res1 = grp.SequenceEqual(actual.Select(g => (g.Key, g.Sum(c => c.V))));
            return (res && res1).ToProperty();
        }

        [Property]
        public Property GroupedColsSlice(int?[,] sut)
        {
            var (rs, rt, cs, ct) = Slicer(sut);

            var expected = ToJaggedColMajorIdx(sut).Select(r => r.Skip(rs).Take(rt)).Skip(cs).Take(ct).SelectMany(x => x);

            var actual = ToFlat(sut.GroupedCols(rs, rt, cs, ct));

            return (expected.SequenceEqual(actual)).ToProperty();
        }

        //............................ Helpers ...................................................

        static void TestRange<T>(T[,] matrix, int axis, int rowSkip, int rowTake, int colSkip, int colTake, Func<T[,], int, int, int, int, dynamic> fn)
        {
            var ex = Record.Exception(() => fn(matrix, rowSkip, rowTake, colSkip, colTake));

            if (ex != null)
                Assert.IsType<ArgumentOutOfRangeException>(ex);

            if ((rowSkip < 0 || rowSkip > matrix.GetUpperBound(0))
                || colSkip < 0 || colSkip > matrix.GetUpperBound(1))
                Assert.Contains("skip", ex.Message);
            else if ((rowTake < 0 || rowTake > matrix.GetLength(0))
                || (colTake < 0 || colTake > matrix.GetLength(1)))
                Assert.Contains("take", ex.Message);
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

        public static IEnumerable<object[]> Get2DRanges()
        {
            var data = new int?[3, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };

            yield return new object[] { data, 0, 0, 3, 0 ,3 };
            yield return new object[] { data, 0, -1, 3, 0, 3 };
            yield return new object[] { data, 0, 3, 3, 0 ,3 };
            yield return new object[] { data, 0, 0, -1, 0 ,3 };
            yield return new object[] { data, 0, 0, 4, 0 , 3 };
            yield return new object[] { data, 0, 0, 3, -1, 3, };
            yield return new object[] { data, 0, 0,3, 3, 3, };
            yield return new object[] { data, 0, 0,3, 0, -1, };
            yield return new object[] { data, 0, 0,3, 0, 4, };
        }

        public static (int,int,int,int) Slicer<T>(T[,] sut)
        {
            var rs = 0;
            var rl = sut.GetLength(0);
            var cs = 0;
            var cl = sut.GetLength(1);
            if (rl > 2)
            {
                rs = 1;
                rl--;
            }
            if (cl > 2)
            {
                cs = 1;
                cl--;
            }

            return (rs, rl, cs, cl);

        }

        public static (int, int, int, int) SlicerTranspose<T>(T[,] sut)
        {
            var cs = 0;
            var cl = sut.GetLength(0);
            var rs = 0;
            var rl = sut.GetLength(1);
            if (rl > 2)
            {
                rs = 1;
                rl--;
            }
            if (cl > 2)
            {
                cs = 1;
                cl--;
            }

            return (rs, rl, cs, cl);
        }
    }
}
