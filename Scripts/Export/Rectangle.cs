using System;

namespace ExportTilesetDefinition
{
    [Serializable]
    public struct Rectangle : IEquatable<Rectangle>
    {
        public int x;
        public int y;
        public int w;
        public int h;
        
        public bool Equals(Rectangle other)
        {
            return x == other.x && y == other.y && w == other.w && h == other.h;
        }

        public override string ToString()
        {
            return $"(x:{x}, y:{y}, width:{w}, height:{h})";
        }
    }
}
