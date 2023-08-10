using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExportTilesetDefinition
{
    public static class Profiler
    {
        public static SortedDictionary<int, Data> Messages = new SortedDictionary<int, Data>();

        public class Data
        {
            public string Label;
            public int Ms;

            public void WriteComplete()
            {
                Console.WriteLine($"{Label} completed in {Ms} ms");
            }
        }

        public static void RunWithProfiling(string id, Action method)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            method.Invoke();
            stopwatch.Stop();
            Console.WriteLine($"{id} completed in {stopwatch.Elapsed.Milliseconds} ms");
            Console.WriteLine();
        }
    }
}