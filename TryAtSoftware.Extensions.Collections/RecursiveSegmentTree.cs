namespace TryAtSoftware.Extensions.Collections;

using System;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class RecursiveSegmentTree<TValue, TOutput, TChange> : ISegmentTree<TOutput, TChange>
{
    private readonly RecursiveSegmentTreeNode<TValue, TOutput, TChange> _root;
    private readonly int _n;

    public RecursiveSegmentTree(ISegmentTreeEngine<TValue, TOutput, TChange> engine, int n)
    {
        if (engine is null) throw new ArgumentNullException(nameof(engine));
        if (n < 0) throw new ArgumentException("The count of base elements for the segment tree must not be negative.");

        this._root = new RecursiveSegmentTreeNode<TValue, TOutput, TChange>(engine, 0, n - 1);
        this._n = n;
    }

    public TOutput Query(int index) => this.Query(index, index);

    public TOutput Query(int start, int end)
    {
        if (start < 0) throw new ArgumentException("The query start index must not be negative.", nameof(start));
        if (end >= this._n) throw new ArgumentException("The query end index must not exceed the original bounds.", nameof(end));
        if (start > end) throw new InvalidOperationException("The query start index must be less than or equal to the query end index");

        return this._root.Query(start, end);
    }

    public void Update(int index, TChange change) => this.Update(index, index, change);

    public void Update(int start, int end, TChange change) => throw new System.NotImplementedException();

    public void LazyUpdate(int index, TChange change) => this.LazyUpdate(index, index, change);

    public void LazyUpdate(int start, int end, TChange change) => throw new System.NotImplementedException();

    private class Node
    {
    }
}

internal class RecursiveSegmentTreeNode<TValue, TOutput, TChange>
{
    private readonly ISegmentTreeEngine<TValue, TOutput, TChange> _engine;
    private readonly int _start, _end, _m1, _m2;
    private readonly bool _isLeaf;

    private RecursiveSegmentTreeNode<TValue, TOutput, TChange>? _left, _right;
    private TValue? _value;

    private RecursiveSegmentTreeNode<TValue, TOutput, TChange> Left => this._left ??= new RecursiveSegmentTreeNode<TValue, TOutput, TChange>(this._engine, this._start, this._m1);
    private RecursiveSegmentTreeNode<TValue, TOutput, TChange> Right => this._right ??= new RecursiveSegmentTreeNode<TValue, TOutput, TChange>(this._engine, this._m2, this._end);
    private TValue Value => this._value!;

    public RecursiveSegmentTreeNode(ISegmentTreeEngine<TValue, TOutput, TChange> engine, int start, int end)
    {
        this._engine = engine;
        this._start = start;
        this._end = end;
        this._isLeaf = start == end;

        if (end - start < 2)
        {
            this._m1 = start;
            this._m2 = end;
        }
        else
        {
            this._m1 = start + (end - start) / 2;
            this._m2 = this._m1 + 1;
        }

        if (this._isLeaf) this._value = engine.CreateDefaultValue();
    }

    // It is guaranteed that `this._start <= queryStart <= queryEnd <= this._end`. 
    public TOutput Query(int queryStart, int queryEnd)
    {
        if (this._isLeaf) return this._engine.ProduceResult(this.Value);

        if (queryEnd <= this._m1) return this.Left.Query(queryStart, queryEnd);
        if (queryStart >= this._m2) return this.Right.Query(queryStart, queryEnd);

        return this._engine.Merge(this.Left.Query(queryStart, this._m1), this.Right.Query(this._m2, queryEnd));
    }
}