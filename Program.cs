using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Order;
using System.Diagnostics.CodeAnalysis;
using System.Buffers;
using Microsoft.Win32.SafeHandles;
using System.Reflection;

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
        // var summary = BenchmarkRunner.Run<Foreaches>();
        // var summary = BenchmarkRunner.Run<AsyncVsSyncFiles>();
        // var summary = BenchmarkRunner.Run<EnumerateFilesVsGetFiles1>();
        var summary = BenchmarkRunner.Run<Files8>();
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

// [SimpleJob(launchCount: 2, warmupCount: 1, targetCount: 2)]
[Orderer(SummaryOrderPolicy.Method)]
[MemoryDiagnoser]
public class AsyncVsSyncFiles
{
    const int kb = 1024;
    const int mb = kb * kb;
    [Params(kb, mb)]
    public long numBytes;
    [Params(5, 10, 25)]
    public int numFiles;
    public byte[] bytes;

    [GlobalSetup]
    public void GlobalSetup()
    {
        bytes = new byte[numBytes]; // executed once per each N value
        _toClear.Clear();
    }

    public List<string> _toClear = new List<string>(); 


    [GlobalCleanup]
    public void GlobalCleanup()
    {
        foreach (var d in _toClear)
        {
            foreach (var f in Directory.EnumerateFiles(d))
                File.Delete(f);
            Directory.Delete(d);
        }
    }

    public string MakeTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_sync");
        _toClear.Add(dir);
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Benchmark]
    public void write_bytes_sync()
    {
        var dir = MakeTempDir();
        for (int i = 0; i < numFiles; i++)
        {
            var filePath = Path.Combine(dir, i.ToString());
            File.WriteAllBytes(filePath, bytes);
            bytes[i] = (byte) i;
        }
    }

    [Benchmark]
    public async Task write_bytes_async()
    {
        var dir = MakeTempDir();
        var tasks = new Task[numFiles];
        for (int i = 0; i < numFiles; i++)
        {
            var filePath = Path.Combine(dir, i.ToString());
            tasks[i] = File.WriteAllBytesAsync(filePath, bytes);
            bytes[i] = (byte) (i + 1);
        }
        await Task.WhenAll(tasks);
    }
}

[HtmlExporter]
[MemoryDiagnoser]
[MinIterationCount(5)]
[MaxIterationCount(15)]
[MinWarmupCount(1)]
[MaxWarmupCount(10)]
public class EnumerateFilesVsGetFiles1
{
    string path = @"E:\Coding";
    // [Benchmark]
    public string[] enumerate_no_filter()
    {
        return Directory.EnumerateFiles(path).ToArray();
    }
    
