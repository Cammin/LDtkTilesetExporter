using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utf8Json;

namespace ExportTilesetDefinition
{
    public class LDtkAdditionalTilesFinder
    {
        private readonly string _projectPath;
        
        public LDtkAdditionalTilesFinder(string projectPath)
        {
            _projectPath = projectPath;
        }
        
        /// <summary>
        ///Dict of (TilesetUid => Rects)
        /// 
        /// Gets any and all information of used tiles. tiles from level/entity fields. these are the tiles that could have a size that deviates from the tileset definition's gridSize
        /// Important: when we get the slices inside levels, we need to dig into json for some of these.
        /// Because we are looking for tile instances in levels, 
        /// Technically we only need to grab the malformed tiles; the ones that are not in equal width/height of the layer's gridsize.
        /// This is because all the grid size tiles are going to be generated regardless.
        /// The instances where this can be the case are: editor visual for enum def value, editor visual for entity, and tile fields from levels/entities
        ///
        /// Instance-based possible sources of additional sprites are:
        /// - EntityInstance.Tile
        /// - EntityInstance.FieldInstances.Tile
        /// - Level.FieldInstances.Tile
        ///
        /// Definition-based possible sources of additional sprites are:
        /// - AutoLayerRuleGroup.Icon
        /// - EntityDefinition.TileRect
        /// - EntityDefinition.UiTileRect
        /// - EnumValueDefinition.TileRect
        /// - IntGridValueDefinition.Tile
        /// </summary>
        public Dictionary<int, HashSet<Rectangle>> GetAllAdditionalSpritesInProject(LdtkJson json)
        {
            Dictionary<int, HashSet<Rectangle>> allRects = new Dictionary<int, HashSet<Rectangle>>();
            
            // - AutoLayerRuleGroup.Icon
            // - IntGridValueDefinition.Tile
            foreach (LayerDefinition layer in json.Defs.Layers)
            {
                foreach (AutoLayerRuleGroup group in layer.AutoRuleGroups)
                {
                    AddToDict(group.Icon);
                }
                foreach (IntGridValueDefinition value in layer.IntGridValues)
                {
                    AddToDict(value.Tile);
                }
            }
            
            // - EntityDefinition.TileRect
            // - EntityDefinition.UiTileRect
            foreach (EntityDefinition def in json.Defs.Entities)
            {
                AddToDict(def.TileRect);
                AddToDict(def.UiTileRect);
            }
            
            // - EnumValueDefinition.TileRect
            foreach (var def in json.Defs.Enums.Concat(json.Defs.ExternalEnums))
            {
                foreach (EnumValueDefinition value in def.Values)
                {
                    AddToDict(value.TileRect);
                }
            }
            
            //Instances
            World[] worlds = GetWorlds(json);
            foreach (World world in worlds)
            {
                if (json.ExternalLevels)
                {
                    for (int i = 0; i < world.Levels.Length; i++)
                    {
                        world.Levels[i] = GetSeparateLevel(world.Levels[i]);
                    }
                    Console.WriteLine();
                    
                    Level GetSeparateLevel(Level srcLvl)
                    {
                        Stopwatch stopWatch = Stopwatch.StartNew();
                        Level lvl;
                        try
                        {
                            string levelPath = GetPathRelativeToPath(_projectPath, srcLvl.ExternalRelPath);
                            byte[] levelJson = File.ReadAllBytes(levelPath);
                            lvl = JsonSerializer.Deserialize<Level>(levelJson);
                        }
                        catch (Exception e)
                        {
                            throw new JsonParsingException($"Failed to load separate level \"{srcLvl.Identifier}\": {e}");
                        }
                        finally
                        {
                            stopWatch.Stop();
                        }
                        
                        Console.WriteLine($"Deserialize separate level \"{srcLvl.Identifier}\" in {stopWatch.Elapsed.Milliseconds} ms");
                        return lvl;
                    }
                }
                
                // - EntityInstance.Tile
                // - EntityInstance.FieldInstances.Tile
                // - Level.FieldInstances.Tile
                foreach (Level level in world.Levels)
                {
                    //level fields
                    foreach (FieldInstance levelFieldInstance in level.FieldInstances) 
                    {
                        TryAddFieldInstance(levelFieldInstance);
                    }
                    
                    //entity tile and entity fields
                    foreach (LayerInstance layer in level.LayerInstances)
                    {
                        foreach (EntityInstance entity in layer.EntityInstances)
                        {
                            AddToDict(entity.Tile);
                            
                            foreach (FieldInstance entityField in entity.FieldInstances)
                            {
                                TryAddFieldInstance(entityField);
                            }
                        }
                    }
                    continue;

                    void TryAddFieldInstance(FieldInstance field)
                    {
                        if (!IsTile(field))
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
                            AddToDict(rect);
                        }
                    }
                    
                    void DoSomeSeparateLevelLoading()
                    {
                        //NOTICE: depending on performance from directly getting json data instead of digging, i'll release this back.
                        //else it's not external levels and can ge grabbed from the json data for better performance
                        //Level field instances are still available in project json even with separate levels. They are both available in project and separate level files
                        //level's field instances
                        //discarded for the time being. look back on it if there's performance concerns
                        
                        //Entity tile fields. If external levels, then dig into the level. If in our own json, then we can safely get them from the layer instances in the json.
                        if (json.ExternalLevels)
                        {
                            string levelPath = GetPathRelativeToPath(_projectPath, level.ExternalRelPath);

                            List<FieldInstance> fields = new List<FieldInstance>();
                        
                            //TEMP: don't use the digger for now just to make things simpler for now. if the export app gets slow, then we can reconsider
                            if (!JsonDigger.GetUsedFieldTiles(levelPath, ref fields))
                            {
                                Console.WriteLine($"Couldn't get entity tile field instance for level: {level.Identifier}");
                                //continue;
                            }

                        
                            foreach (FieldInstance field in fields)
                            {
                                TryAddFieldInstance(field);
                            }
                        
                            //continue;
                        }
                    }
                }
            }
            return allRects;

