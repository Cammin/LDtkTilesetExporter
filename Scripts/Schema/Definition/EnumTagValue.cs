﻿using System.Runtime.Serialization;

namespace ExportTilesetDefinition
{
    /// <summary>
    /// In a tileset definition, enum based tag infos
    /// </summary>
    public partial class EnumTagValue
    {
        [DataMember(Name = "enumValueId")]
        public string EnumValueId { get; set; }

        [DataMember(Name = "tileIds")]
        public int[] TileIds { get; set; }
    }
}