    // [Benchmark]
    public string[] get_files_no_filter()
    {
        return Directory.GetFiles(path);
    }

    
    // [Benchmark]
    public string[] enumerate_source_files()
    {
        return Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).ToArray();
    }
    
    // [Benchmark]
    public string[] get_source_files()
    {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
    }


    string ignoredFile = Path.GetFullPath("Program.cs");
    // [Benchmark]
    public string[] get_files_except_one()
    {
        var files = Directory.GetFiles(path);
        return files.Where(f => f != ignoredFile).ToArray();
    }

    // [Benchmark]
    public string[] enumerate_files_except_one()
    {
        return Directory.EnumerateFiles(path).Where(f => f != ignoredFile).ToArray();
    }


    // [Benchmark]
    public string[] get_source_files_except_one()
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => f != ignoredFile).ToArray();
    }

    // [Benchmark]
    public string[] enumerate_source_files_except_one()
    {
        return Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(f => f != ignoredFile).ToArray();
    }

    string ignoredDirectoryName = "avr";
    // [Benchmark]
    public string[] get_files_that_are_not_in_directory_simplest()
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => new DirectoryInfo(Path.GetDirectoryName(f)).Name != ignoredDirectoryName).ToArray();
    }
    
    // [Benchmark]
    public string[] enumerate_files_that_are_not_in_directory_simplest()
    {
        var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => new DirectoryInfo(Path.GetDirectoryName(f)).Name != ignoredDirectoryName).ToArray();
    }

    record Ignorer(string ignoredName) : IShouldIgnoreDirectory
    {
        public bool ShouldIgnoreDirectory(string fullFilePath)
        {
            return new DirectoryInfo(Path.GetDirectoryName(fullFilePath)).Name == ignoredName;
        }
    }

    record Ignorer2(string ignoredName) : IShouldIgnoreDirectory
    {
        public bool ShouldIgnoreDirectory(string fullFilePath)
        {
            var i = fullFilePath.LastIndexOf(Path.DirectorySeparatorChar);
            if (i == -1)
                return false;
            int length = fullFilePath.Length - (i + 1);
            var s = fullFilePath.AsSpan(i + 1, length);
            return s.SequenceCompareTo(ignoredName) == 0;
        }
    }

    [Benchmark]
    public string[] get_files_that_are_not_in_directory_ignorer()
    {
        return IShouldIgnoreDirectory.EnumerateFilesIgnoring(path, new Ignorer(ignoredDirectoryName), "*.cs").ToArray();
    }

    [Benchmark]
    public string[] get_files_that_are_not_in_directory_ignorer_less_alloc()
    {
        return IShouldIgnoreDirectory.EnumerateFilesIgnoring(path, new Ignorer2(ignoredDirectoryName), "*.cs").ToArray();
    }

    static bool IsIgnored(string path, string ignoredName)
    {
        int idx = path.IndexOf(Path.DirectorySeparatorChar);
        while (idx != -1 && idx < path.Length)
        {
            int next = path.IndexOf(Path.DirectorySeparatorChar, idx + 1);
            if (next == -1)
                next = path.Length;
            int length = next - (idx + 1);
            if (path.AsSpan(idx + 1, length).SequenceCompareTo(ignoredName) == 0)
                return true;
            idx = next;
        }
        return false;
    }

    static bool IsIgnoredArray(string path, string[] ignoredNames)
    {
        int idx = path.IndexOf(Path.DirectorySeparatorChar);
        while (idx != -1 && idx < path.Length)
        {
            int next = path.IndexOf(Path.DirectorySeparatorChar, idx + 1);
            if (next == -1)
                next = path.Length;
            int length = next - (idx + 1);
            foreach (var ignoredName in ignoredNames)
                if (path.AsSpan(idx + 1, length).SequenceCompareTo(ignoredName) == 0)
                    return true;
            idx = next;
        }
        return false;
    }

    // [Benchmark]
    public string[] get_files_that_are_not_in_directory_all_directories_with_this_name()
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => !IsIgnored(f, ignoredDirectoryName)).ToArray();
    }

    [Benchmark]
    public string[] enumerate_files_that_are_not_in_directory_all_directories_with_this_name()
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => !IsIgnored(f, ignoredDirectoryName)).ToArray();
    }


    string[] ignoredNames = new[] { "avr", "a", "Kari", "Hello", "Stuff", "bin", "obj" };

    record IgnorerArray(string[] ignoredNames) : IShouldIgnoreDirectory
    {
        public bool ShouldIgnoreDirectory(string fullFilePath)
        {
            var i = fullFilePath.LastIndexOf(Path.DirectorySeparatorChar);
            if (i == -1)
                return false;
            int length = fullFilePath.Length - (i + 1);
            var s = fullFilePath.AsSpan(i + 1, length);
            foreach (var f in ignoredNames)
            {
                if (s.SequenceCompareTo(f) == 0)
                    return true;
            }
            return false;
        }
    }

    [Benchmark]
    public string[] get_files_that_are_not_in_directory_ignorer_array()
    {
        return IShouldIgnoreDirectory.EnumerateFilesIgnoring(path, new IgnorerArray(ignoredNames), "*.cs").ToArray();
    }

    [Benchmark]
    public string[] enumerate_files_that_are_not_in_directory_all_directories_with_this_name_array()
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        return files.Where(f => !IsIgnoredArray(f, ignoredNames)).ToArray();
    }
}


public interface IShouldIgnoreDirectory
{
    bool ShouldIgnoreDirectory(string fullFilePath);

