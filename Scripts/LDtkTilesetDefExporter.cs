using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDtkUnity;
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
            //var fieldSlices = GetAllAdditionalSpritesInProject(json);
            //Dictionary<int, HashSet<RectInt>> fieldSlices = null;
            
            foreach (TilesetDefinition def in json.Defs.Tilesets)
            {
                ExportTilesetDefinition(def, null);
            }
        }
        
        /// <summary>
        ///Dict of (TilesetUid => Rects)
        /// 
        /// Gets any and all information of used tiles. tiles from level/entity fields. these are the tiles that could have a size that deviates from the tileset definition's gridSize
        /// Important: when we get the slices inside levels, we need to dig into json for some of these.
        /// Because we are looking for tile instances in levels, 
        /// Technically we only need to grab the malformed tiles; the ones that are not in equal width/height of the layer's gridsize.
        /// This is because all of the gridsize tiles are going to be generated regardless.
        /// The instances where this can be the case are: editor visual for enum def value, editor visual for entity, and tile fields from levels/entities
        /// </summary>
        /*private Dictionary<int, HashSet<RectInt>> GetAllAdditionalSpritesInProject(LdtkJson json)
        {
            Dictionary<int, HashSet<RectInt>> fieldSlices = new Dictionary<int, HashSet<RectInt>>();

            foreach (World world in json.UnityWorlds)
            {
                foreach (Level level in world.Levels)
                {
                    //Entity tile fields. If external levels, then dig into the level. If in our own json, then we can safely get them from the layer instances in the json.
                    if (json.ExternalLevels)
                    {
                        string levelPath = new LDtkRelativeGetterLevels().GetPath(level, _projectPath);
                        List<FieldInstance> fields = new List<FieldInstance>();
                        if (!LDtkJsonDigger.GetUsedFieldTiles(levelPath, ref fields))
                        {
                            Console.WriteLine($"Couldn't get entity tile field instance for level: {level.Identifier}");
                            continue;
                        }
                    
                        foreach (FieldInstance field in fields)
                        {
                            TryAddFieldInstance(field);
                        }
                        continue;
                    }

                    //NOTICE: depending on performance from directly getting json data instead of digging, i'll release this back.
                    //else it's not external levels and can ge grabbed from the json data for better performance
                    //Level field instances are still available in project json even with separate levels. They are both available in project and separate level files

                    //level's field instances
                    foreach (FieldInstance levelFieldInstance in level.FieldInstances) 
                    {
                        TryAddFieldInstance(levelFieldInstance);
                    }
                
                    //entity field instances
                    foreach (LayerInstance layer in level.LayerInstances)
                    {
                        foreach (EntityInstance entity in layer.EntityInstances)
                        {
                            foreach (FieldInstance entityField in entity.FieldInstances)
                            {
                                TryAddFieldInstance(entityField);
                            }
                        }
                    }
                    
                    void TryAddFieldInstance(FieldInstance field)
                    {
                        if (!field.IsTile)
                        {
                            return;
                        }

                        TilesetRectangle[] rects = GetTilesetRectanglesFromField(field);
                        if (rects == null)
                        {
                            return;
                        }
                        foreach (TilesetRectangle rect in rects) //the expected value here is a string of the field.Value
                        {
                            //Debug.Log($"Element {element}");
                            if (rect == null)
                            {
                                Console.WriteLine($"A FieldInstance element was null for {field.Identifier}");
                                continue;
                            }

                            //deny adding duplicated to avoid identifier uniqueness
                            int key = rect.TilesetUid;
                            if (!fieldSlices.ContainsKey(key))
                            {
                                fieldSlices.Add(key, new HashSet<RectInt>());
                            }
                            fieldSlices[key].Add(rect.UnityRectInt);
                        }
                    }
                }
            }
            return fieldSlices;
        }*/
        
        
        /*private TilesetRectangle[] GetTilesetRectanglesFromField(FieldInstance field)
        {
            //if it's prebuilt via the json digger
            if (field.Value is TilesetRectangle[] arrayRects)
            {
                return arrayRects;
            }
            
            bool isArray = field.Definition.IsArray;
            
            //if it was a single null value
            if (field.Value == null)
            {
                if (isArray)
                {
                    Console.WriteLine($"Null array? This is never supposed to be reached.");
                }

                return null;
            }
            

            if (isArray)
            {
                if (field.Value is List<object> rectangles)
                {
                    return rectangles.Select(ConvertDict).Where(p => p != null).ToArray();
                }

                Console.WriteLine($"This is never supposed to be reached. {field.Identifier} {field.Value.GetType()}");
                return null;
            }

            if (field.Value is Dictionary<string, object> dict)
            {
                return new TilesetRectangle[] {ConvertDict(dict)};
            }
            
            //this could be if we did a json dig to construct this array
            if (field.Value is TilesetRectangle[] rects)
            {
                return rects;
            }

            Console.WriteLine($"This is never supposed to be reached. {field.Identifier} {field.Value.GetType()}");
            return null;
        }*/
        
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
            
            TryCreateDirectoryForFile(writePath);

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
        
        public static void TryCreateDirectoryForFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        public static TilesetRectangle ConvertDict(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is Dictionary<string, object> dict)
            {
                double tilesetUid = (double)dict["tilesetUid"];
                double x = (double)dict["x"];
                double y = (double)dict["y"];
                double w = (double)dict["w"];
                double h = (double)dict["h"];
                return new TilesetRectangle
                {
                    TilesetUid = (int)tilesetUid,
                    X = (int)x,
                    Y = (int)y,
                    W = (int)w,
                    H = (int)h
                };
            }

            Console.WriteLine("Issue parsing tile");
            return null;
        }
    }
}