using System.Runtime.Serialization;

namespace ExportTilesetDefinition
{
    public partial class LdtkTableOfContentEntry
    {
        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }

        [DataMember(Name = "instances")]
        public ReferenceToAnEntityInstance[] Instances { get; set; }
    }
}