    public static IEnumerable<string> EnumerateFilesIgnoring(
        [NotNull] string rootDirectory, 
        [NotNull] IShouldIgnoreDirectory ignore,
        [NotNull] string fileSearchPattern = "*")
    {
        Debug.Assert(rootDirectory != null, "Check yourself before calling");
        Debug.Assert(ignore != null, "Check yourself before calling");
        Debug.Assert(fileSearchPattern != null, "Invalid pattern");

        // RESULT: twice as slow as filtering EnumerateFiles
        Stack<string> directories = new Stack<string>();
        directories.Push(rootDirectory);
        while (directories.Count > 0)
        {
            string current = directories.Pop();
            if (ignore.ShouldIgnoreDirectory(current))
                continue;
            foreach (var subdir in Directory.EnumerateDirectories(current, "*", SearchOption.TopDirectoryOnly))
                directories.Push(subdir);
            foreach (var file in Directory.EnumerateFiles(current, fileSearchPattern, SearchOption.TopDirectoryOnly))
                yield return file;
        }
    }
}


[HtmlExporter]
[MemoryDiagnoser]
[MinIterationCount(5)]
[MaxIterationCount(15)]
[MinWarmupCount(1)]
[MaxWarmupCount(10)]
public class Files8
{
    const int kb = 1024;
    const int mb = kb * kb;
    
    
    [Params(
        //kb, 
        // 128 * kb
        // 1 * kb,
        2 * kb,
        // 4 * kb,
        8 * kb,
        // 16 * kb,
        64 * kb
    // 16 * kb, 64 * kb
    // , mb
    )]
    public int numBytesPerChunk;
    
    
    [Params(
        // 1, 2
        // , 
        10
        // , 10
    )]
    public int numChunks;


    [Params(
        // 1, 5, 
        // 20 
        //, 25
        10
        // , 50
    )]
    public int numFilesAtOnce;


    [Params(1, 2, 4, 8)]
    public int numParallelHandles;
    
    
    public byte[][] bytes;


    [GlobalSetup]
    public void GlobalSetup()
    {
        bytes = new byte[numChunks][]; // executed once per each N value
        for (int i = 0; i < numChunks; i++)
            bytes[i] = new byte[numBytesPerChunk];
        _toClear.Clear();
    }

    public List<string> _toClear = new List<string>(); 


    [GlobalCleanup]
    public void GlobalCleanup()
    {
        foreach (var d in _toClear)
        {
            foreach (var f in Directory.EnumerateFiles(d))
                File.Delete(f);
            Directory.Delete(d);
        }
    }

