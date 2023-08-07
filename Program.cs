using System;

namespace ExportTilesetDefinition
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            new Exporter().Start();
            Console.Read();
        }
    }
}