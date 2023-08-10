using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ExportTilesetDefinition
{
    internal static class Program
    {
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

            Profiler.RunWithProfiling("BeginExport", () =>
            {
                new Exporter().Start();
            });
            

#if DEBUG
            Console.Read();
#endif
        }
    }
}