using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utf8Json;

namespace ExportTilesetDefinition
{
    /// <summary>
    /// This is the master class for creating tiles+sprite, and backgrounds. It's also ideally the only time we should load textures.
    /// This also will create and export the tileset definition file.
    ///
    /// The previous method just had made sprites based on any first occurrence.
    /// The new way of creating sprites are doing to be localized to individual tileset definitions so that we can make the separate importing
    /// </summary>
    internal sealed class LDtkTilesetDefExporter
    {
        private readonly int _pixelsPerUnit;
        private readonly string _projectPath;
        
        public LDtkTilesetDefExporter(string projectPath, int pixelsPerUnit)
        {
            _pixelsPerUnit = pixelsPerUnit;
            _projectPath = projectPath;
        }

        
        /// <summary>
        /// Figure out what malformed tiles should generate, and then export a tileset definition file for every tileset definition of this project
        /// </summary>
        public void ExportTilesetDefinitions(LdtkJson json)
        {
            var fieldSlices = new LDtkAdditionalTilesFinder(_projectPath).GetAllAdditionalSpritesInProject(json);
            //Dictionary<int, HashSet<RectInt>> fieldSlices = null;
            
            foreach (TilesetDefinition def in json.Defs.Tilesets)
            {
                ExportTilesetDefinition(def, fieldSlices);
            }
        }

        public static string TilesetExportPath(string projectImporterPath, TilesetDefinition def)
        {
            string directoryName = Path.GetDirectoryName(projectImporterPath);
            string projectName = Path.GetFileNameWithoutExtension(projectImporterPath);

            if (directoryName == null)
            {
                Console.WriteLine($"Issue formulating a tileset definition path; Path was invalid for: {projectImporterPath}");
                return null;
            }
            
            return Path.Combine(directoryName, projectName, def.Identifier) + ".ldtkt";
        }

        private void ExportTilesetDefinition(TilesetDefinition def, Dictionary<int, HashSet<Rectangle>> rectsToGenerate)
        {
            string writePath = TilesetExportPath(_projectPath, def);
            
            string writeDir = Path.GetDirectoryName(writePath);
            if (!Directory.Exists(writeDir))
            {
                Directory.CreateDirectory(writeDir);
            }

            LDtkTilesetDefinition data = new LDtkTilesetDefinition()
            {
                Def = def,
                Ppu = _pixelsPerUnit,
            };

            if (rectsToGenerate != null && rectsToGenerate.TryGetValue(def.Uid, out HashSet<Rectangle> rects))
            {
                Console.WriteLine($"Got field slices for def {def.Identifier}: {rects.Count}");
                data.Rects = rects.ToList();
            }
            
            byte[] bytes;
            try
            {
                bytes = data.ToJson();
            }
            catch (Exception e)
            {
                throw new JsonParsingException($"Failed to serialize a tileset definition of {def.Identifier}: {e}");
            }
            
            File.WriteAllBytes(writePath, bytes);
            Console.WriteLine($"Wrote a new tileset definition \"{def.Identifier}\" at: {writePath}");
        }
        
    }
}