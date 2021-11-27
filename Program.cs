using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Linq;

[SimpleJob(10, 5, 50, 15)]
public class Program
{
    [Params(1, 100, 10000)]
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

    public class DictInherited : Dictionary<int, int>, IIndexable<int>
    {
        public DictInherited(int capacity) : base(capacity) {}
    }

    [Benchmark]
    public IIndexable<int> iindexable_dict_inherited()
    {
        IIndexable<int> indexable = new DictInherited(count);
        for (int i = 0; i < count; i++) 
            indexable[i] = 1;
        return indexable;
    }

    [Benchmark]
    public object iindexable_dict_inherited_mixed()
    {
        IIndexable<int>[] indexable = { 
            new DictInherited(count), new ListT(count), 
            new DictInherited(count), new ListT(count),
        };
        for (int i = 0; i < count; i++)
        for (int arrIndex = 0; arrIndex < indexable.Length; arrIndex++) 
        {
            indexable[arrIndex][i] = 1;
        }
        return indexable;
    }

    [Benchmark]
    public object iindexable_dict_inherited_check_mixed()
    {
        IIndexable<int>[] indexable = { 
            new DictInherited(count), new ListT(count), 
            new DictInherited(count), new ListT(count),
        };
        for (int i = 0; i < count; i++)
        for (int arrIndex = 0; arrIndex < indexable.Length; arrIndex++) 
        {
            if (indexable[arrIndex] is ListT a)
                a[i] = 1;
            else if (indexable[arrIndex] is DictInherited d)
                d[i] = 1;
        }
        return indexable;
    }

    public static void Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<Program>();
        // var summary = BenchmarkRunner.Run<MyListVsList>();
        var summary = BenchmarkRunner.Run<Foreaches>();
    }
}

[SimpleJob(10, 5, 10, 5)]
public class MyListVsList
{
    // [Benchmark]
    // public void addin_int_myown()
    // {
    //     MyList<int> list = new MyList<int>(1);
    //     for (int i = 0; i < 1000000; i++)
    //         list.AddIn(i);
    // }

    // [Benchmark]
    // public void add_int_myown()
    // {
    //     MyList<int> list = new MyList<int>(1);
    //     for (int i = 0; i < 1000000; i++)
    //         list.Add(i);
    // }

    // unsafe struct Bigstruct
    // {
    //     public fixed int things[100];
    // }

    // [Benchmark]
    // public unsafe void addin_myown_bigstruct()
    // {
    //     MyList<Bigstruct> list = new MyList<Bigstruct>(1);
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         var a = new Bigstruct();
    //         for (int j = 0; j < 100; j++)
    //             a.things[j] = i;
    //         list.AddIn(a);
    //     }
    // }

    // [Benchmark]
    // public unsafe void add_myown_bigstruct()
    // {
    //     MyList<Bigstruct> list = new MyList<Bigstruct>(1);
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         var a = new Bigstruct();
    //         for (int j = 0; j < 100; j++)
    //             a.things[j] = i;
    //         list.Add(a);
    //     }
    // }

    // [Benchmark]
    // public void add_int_list()
    // {
    //     List<int> list = new List<int>(1);
    //     for (int i = 0; i < 1000000; i++)
    //         list.Add(i);
    // }

    // [Benchmark]
    // public unsafe void add_bigstruct_list()
    // {
    //     List<Bigstruct> list = new List<Bigstruct>(1);
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         var a = new Bigstruct();
    //         for (int j = 0; j < 100; j++)
    //             a.things[j] = i;
    //         list.Add(a);
    //     }
    // }

    // [Benchmark]
    // public void remove_int_myown()
    // {
    //     MyList<int> list = new MyList<int>(10000);
    //     while (list._count < 10000) 
    //         list.Add(list._count);
    //     var comp = EqualityComparer<int>.Default;
    //     for (int i = 0; i < 5000; i++)
    //         list.Remove(i, comp);
    // }

    [Benchmark]
    public void remove_int_myown_no_comparer()
    {
        MyList<int> list = new MyList<int>(25000);
        while (list._count < 25000) 
            list.Add(list._count);
        for (int i = 5000; i < 10000; i++)
            list.Remove(i);
    }

    [Benchmark]
    public void remove_int_myown_loop()
    {
        MyList<int> list = new MyList<int>(25000);
        while (list._count < 25000) 
            list.Add(list._count);
        for (int i = 5000; i < 10000; i++)
            list.RemoveLoop(i);
    }

