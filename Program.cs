using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[SimpleJob(5, 5, 15, 15)]
public class Program
{
    [Params(1, 100, 100000)]
    public int count;
    
    [Benchmark]
    public IList<int> ilist()
    {
        IList<int> ilist = new int[count]; 
        for (int i = 0; i < count; i++) 
            ilist[i] = 1;
        return ilist;
    }

    [Benchmark]
    public IDictionary<int, int> idict()
    {
        IDictionary<int, int> idict = new Dictionary<int, int>(count);
        for (int i = 0; i < count; i++) 
            idict[i] = 1;
        return idict;
    }

    [Benchmark]
    public int[] array()
    {
        int[] arr = new int[count]; 
        for (int i = 0; i < count; i++) 
            arr[i] = 1;
        return arr;
    }

    [Benchmark]
    public IDictionary<int, int> dict()
    {
        IDictionary<int, int> dict = new Dictionary<int, int>(count);
        for (int i = 0; i < count; i++) 
            dict[i] = 1;
        return dict;
    }

	public interface IIndexable<T>
	{
		T this[int index] { get; set; }
	}

    public class ListT : IIndexable<int>
    {
        public int[] list;
        public ListT(int size) => this.list = new int[size];

        public int this[int index] { 
            get => list[index]; 
            set => list[index] = value; 
        }
    }

    public class DictionaryT : IIndexable<int>
    {
        public Dictionary<int, int> dict;
        public DictionaryT(int size) => this.dict = new Dictionary<int, int>(size);

        public int this[int index] { 
            get => dict[index]; 
            set => dict[index] = value; 
        }
    }

    [Benchmark]
    public IIndexable<int> iindexable_array()
    {
        IIndexable<int> indexable = new ListT(count);
        for (int i = 0; i < count; i++) 
            indexable[i] = 1;
        return indexable;
    }

    [Benchmark]
    public IIndexable<int> iindexable_dict()
    {
        IIndexable<int> indexable = new DictionaryT(count);
        for (int i = 0; i < count; i++) 
            indexable[i] = 1;
        return indexable;
    }

    [Benchmark]
    public object check_dict()
    {
        object indexable = new Dictionary<int, int>(count);
        for (int i = 0; i < count; i++) 
            if (indexable is Dictionary<int, int> d)
                d[i] = 1;
        return indexable;
    }

    [Benchmark]
    public object check_arr()
    {
        object indexable = new int[count];
        for (int i = 0; i < count; i++) 
            if (indexable is int[] a)
                a[i] = 1;
        return indexable;
    }

    [Benchmark]
    public object check_mixed()
    {
        object[] indexable = { 
            new Dictionary<int, int>(count), new int[count], 
            new Dictionary<int, int>(count), new int[count],
        };
        
        for (int i = 0; i < count; i++)
        for (int arrIndex = 0; arrIndex < indexable.Length; arrIndex++) 
        {
            if (indexable[arrIndex] is int[] a)
                a[i] = 1;
            else if (indexable[arrIndex] is Dictionary<int, int> d)
                d[i] = 1;
        }

        return indexable;
    }

    [Benchmark]
    public object iindexable_mixed()
    {
        IIndexable<int>[] indexable = { 
            new DictionaryT(count), new ListT(count), 
            new DictionaryT(count), new ListT(count),
        };
        
        for (int i = 0; i < count; i++)
        for (int arrIndex = 0; arrIndex < indexable.Length; arrIndex++) 
        {
            indexable[arrIndex][i] = 1;
        }
        
        return indexable;
    }

    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Program>();
    }
}