namespace TryAtSoftware.Extensions.Collections;

using System;
using TryAtSoftware.Extensions.Collections.Interfaces;

public class RecursiveSegmentTree<TValue> : ISegmentTree<TValue>
{
    private readonly RecursiveSegmentTreeNode<TValue> _root;
    private readonly int _n;

    public RecursiveSegmentTree(int n, ISegmentTreeInitializationEngine<TValue> initializationEngine)
    {
        if (initializationEngine is null) throw new ArgumentNullException(nameof(initializationEngine));
        if (n < 0) throw new ArgumentException("The count of base elements for the segment tree must not be negative.");

        this._root = new RecursiveSegmentTreeNode<TValue>(initializationEngine, 0, n - 1);
        this._n = n;
    }

    public TOutput Query<TOutput>(int index, ISegmentTreeQueryOperator<TValue, TOutput> queryOperator) => this.Query(index, index, queryOperator);

    public TOutput Query<TOutput>(int start, int end, ISegmentTreeQueryOperator<TValue, TOutput> queryOperator)
    {
        this.ValidateBounds(start, end);
        return this._root.Query(start, end, queryOperator);
    }

    public void Update(int index, ISegmentTreeChangeOperator<TValue> changeOperator) => this.Update(index, index, changeOperator);

    public void Update(int start, int end, ISegmentTreeChangeOperator<TValue> changeOperator)
    {
        this.ValidateBounds(start, end);
        this._root.Update(start, end, changeOperator);
    }

    public void LazyUpdate(int index, ISegmentTreeChangeOperator<TValue> changeOperator) => this.LazyUpdate(index, index, changeOperator);

    public void LazyUpdate(int start, int end, ISegmentTreeChangeOperator<TValue> changeOperator)
    {
        this.ValidateBounds(start, end);
        this._root.LazyUpdate(start, end, changeOperator);
    }

    private void ValidateBounds(int start, int end)
    {
        if (start < 0) throw new ArgumentException("The query start index must not be negative.", nameof(start));
        if (end >= this._n) throw new ArgumentException("The query end index must not exceed the original bounds.", nameof(end));
        if (start > end) throw new InvalidOperationException("The query start index must be less than or equal to the query end index");
    }
}

internal class RecursiveSegmentTreeNode<TValue>
{
    private readonly ISegmentTreeInitializationEngine<TValue> _initializationEngine;
    private readonly int _start, _end, _m1, _m2;
    private readonly bool _isLeaf;

    private RecursiveSegmentTreeNode<TValue>? _left, _right;
    private TValue? _value;
    private LazyUpdateDefinition<TValue>? _lazyUpdateDefinition;

    private RecursiveSegmentTreeNode<TValue> Left => this._left ??= new RecursiveSegmentTreeNode<TValue>(this._initializationEngine, this._start, this._m1);
    private RecursiveSegmentTreeNode<TValue> Right => this._right ??= new RecursiveSegmentTreeNode<TValue>(this._initializationEngine, this._m2, this._end);
    private TValue Value => this._value!;

    public RecursiveSegmentTreeNode(ISegmentTreeInitializationEngine<TValue> initializationEngine, int start, int end)
    {
        this._initializationEngine = initializationEngine;
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

        if (this._isLeaf) this._value = this._initializationEngine.CreateInitialValue(this._start);
    }

    // For all the methods below is guaranteed that `this._start <= queryStart <= queryEnd <= this._end`. 
    public TOutput Query<TOutput>(int queryStart, int queryEnd, ISegmentTreeQueryOperator<TValue, TOutput> queryOperator)
    {
        if (this._isLeaf) return queryOperator.ProduceResult(this.Value);

        this.PopulateLazyUpdate();
        if (queryEnd <= this._m1) return this.Left.Query(queryStart, queryEnd, queryOperator);
        if (queryStart >= this._m2) return this.Right.Query(queryStart, queryEnd, queryOperator);

        return queryOperator.Merge(this.Left.Query(queryStart, this._m1, queryOperator), this.Right.Query(this._m2, queryEnd, queryOperator));
    }

    public void Update(int queryStart, int queryEnd, ISegmentTreeChangeOperator<TValue> changeOperator)
    {
        if (this._isLeaf) this._value = changeOperator.ApplyChange(this.Value);
        else
        {
            this.PopulateLazyUpdate();
            if (queryStart <= this._m1) this.Left.Update(queryStart, Math.Min(this._m1, queryEnd), changeOperator);
            if (queryEnd >= this._m2) this.Right.Update(Math.Max(this._m2, queryStart), queryEnd, changeOperator);
        }
    }

    public void LazyUpdate(int queryStart, int queryEnd, ISegmentTreeChangeOperator<TValue> changeOperator)
    {
        if (this._isLeaf) this._value = changeOperator.ApplyChange(this.Value);
        else
        {
            this.PopulateLazyUpdate();
            this._lazyUpdateDefinition = new LazyUpdateDefinition<TValue>(queryStart, queryEnd, changeOperator);
        }
    }

    private void PopulateLazyUpdate()
    {
        if (this._lazyUpdateDefinition is null) return;
        
        if (this._lazyUpdateDefinition.Start <= this._m1) this.Left.LazyUpdate(this._lazyUpdateDefinition.Start, Math.Min(this._m1, this._lazyUpdateDefinition.End), this._lazyUpdateDefinition.ChangeOperator);
        if (this._lazyUpdateDefinition.End >= this._m2) this.Right.LazyUpdate(Math.Max(this._m2, this._lazyUpdateDefinition.Start), this._lazyUpdateDefinition.End, this._lazyUpdateDefinition.ChangeOperator);
        this._lazyUpdateDefinition = null;
    }
}

internal class LazyUpdateDefinition<TValue>(int start, int end, ISegmentTreeChangeOperator<TValue> changeOperator)
{
    public int Start { get; } = start;
    public int End { get; } = end;
    public ISegmentTreeChangeOperator<TValue> ChangeOperator { get; } = changeOperator;
}