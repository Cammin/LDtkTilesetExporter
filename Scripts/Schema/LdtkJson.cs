// <auto-generated />
//
// To parse this JSON data, get Utf8Json then do:
//
//    using LDtkUnity;
//
//    var ldtkJson = LdtkJson.FromJson(jsonString);

namespace LDtkUnity
{
    using System;
    using System.Collections.Generic;

    using Utf8Json;
    using System.Runtime.Serialization;

    /// <summary>
    /// This file is a JSON schema of files created by LDtk level editor (https://ldtk.io).
    ///
    /// This is the root of any Project JSON file. It contains:  - the project settings, - an
    /// array of levels, - a group of definitions (that can probably be safely ignored for most
    /// users).
    /// </summary>
    public partial class LdtkJson
    {
        /// <summary>
        /// This object is not actually used by LDtk. It ONLY exists to force explicit references to
        /// all types, to make sure QuickType finds them and integrate all of them. Otherwise,
        /// Quicktype will drop types that are not explicitely used.
        /// </summary>
        [IgnoreDataMember]
        [DataMember(Name = "__FORCED_REFS")]
        public ForcedRefs ForcedRefs { get; set; }

        /// <summary>
        /// LDtk application build identifier.<br/>  This is only used to identify the LDtk version
        /// that generated this particular project file, which can be useful for specific bug fixing.
        /// Note that the build identifier is just the date of the release, so it's not unique to
        /// each user (one single global ID per LDtk public release), and as a result, completely
        /// anonymous.
        /// </summary>
        [DataMember(Name = "appBuildId")]
        public float AppBuildId { get; set; }

        /// <summary>
        /// Number of backup files to keep, if the `backupOnSave` is TRUE
        /// </summary>
        [DataMember(Name = "backupLimit")]
        public int BackupLimit { get; set; }

        /// <summary>
        /// If TRUE, an extra copy of the project will be created in a sub folder, when saving.
        /// </summary>
        [DataMember(Name = "backupOnSave")]
        public bool BackupOnSave { get; set; }

        /// <summary>
        /// Target relative path to store backup files
        /// </summary>
        [DataMember(Name = "backupRelPath")]
        public string BackupRelPath { get; set; }

        /// <summary>
        /// Project background color
        /// </summary>
        [DataMember(Name = "bgColor")]
        public string BgColor { get; set; }

        /// <summary>
        /// An array of command lines that can be ran manually by the user
        /// </summary>
        [DataMember(Name = "customCommands")]
        public LdtkCustomCommand[] CustomCommands { get; set; }

        /// <summary>
        /// Default grid size for new layers
        /// </summary>
        [DataMember(Name = "defaultGridSize")]
        public int DefaultGridSize { get; set; }

        /// <summary>
        /// Default background color of levels
        /// </summary>
        [DataMember(Name = "defaultLevelBgColor")]
        public string DefaultLevelBgColor { get; set; }

        /// <summary>
        /// **WARNING**: this field will move to the `worlds` array after the "multi-worlds" update.
        /// It will then be `null`. You can enable the Multi-worlds advanced project option to enable
        /// the change immediately.<br/><br/>  Default new level height
        /// </summary>
        [DataMember(Name = "defaultLevelHeight")]
        public int? DefaultLevelHeight { get; set; }

        /// <summary>
        /// **WARNING**: this field will move to the `worlds` array after the "multi-worlds" update.
        /// It will then be `null`. You can enable the Multi-worlds advanced project option to enable
        /// the change immediately.<br/><br/>  Default new level width
        /// </summary>
        [DataMember(Name = "defaultLevelWidth")]
        public int? DefaultLevelWidth { get; set; }

        /// <summary>
        /// Default X pivot (0 to 1) for new entities
        /// </summary>
        [DataMember(Name = "defaultPivotX")]
        public float DefaultPivotX { get; set; }

        /// <summary>
        /// Default Y pivot (0 to 1) for new entities
        /// </summary>
        [DataMember(Name = "defaultPivotY")]
        public float DefaultPivotY { get; set; }

        /// <summary>
        /// A structure containing all the definitions of this project
        /// </summary>
        [DataMember(Name = "defs")]
        public Definitions Defs { get; set; }

        /// <summary>
        /// If the project isn't in MultiWorlds mode, this is the IID of the internal "dummy" World.
        /// </summary>
        [DataMember(Name = "dummyWorldIid")]
        public string DummyWorldIid { get; set; }

        /// <summary>
        /// If TRUE, the exported PNGs will include the level background (color or image).
        /// </summary>
        [DataMember(Name = "exportLevelBg")]
        public bool ExportLevelBg { get; set; }

        /// <summary>
        /// **WARNING**: this deprecated value is no longer exported since version 0.9.3  Replaced
        /// by: `imageExportMode`
        /// </summary>
        [DataMember(Name = "exportPng")]
        public bool? ExportPng { get; set; }

        /// <summary>
        /// If TRUE, a Tiled compatible file will also be generated along with the LDtk JSON file
        /// (default is FALSE)
        /// </summary>
        [DataMember(Name = "exportTiled")]
        public bool ExportTiled { get; set; }

        /// <summary>
        /// If TRUE, one file will be saved for the project (incl. all its definitions) and one file
        /// in a sub-folder for each level.
        /// </summary>
        [DataMember(Name = "externalLevels")]
        public bool ExternalLevels { get; set; }

