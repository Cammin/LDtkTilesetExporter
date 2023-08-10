using Utf8Json;

namespace ExportTilesetDefinition
{
    public static class Serialize
    {
        public static byte[] ToJson(this LdtkJson self) => JsonSerializer.Serialize(self);
    }
}