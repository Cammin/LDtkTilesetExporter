using System;
using System.Collections.Generic;
using System.IO;
using Utf8Json;

namespace ExportTilesetDefinition
{
    internal static class JsonDigger
    {
        private delegate bool JsonDigAction<T>(ref JsonReader reader, ref T result);
        
        /// <summary>
        /// This gets used field tiles from BOTH levels and entity layers, as both arrays are named "fieldInstances"
        /// </summary>
        public static bool GetUsedFieldTiles(string levelOrProjectPath, ref List<FieldInstance> result) => 
            DigIntoJson(levelOrProjectPath, GetUsedFieldTilesReader, ref result);

        private static bool DigIntoJson<T>(string path, JsonDigAction<T> digAction, ref T result)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Couldn't locate the file to dig into the json for at path: \"{path}\"");
                return false;
            }
            
            byte[] bytes = File.ReadAllBytes(path);
            JsonReader reader = new JsonReader(bytes);
            bool success = digAction.Invoke(ref reader, ref result);

            if (success)
            {
                //Debug.Log($"Dug json and got {result} for {actionThing.Method.Name} at {path}");
                return true;
            }
            
            Console.WriteLine($"Issue digging into the json for {path}");
            return false;
        }
        
        private static bool GetUsedFieldTilesReader(ref JsonReader reader, ref List<FieldInstance> result)
        {
            //a field instance (1.2.5): { "__identifier": "integer", "__value": 12345, "__type": "Int", "__tile": null, "defUid": 105, "realEditorValues": [{ "id": "V_Int", "params": [12345] }] },
            //a field instance (1.3.0): { "__identifier": "integer", "__type": "Int", "__value": 12345, "__tile": null, "defUid": 105, "realEditorValues": [{ "id": "V_Int", "params": [12345] }] },
            //"fieldInstances": [{ "__identifier": "LevelTile", "__value": { "tilesetUid": 149, "x": 96, "y": 32, "w": 32, "h": 16 }, "__type": "Tile", "__tile": { "tilesetUid": 149, "x": 96, "y": 32, "w": 32, "h": 16 }, "defUid": 164, "realEditorValues": [{"id": "V_String","params": ["96,32,32,16"]}] }]
            while (reader.Read())
            {
                if (!reader.ReadIsPropertyName("fieldInstances"))
                {
                    continue;
                }
                
                Assert.IsTrue(reader.GetCurrentJsonToken() == JsonToken.BeginArray);
                
                int arrayDepth = 0;

                while (reader.IsInArray(ref arrayDepth)) //fieldInstances array. 
                {
                    //in case we're in the next object
                    reader.ReadIsValueSeparator();
                    
                    Assert.IsTrue(reader.GetCurrentJsonToken() == JsonToken.BeginObject);
                    int fieldInstanceObjectDepth = 0;
                    while (reader.IsInObject(ref fieldInstanceObjectDepth))
                    {
                        FieldInstance field = new FieldInstance();
                
                        //_identifier
                        Assert.IsTrue(reader.ReadIsPropertyName("__identifier"));
                        field.Identifier = reader.ReadString();
                        Assert.IsTrue(reader.ReadIsValueSeparator());
                        
                        //__type
                        reader.ReadIsPropertyName("__type");
                        field.Type = reader.ReadString();
                        if (field.Type != "Tile" && field.Type != "Array<Tile>")
                        {
                            reader.ReadToObjectEnd(fieldInstanceObjectDepth);
                            break;
                        }
                        Assert.IsTrue(reader.ReadIsValueSeparator());
                        
                        //__value
                        Assert.IsTrue(reader.ReadIsPropertyName("__value"));

                        //start object or start array, or null. if it's null, and in which that case, then we're done digging in this one.
                        if (reader.ReadIsNull())
                        {
                            reader.ReadToObjectEnd(fieldInstanceObjectDepth);
                            break;
                        }
                        
                        List<TilesetRectangle> rects = new List<TilesetRectangle>();
                        bool isArray = reader.GetCurrentJsonToken() == JsonToken.BeginArray;
                        if (isArray)
                        {
                            int valueArrayDepth = 0;
                            while (reader.IsInArray(ref valueArrayDepth))
                            {
                                reader.ReadIsValueSeparator();
                                ReadTilesetRectangleObject(ref reader);
                            }
                        }
                        else
                        {
                            ReadTilesetRectangleObject(ref reader);
                        }
                        field.Value = rects.ToArray();
                        
                        void ReadTilesetRectangleObject(ref JsonReader readerInScope)
                        {
                            if (readerInScope.ReadIsNull())
                            {
                                return;
                            }
                            
                            if (!readerInScope.ReadIsBeginObject())
                            {
                                readerInScope.ReadNext();
                                return;
                            }
                            
                            //tilesetUid. do a check here because it could be the point field
                            Assert.IsTrue(readerInScope.GetCurrentJsonToken() == JsonToken.String);
                            if (readerInScope.ReadString() != "tilesetUid")
                            {
                                readerInScope.ReadToObjectEnd(1);
                                return;
                            }
                            TilesetRectangle rect = new TilesetRectangle();
                            Assert.IsTrue(readerInScope.ReadIsNameSeparator());
                            
                            rect.TilesetUid = readerInScope.ReadInt32();
                            Assert.IsTrue(readerInScope.ReadIsValueSeparator());

                            //x
                            Assert.IsTrue(readerInScope.ReadIsPropertyName("x"));
                            rect.X = readerInScope.ReadInt32();
                            Assert.IsTrue(readerInScope.ReadIsValueSeparator());

                            //y
                            Assert.IsTrue(readerInScope.ReadIsPropertyName("y"));
                            rect.Y = readerInScope.ReadInt32();
                            Assert.IsTrue(readerInScope.ReadIsValueSeparator());

                            //w
                            Assert.IsTrue(readerInScope.ReadIsPropertyName("w"));
                            rect.W = readerInScope.ReadInt32();
                            Assert.IsTrue(readerInScope.ReadIsValueSeparator());

                            //h
                            Assert.IsTrue(readerInScope.ReadIsPropertyName("h"));
                            rect.H = readerInScope.ReadInt32();
                            
                            readerInScope.ReadIsEndObjectWithVerify();
                            rects.Add(rect);
                        }

                        //defUid
                        reader.ReadUntilPropertyName("defUid");
                        field.DefUid = reader.ReadInt32();
                        
                        result.Add(field);
                        reader.ReadToObjectEnd(fieldInstanceObjectDepth);
                        break;
                    }
                }
            }
            return true;
        }
    }
}