        /// <summary>
        /// An array containing various advanced flags (ie. options or other states). Possible
        /// values: `DiscardPreCsvIntGrid`, `ExportPreCsvIntGridFormat`, `IgnoreBackupSuggest`,
        /// `PrependIndexToLevelFileNames`, `MultiWorlds`, `UseMultilinesType`
        /// </summary>
        [DataMember(Name = "flags")]
        public Flag[] Flags { get; set; }

        /// <summary>
        /// Naming convention for Identifiers (first-letter uppercase, full uppercase etc.) Possible
        /// values: `Capitalize`, `Uppercase`, `Lowercase`, `Free`
        /// </summary>
        [DataMember(Name = "identifierStyle")]
        public IdentifierStyle IdentifierStyle { get; set; }

        /// <summary>
        /// Unique project identifier
        /// </summary>
        [DataMember(Name = "iid")]
        public string Iid { get; set; }

        /// <summary>
        /// "Image export" option when saving project. Possible values: `None`, `OneImagePerLayer`,
        /// `OneImagePerLevel`, `LayersAndLevels`
        /// </summary>
        [DataMember(Name = "imageExportMode")]
        public ImageExportMode ImageExportMode { get; set; }

        /// <summary>
        /// File format version
        /// </summary>
        [DataMember(Name = "jsonVersion")]
        public string JsonVersion { get; set; }

        /// <summary>
        /// The default naming convention for level identifiers.
        /// </summary>
        [DataMember(Name = "levelNamePattern")]
        public string LevelNamePattern { get; set; }

        /// <summary>
        /// All levels. The order of this array is only relevant in `LinearHorizontal` and
        /// `linearVertical` world layouts (see `worldLayout` value).<br/>  Otherwise, you should
        /// refer to the `worldX`,`worldY` coordinates of each Level.
        /// </summary>
        [DataMember(Name = "levels")]
        public Level[] Levels { get; set; }

        /// <summary>
        /// If TRUE, the Json is partially minified (no indentation, nor line breaks, default is
        /// FALSE)
        /// </summary>
        [DataMember(Name = "minifyJson")]
        public bool MinifyJson { get; set; }

        /// <summary>
        /// Next Unique integer ID available
        /// </summary>
        [DataMember(Name = "nextUid")]
        public int NextUid { get; set; }

        /// <summary>
        /// File naming pattern for exported PNGs
        /// </summary>
        [DataMember(Name = "pngFilePattern")]
        public string PngFilePattern { get; set; }

        /// <summary>
        /// If TRUE, a very simplified will be generated on saving, for quicker & easier engine
        /// integration.
        /// </summary>
        [DataMember(Name = "simplifiedExport")]
        public bool SimplifiedExport { get; set; }

        /// <summary>
        /// All instances of entities that have their `exportToToc` flag enabled are listed in this
        /// array.
        /// </summary>
        [DataMember(Name = "toc")]
        public LdtkTableOfContentEntry[] Toc { get; set; }

        /// <summary>
        /// This optional description is used by LDtk Samples to show up some informations and
        /// instructions.
        /// </summary>
        [DataMember(Name = "tutorialDesc")]
        public string TutorialDesc { get; set; }

        /// <summary>
        /// **WARNING**: this field will move to the `worlds` array after the "multi-worlds" update.
        /// It will then be `null`. You can enable the Multi-worlds advanced project option to enable
        /// the change immediately.<br/><br/>  Height of the world grid in pixels.
        /// </summary>
        [DataMember(Name = "worldGridHeight")]
        public int? WorldGridHeight { get; set; }

        /// <summary>
        /// **WARNING**: this field will move to the `worlds` array after the "multi-worlds" update.
        /// It will then be `null`. You can enable the Multi-worlds advanced project option to enable
        /// the change immediately.<br/><br/>  Width of the world grid in pixels.
        /// </summary>
        [DataMember(Name = "worldGridWidth")]
        public int? WorldGridWidth { get; set; }

        /// <summary>
        /// **WARNING**: this field will move to the `worlds` array after the "multi-worlds" update.
        /// It will then be `null`. You can enable the Multi-worlds advanced project option to enable
        /// the change immediately.<br/><br/>  An enum that describes how levels are organized in
        /// this project (ie. linearly or in a 2D space). Possible values: &lt;`null`&gt;, `Free`,
        /// `GridVania`, `LinearHorizontal`, `LinearVertical`
        /// </summary>
        [DataMember(Name = "worldLayout")]
        public WorldLayout? WorldLayout { get; set; }

        /// <summary>
        /// This array will be empty, unless you enable the Multi-Worlds in the project advanced
        /// settings.<br/><br/> - in current version, a LDtk project file can only contain a single
        /// world with multiple levels in it. In this case, levels and world layout related settings
        /// are stored in the root of the JSON.<br/> - with "Multi-worlds" enabled, there will be a
        /// `worlds` array in root, each world containing levels and layout settings. Basically, it's
        /// pretty much only about moving the `levels` array to the `worlds` array, along with world
        /// layout related values (eg. `worldGridWidth` etc).<br/><br/>If you want to start
        /// supporting this future update easily, please refer to this documentation:
        /// https://github.com/deepnight/ldtk/issues/231
        /// </summary>
        [DataMember(Name = "worlds")]
        public World[] Worlds { get; set; }
    }

    public partial class LdtkJson
    {
        public static LdtkJson FromJson(string json) => JsonSerializer.Deserialize<LdtkJson>(json);
    }
}