    [Benchmark]
    public void remove_int_list()
    {
        List<int> list = new List<int>(25000);
        while (list.Count < 25000) 
            list.Add(list.Count); 
        for (int i = 5000; i < 10000; i++)
            list.Remove(i);
    }

    public struct MyList<TElement> where TElement : IEquatable<TElement>
    {
        public TElement[] _array;
        public int _count;

        public MyList(int capacity)
        {
            _array = new TElement[capacity];
            _count = 0;
        }

        public void AddIn(in TElement element)
        {
            if (_count == _array.Length)
                Array.Resize(ref _array, _array.Length * 2);
            _array[_count] = element;
            _count++;
        }

        public void Add(TElement element)
        {
            if (_count == _array.Length)
                Array.Resize(ref _array, _array.Length * 2);
            _array[_count] = element;
            _count++;
        }

        public void RemoveAt(int index)
        {
            Debug.Assert(index < _count);
            _array[index] = _array[_count - 1];
            _count--;
        }

        public void Remove(TElement element)
        {
            int index = Array.IndexOf<TElement>(_array, element, 0, _count);
            Debug.Assert(index != -1);
            RemoveAt(index);
        }

        public void RemoveLoop(TElement element)
        {
            int index = 0;
            while (!_array[index].Equals(element)) 
            { 
                index++; 
                Debug.Assert(index < _count);
            }
            RemoveAt(index);
        }

        public void Remove<Eq>(TElement element, Eq comparer) where Eq : EqualityComparer<TElement>
        {
            int index = 0;
            while (!comparer.Equals(_array[index], element)) 
            { 
                index++; 
                Debug.Assert(index < _count);
            }
            RemoveAt(index);
        }

        public bool TryRemove<Eq>(TElement element, Eq comparer) where Eq : EqualityComparer<TElement>
        {
            // int index = Array.IndexOf<TElement>(_array, element, 0, _count);
            // if (index == -1)
            //     return false;
            // RemoveAt(index);
            // return true;
            for (int index = 0; index < _count; index++)
            {
                if (comparer.Equals(_array[index], element))
                {
                    RemoveAt(index);
                    return true;
                }
            }
            return false;
        }
    }
}

[SimpleJob(10, 5, 15, 15)]
public class Foreaches
{
    unsafe struct Struct
    {
        public fixed int i[100];
        public Struct(int i)
        {
            for (int j = 0; j < 100; j++)
                this.i[j] = i;
        }
    }

    List<int> int_list;
    List<Struct> struct_list;
    int[] int_array;
    Struct[] struct_array;

    const int mil = 1000000;

    [GlobalSetup]
    public void GlobalSetup()
    {
        int_list = new List<int>(mil);
        struct_list = new List<Struct>(mil);
        int_array = new int[mil];
        struct_array = new Struct[mil];    

        for (int i = 0; i < mil; i++)
        {
            int_list.Add(i);
            struct_list.Add(new Struct(i));
            int_array[i] = i;
            struct_array[i] = new Struct(i);
        }
    }

    [Benchmark]
    public int list_foreach_int()
    {
        int sum = 0;
        foreach (int el in int_list)    sum += el;
        return sum;
    }
    
    [Benchmark]
    public unsafe int list_foreach_struct()
    {
        int sum = 0;
        foreach (Struct el in struct_list)    sum += el.i[0];
        return sum;
    }
    
    [Benchmark]
    public int list_for_int()
    {
        int sum = 0;
        for (int i = 0; i < mil; i++)   sum += int_list[i];
        return sum;
    }
    
    [Benchmark]
    public unsafe int list_for_struct()
    {
        int sum = 0;
        for (int i = 0; i < mil; i++) { var a = struct_list[i]; sum += a.i[0]; }
        return sum;
    }

    [Benchmark]
    public int array_foreach_int()
    {
        int sum = 0;
        foreach (int el in int_array)    sum += el;
        return sum;
    }
    
    [Benchmark]
    public unsafe int array_foreach_struct()
    {
        int sum = 0;
        foreach (Struct el in struct_array)    sum += el.i[0];
        return sum;
    }
    
    [Benchmark]
    public int array_for_int()
    {
        int sum = 0;
        for (int i = 0; i < mil; i++)   sum += int_array[i];
        return sum;
    }
    
    [Benchmark]
    public unsafe int array_for_struct()
    {
        int sum = 0;
        for (int i = 0; i < mil; i++) { sum += struct_array[i].i[0]; }
        return sum;
    }
}