            void AddToDict(TilesetRectangle rect)
            {
                if (rect == null)
                {
                    return;
                }
                
                var key = rect.TilesetUid;
                if (!allRects.ContainsKey(key))
                {
                    allRects.Add(key, new HashSet<Rectangle>());
                }
                allRects[key].Add(FromTilesetRectangle(rect));
            }
            
            
        }

        public string GetPathRelativeToPath(string assetPath, string relPath)
        {
            string startDir = Path.GetDirectoryName(assetPath);
            string dirtyPath = Path.Combine(startDir, relPath);
            string fullPath = Path.GetFullPath(dirtyPath);
            return fullPath.Replace('\\', '/');
        }
        
        public bool IsTile(FieldInstance instance)
        {
            return instance.Type == "Tile" || instance.Type == "Array<Tile>"; 
        }
        
        public static Rectangle FromTilesetRectangle(TilesetRectangle rect)
        {
            return new Rectangle()
            {
                x = rect.X,
                y = rect.Y,
                w = rect.W,
                h = rect.H,
            };
        }
        
        public World[] GetWorlds(LdtkJson json)
        {
            if (!json.Worlds.IsNullOrEmpty())
            {
                return json.Worlds;
            }
            
            return new World[] { new World 
            {
                Identifier = "World",
                Iid = json.DummyWorldIid,
                Levels = json.Levels,
                WorldLayout = json.WorldLayout.Value,
                WorldGridWidth = json.WorldGridWidth.Value,
                WorldGridHeight = json.WorldGridHeight.Value
            }};
        }
        
        private TilesetRectangle[] GetTilesetRectanglesFromField(FieldInstance field)
        {
            //if it's prebuilt via the json digger
            if (field.Value is TilesetRectangle[] arrayRects)
            {
                return arrayRects;
            }
            
            //if it was a single null value
            if (field.Value == null)
            {
                return null;
            }
            
            //if array
            if (field.Value is List<object> rectangles)
            {
                return rectangles.Select(ConvertDict).Where(p => p != null).ToArray();
            }

            //if single
            if (field.Value is Dictionary<string, object> dict)
            {
                return new TilesetRectangle[] {ConvertDict(dict)};
            }
            
            //this could be if we did a json dig to construct this array
            if (field.Value is TilesetRectangle[] rects)
            {
                return rects;
            }

            throw new Exception($"This is never supposed to be reached. {field.Identifier} {field.Value.GetType()}");
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