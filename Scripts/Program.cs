using System;
using System.Diagnostics;

namespace ExportTilesetDefinition
{
    internal static class Program
    {
        public static Stopwatch Watch = new Stopwatch();
        
        public static void Main(string[] args)
        {
#if DEBUG
            //to work from the test dir
            args = Environment.GetCommandLineArgs();
            if (args.Any(p=> p == "RUN_TEST"))
            {
                string newDir = Path.Combine(Environment.CurrentDirectory, "..", "..", "Test");
                string newPath = Path.GetFullPath(newDir).Replace(Environment.CurrentDirectory, "");
                Directory.SetCurrentDirectory(newPath);
            }
#endif

            Watch.Start();
            new Exporter().Start();
            Watch.Stop();
            
            Console.WriteLine($"Completed Operation in {Watch.ElapsedMilliseconds} ms");
            
#if DEBUG
            Console.Read();
#endif
        }
    }
}