    public string MakeTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_sync");
        _toClear.Add(dir);
        Directory.CreateDirectory(dir);
        return dir;
    }


    // [Benchmark]
    public void write_files_sync()
    {
        var arr = ArrayPool<byte>.Shared.Rent(numChunks * numBytesPerChunk);
        string dir = MakeTempDir();
        for (int fileIndex = 0; fileIndex < numFilesAtOnce; fileIndex++)
        {
            for (int chunkIndex = 0; chunkIndex < numChunks; chunkIndex++)
            {
                Buffer.BlockCopy(bytes[chunkIndex], 0, arr, chunkIndex * numBytesPerChunk, numBytesPerChunk); 
            }
            File.WriteAllBytes(Path.Join(dir, fileIndex.ToString() + ".txt"), arr); 
        }
        ArrayPool<byte>.Shared.Return(arr);
    }

    public Task WriteFilesAsync(bool queueWithTaskRun)
    {
        Task[] result = new Task[numFilesAtOnce];
        string dir = MakeTempDir();
        for (int i = 0; i < result.Length; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            if (queueWithTaskRun)
                result[i] = Task.Run(delegate { WriteFileAsync(path, bytes).Wait(); }); 
            else
                result[i] = WriteFileAsync(path, bytes);
        }
        return Task.WhenAll(result);

        static async Task WriteFileAsync(string path, byte[][] bytes)
        {
            int length = bytes.Sum(b => b.Length);
            var arr = ArrayPool<byte>.Shared.Rent(length);

            int offset = 0;
            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
            {
                Buffer.BlockCopy(bytes[chunkIndex], 0, arr, offset, bytes[chunkIndex].Length); 
                offset += bytes[chunkIndex].Length;
            }

            await File.WriteAllBytesAsync(path, arr); 
            ArrayPool<byte>.Shared.Return(arr);
        }
    }

    // [Benchmark]
    public Task write_files_async1()
    {
        return WriteFilesAsync(false);
    }

    // [Benchmark]
    public Task write_files_async2()
    {
        return WriteFilesAsync(true);
    }

    public static class ReflectedFileStreamHelpers
    {
        // The method I need is internal.
        // System.IO.Strategies.FileStreamHelpers.SetFileLength(SafeFileHandle, long);
        public static readonly Action<SafeFileHandle, long> SetFileLength;

        static ReflectedFileStreamHelpers()
        {
            SetFileLength = typeof(FileStream).Assembly
                .GetType("System.IO.Strategies.FileStreamHelpers")
                .GetMethod("SetFileLength", BindingFlags.Static|BindingFlags.NonPublic)
                .CreateDelegate<Action<SafeFileHandle, long>>();
        }
    }

    // [Benchmark]
    public Task write_files_async_without_buffer_copies_safe_handle()
    {
        Task[] result = new Task[numFilesAtOnce];
        string dir = MakeTempDir();
        for (int i = 0; i < result.Length; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            result[i] = WriteFileAsync(path, bytes);
        }
        return Task.WhenAll(result);

        static async Task WriteFileAsync(string path, byte[][] bytes)
        {
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);

            int offset = 0;
            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
            {
                await RandomAccess.WriteAsync(handle, bytes[chunkIndex], offset);
                offset += bytes[chunkIndex].Length;
            }
        }
    }

    // [Benchmark]
    public async Task write_files_almost_sync_without_buffer_copies_safe_handle()
    {
        string dir = MakeTempDir();
        for (int i = 0; i < numFilesAtOnce; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            await WriteFileAsync(path, bytes);
        }

        static async Task WriteFileAsync(string path, byte[][] bytes)
        {
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);

            int offset = 0;
            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
            {
                await RandomAccess.WriteAsync(handle, bytes[chunkIndex], offset);
                offset += bytes[chunkIndex].Length;
            }
        }
    }

    // [Benchmark]
    public async Task write_files_almost_sync_file_stream()
    {
        string dir = MakeTempDir();
        for (int i = 0; i < numFilesAtOnce; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            await WriteFileAsync(path, bytes);
        }

        static async Task WriteFileAsync(string path, byte[][] bytes)
        {
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);
            using FileStream fileStream = new FileStream(handle, FileAccess.Write);

            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
                await fileStream.WriteAsync(bytes[chunkIndex]);
        }
    }

    
    // [Benchmark]
    public void write_files_sync_file_stream()
    {
        string dir = MakeTempDir();
        for (int i = 0; i < numFilesAtOnce; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);
            using FileStream fileStream = new FileStream(handle, FileAccess.Write);

            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
                fileStream.Write(bytes[chunkIndex]);
        }
    }


    // [Benchmark]
    public void write_files_sync_without_buffer_copies_safe_handle()
    {
        string dir = MakeTempDir();
        for (int i = 0; i < numFilesAtOnce; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);

            int offset = 0;
            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
            {
                RandomAccess.Write(handle, bytes[chunkIndex], offset);
                offset += bytes[chunkIndex].Length;
            }
        }
    }


    // [Benchmark]
    public Task write_files_async_with_buffering_all_safe_handle()
    {
        Task[] result = new Task[numFilesAtOnce];
        string dir = MakeTempDir();
        for (int i = 0; i < result.Length; i++)
        {
            var path = Path.Join(dir, i.ToString() + ".txt");
            result[i] = WriteFileAsync(path, bytes);
        }
        return Task.WhenAll(result);

        static async Task WriteFileAsync(string path, byte[][] bytes)
        {

            int length = bytes.Sum(b => b.Length);
            var arr = ArrayPool<byte>.Shared.Rent(length);

            int offset = 0;
            for (int chunkIndex = 0; chunkIndex < bytes.Length; chunkIndex++)
            {
                Buffer.BlockCopy(bytes[chunkIndex], 0, arr, offset, bytes[chunkIndex].Length); 
                offset += bytes[chunkIndex].Length;
            }

            using SafeFileHandle handle = File.OpenHandle(path, FileMode.CreateNew, FileAccess.Write);
            await RandomAccess.WriteAsync(handle, arr, offset);
            
            ArrayPool<byte>.Shared.Return(arr);
        }
    }
}
