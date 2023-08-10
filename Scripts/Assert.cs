using System;

namespace ExportTilesetDefinition
{
    public static class Assert
    {
        public static void IsTrue(bool condition)
        {
            if (!condition) throw new Exception("Assertion failed");
        }
    }
}