using System.Collections.Generic;
using Utf8Json;

namespace ExportTilesetDefinition
{
    /// <summary>
    /// A wrapper on the TilesetDefinition that contains some additional data in order to independently import certain tiles related to this tileset definition.
    /// We're making this because it's harder to generate an asset and additionally set it's importer's bonus metadata in the same pass.
    /// So we're writing our own text instead to provide that data.
    /// </summary>
    public class LDtkTilesetDefinition
    {
        /// <summary>
        /// AdditionalRects; Contains all tile rects that are not involved with the standard grid slices.
        /// These are not included with the sprite editor window integration, as not only do they overlap when trying to click on a sprite to edit, but also aren't gonna have tilemap assets generated for them anyways, as they wouldn't fit.
        /// These could be extra rects that are shaped like rectangles to slice, from tile field definitions, or icons maybe.
        /// </summary>
        public List<Rectangle> Rects;
        
        /// <summary>
        /// The tileset definition. 
        /// </summary>
        public TilesetDefinition Def;
        
        public byte[] ToJson()
        {
            byte[] serialize = JsonSerializer.Serialize(this);
            return JsonSerializer.PrettyPrintByteArray(serialize);
        }
    }
}