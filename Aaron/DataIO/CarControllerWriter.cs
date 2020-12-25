using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Enums;
using Aaron.Game;
using Aaron.Services;
using Aaron.Structures;
using Aaron.Utils;
using CommonServiceLocator;

namespace Aaron.DataIO
{
    public class CarControllerWriter : ChunkWriter
    {
        private readonly IProjectService _projectService;
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IPresetCarService _presetCarService;
        private readonly IPresetSkinService _presetSkinService;
        private readonly IDataTableService _dataTableService;

        public CarControllerWriter(Stream stream, bool compress) : base(stream, compress)
        {
            _projectService = ServiceLocator.Current.GetInstance<IProjectService>();
            _carService = ServiceLocator.Current.GetInstance<ICarService>();
            _carPartService = ServiceLocator.Current.GetInstance<ICarPartService>();
            _presetSkinService = ServiceLocator.Current.GetInstance<IPresetSkinService>();
            _presetCarService = ServiceLocator.Current.GetInstance<IPresetCarService>();
            _dataTableService = ServiceLocator.Current.GetInstance<IDataTableService>();
        }

        public CarControllerWriter(string file, bool compress = true) : this(
            File.Open(file, FileMode.Create, FileAccess.ReadWrite), compress)
        {
        }

        protected override void Write(IProgress<string> progress = null)
        {
            progress?.Report("Generating car array");

            BeginChunk(0x34600);
            NextAlignment(0x10);

            var cars = _carService.GetCars();
            for (var index = 0; index < cars.Count; index++)
            {
                var aaronCarRecord = cars[index];
                var cti = new CarTypeInfo
                {
                    MaxInstances = new byte[5],
                    WantToKeepLoaded = new byte[5],
                    pad4 = new byte[2],
                    MinTimeBetweenUses = new float[5],
                    AvailableSkinNumbers = new byte[10],

                    CarTypeName = aaronCarRecord.CarTypeName,
                    BaseModelName = aaronCarRecord.BaseModelName,
                    GeometryFilename = @"CARS\" + aaronCarRecord.CarTypeName + @"\GEOMETRY.BIN",
                    CarMemTypeHash = Hashing.BinHash(aaronCarRecord.UsageType.ToString()),
                    CarTypeNameHash = aaronCarRecord.BaseCarID,
                    UsageType = aaronCarRecord.UsageType,
                    ManufacturerName = aaronCarRecord.ManufacturerName,
                    DefaultSkinNumber = aaronCarRecord.DefaultSkinNumber,
                    WhatGame = '\x01',
                    WheelInnerRadiusMax = '\x14',
                    WheelInnerRadiusMin = '\x11',
                    WheelOuterRadius = '\x1a',

                    DefaultBasePaint = aaronCarRecord.DefaultBasePaint,
                    Skinnable = aaronCarRecord.Skinnable,

                    DriverRenderingOffset = new bVector3(),
                    HeadlightPosition = new bVector3(),
                    InCarSteeringWheelRenderingOffset = new bVector3(),

                    Type = index
                };

                BinaryHelpers.WriteStruct(Writer, cti);
            }

            EndChunk();

            progress?.Report("Generating AnimHookupTable");

            BeginChunk(0x34608);

            for (int i = 0; i < 123; i++)
            {
                Writer.Write((byte)0xff);
            }

            Writer.Write((byte)0x00);

            EndChunk();

            progress?.Report("Generating AnimHideTables");
            BeginChunk(0x34609);

            for (int i = 0; i < 256; i++)
            {
                Writer.Write((byte)0xff);
            }
            EndChunk();

            progress?.Report("Generating SlotTypes");
            BeginChunk(0x34607);

            Writer.Write(_projectService.GetCurrentProject().SlotOverrideData);

            foreach (var aaronCarRecord in _carService.GetCars())
            {
                if (aaronCarRecord.Spoiler == null) continue;

                var ss = new SpoilerStructure();
                ss.CarTypeNameHash = ss.CarTypeNameHash2 = aaronCarRecord.BaseCarID;
                ss.Unknown = 0x3C;
                ss.SpoilerHash = (uint)aaronCarRecord.Spoiler.SpoilerType;

                BinaryHelpers.WriteStruct(Writer, ss);
            }

            EndChunk();

            PaddingAlignment(0x8);

            this.GenerateCarPartData(progress);

            PaddingAlignment(0x10);
            DifferenceAlignment(0x8);

            progress?.Report("Writing collision data");
            BeginChunk(0x8003B900);

            foreach (var aaronCarRecord in cars)
            {
                if (aaronCarRecord.BoundsPack == null) continue;

                BeginChunk(0x3b901);

                NextAlignment(0x10);

                var boundsPack = aaronCarRecord.BoundsPack;

                var bh = new BoundsHeader
                {
                    NameHash = aaronCarRecord.CollisionHash,
                    NumBounds = boundsPack.Entries.Count
                };

                BinaryHelpers.WriteStruct(Writer, bh);

                foreach (var aaronBoundsEntry in boundsPack.Entries)
                {
                    var bounds = new Bounds
                    {
                        AttributeName = aaronBoundsEntry.AttributeName,
                        Position = aaronBoundsEntry.Position,
                        ChildIndex = aaronBoundsEntry.ChildIndex,
                        CollectionPtr = 0,
                        HalfDimensions = aaronBoundsEntry.HalfDimensions,
                        Pivot = aaronBoundsEntry.Pivot,
                        NameHash = aaronBoundsEntry.NameHash,
                        PCloudIndex = aaronBoundsEntry.PCloudIndex,
                        NumChildren = aaronBoundsEntry.NumChildren,
                        Orientation = aaronBoundsEntry.Orientation,
                        Surface = aaronBoundsEntry.Surface,
                        Flags = (ushort)aaronBoundsEntry.Flags
                    };

                    BinaryHelpers.WriteStruct(Writer, bounds);
                }

                BinaryHelpers.WriteStruct(Writer, new PCloudHeader
                {
                    NumPClouds = boundsPack.PointClouds.Count
                });

                foreach (var aaronBoundsPointCloud in boundsPack.PointClouds)
                {
                    Writer.Write(aaronBoundsPointCloud.Vertices.Count);
                    Writer.Write(0);
                    Writer.Write(0L);

                    foreach (var vertex in aaronBoundsPointCloud.Vertices)
                    {
                        BinaryHelpers.WriteStruct(Writer, vertex);
                    }
                }

                EndChunk();
            }

            EndChunk();

            PaddingAlignment(0x10);

            progress?.Report("Writing preset cars");

            BeginChunk(0x30220);

            foreach (var aaronPresetCar in _presetCarService.GetPresetCars())
            {
                BinaryHelpers.WriteStruct(Writer, PresetConverter.ConvertAaronPresetToFEPreset(aaronPresetCar));
            }

            EndChunk();

            PaddingAlignment(0x10);

            progress?.Report("Writing preset skins");
            BeginChunk(0x30250);

            foreach (var aaronPresetSkinRecord in _presetSkinService.GetPresetSkins())
            {
                var ps = new PresetSkin
                {
                    Null = new byte[0x8],
                    Null3 = new byte[0x38],

                    PresetName = aaronPresetSkinRecord.PresetName,
                    PaintGroup = aaronPresetSkinRecord.PaintGroup,
                    PaintHue = aaronPresetSkinRecord.PaintHue,
                    PaintVariance = aaronPresetSkinRecord.Variance,
                    PaintSaturation = aaronPresetSkinRecord.Saturation,
                    VinylHash = 0xFFFFFFFFu
                };

                BinaryHelpers.WriteStruct(Writer, ps);
            }

            EndChunk();

            progress?.Report("Writing data tables");

            foreach (var aaronDataTable in _dataTableService.GetDataTables())
            {
                PaddingAlignment(0x10);
                BeginChunk(0x3CE14);
                NextAlignment(0x10);

                Writer.Write(aaronDataTable.Hash);

                foreach (var aaronDataTableEntry in aaronDataTable.Entries)
                {
                    Writer.Write(aaronDataTableEntry.Hash);
                    Writer.Write(aaronDataTableEntry.Unknown);
                    Writer.Write(aaronDataTableEntry.Unknown2);
                }

                EndChunk();
            }

            progress?.Report("Post-processing");

            //BeginChunk(0);

            //var watermarkStr = "File generated by Aaron+ (commit hash: " + VersionReader.AppVersion + ") on ";
            //watermarkStr += DateTime.Now.ToShortDateString();
            //watermarkStr += " @ " + DateTime.Now.ToShortTimeString();
            //watermarkStr += " | Created by heyitsleo";

            //Writer.Write(watermarkStr);

            //EndChunk();
        }

        private void GenerateCarPartData(IProgress<string> progress = null)
        {
            progress?.Report("Generating car part data");

            // The index table is structured in 3 steps:
            //      1. Take the part array as it would appear in [04 46 03 00]
            //      2. Sort the array by part hash
            //      3. Sort the sorted array by the index of each part in the original array
            // aka .OrderBy().ThenBy()

            // The list of car part collections, ordered in ascending order by their hash.
            var sortedCollectionList = _carPartService.GetCarPartCollections()
                .DistinctBy(c => c.Hash)
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Hash)
                .ToList();

            // The entire list of car parts, ordered as they would appear in [04 46 03 00].
            var partArray = sortedCollectionList.SelectMany(c => c.Parts).ToList();

            progress?.Report("Generating part index table");

            BeginChunk(0x3460E);

            {
                // The mapping of part object->array index.
                Dictionary<AaronCarPartRecord, int> carPartIndexDictionary = new Dictionary<AaronCarPartRecord, int>();

                for (int i = 0; i < partArray.Count; i++)
                {
                    carPartIndexDictionary.Add(partArray[i], i);
                }

                foreach (var partPair in carPartIndexDictionary.OrderBy(pair => pair.Key.Hash).ThenBy(pair => pair.Value))
                {
                    Writer.Write(partPair.Key.Hash);
                    Writer.Write(partPair.Value);
                }
            }

            EndChunk();

            progress?.Report("Generating CARINFO_CARPART chunk");

            BeginChunk(0x80034602);

            {
                // The list of all part attributes.
                var attributes = partArray.OrderBy(p => p.Hash).SelectMany(p => p.Attributes).ToList();
                // The list of UNIQUE part attributes.
                var distinctAttributes = attributes.DistinctBy(a => a.GetHashCode()).ToList();
                // The list of racer cars.
                var racerList = _carService.GetCarsByType(CarUsageType.Racing);
                // The list of strings.
                var strings = _carPartService.GetStrings()
                    .Concat(distinctAttributes.SelectMany(a => a.Strings))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .ToList();

                {
                    // Generate header
                    BeginChunk(0x34603);
                    Stream.Seek(8, SeekOrigin.Current);

                    var carPartPack = new CarPartPack();
                    carPartPack.Version = 6;
                    carPartPack.NumParts = partArray.Count; // [04 46 03 00]
                    carPartPack.NumAttributes = distinctAttributes.Count; // [05 46 03 00]
                    carPartPack.NumModelTables = racerList.Count; // [0A 46 03 00]
                    carPartPack.NumTypeNames = sortedCollectionList.Count; // [0B 46 03 00]

                    BinaryHelpers.WriteStruct(Writer, carPartPack);

                    EndChunk();
                }

                // A mapping of string hash codes to relative offsets within the string table.
                Dictionary<int, int> stringOffsets = new Dictionary<int, int>();

                {
                    // Generate string table
                    BeginChunk(0x34606);

                    var startChunkPos = Stream.Position;

                    foreach (var s in strings)
                    {
                        var hash = s.GetHashCode();

                        var streamPosition = (int)(Stream.Position - startChunkPos);

                        if (streamPosition % 4 != 0)
                        {
                            throw new Exception("Something went horribly wrong");
                        }

                        stringOffsets.Add(hash, streamPosition);

                        Writer.WriteAlignedString(s);
                    }

                    EndChunk();
                }

                // A mapping of attribute hash codes to offsets within the attribute table.
                Dictionary<int, int> attributeIndexDictionary = new Dictionary<int, int>();

                for (var index = 0; index < distinctAttributes.Count; index++)
                {
                    if (index > ushort.MaxValue)
                        throw new Exception("Congratulations. You've managed to have too many unique attributes defined. Good job! Time to start deleting things.");
                    var aaronCarPartAttribute = distinctAttributes[index];
                    attributeIndexDictionary.Add(aaronCarPartAttribute.GetHashCode(), index);
                }

                Dictionary<uint, int> partTags = new Dictionary<uint, int>();
                var hashToTagMap = new Dictionary<int, int>();
                var hashToAttribsMap = new Dictionary<int, List<AaronCarPartAttribute>>();
                var partTag = 0;
                foreach (var part in partArray)
                {
                    var attribListCode = AttributeListHashCode(part.Attributes);

                    if (!hashToTagMap.ContainsKey(attribListCode))
                    {
                        hashToTagMap[attribListCode] = partTag;
                        partTag += part.Attributes.Count + 1;
                    }

                    partTags[part.Hash] = hashToTagMap[attribListCode];
                    hashToAttribsMap[attribListCode] = new List<AaronCarPartAttribute>(part.Attributes);
                }

                {
                    // Generate attribute offset tables
                    BeginChunk(0x3460C);
                    var distinctParts = partArray.DistinctBy(p => p.Hash).ToList();
                    ISet<int> written = new HashSet<int>();
                    foreach (var part in distinctParts)
                    {
                        var hashcode = AttributeListHashCode(part.Attributes);

                        if (written.Add(hashcode))
                        {
                            Writer.Write((ushort)part.Attributes.Count);

                            foreach (var attribute in part.Attributes)
                                Writer.Write((ushort)attributeIndexDictionary[attribute.GetHashCode()]);
                        }
                    }
                    //foreach (var aaronCarPartRecord in distinctParts)
                    //{
                    //    partTags[aaronCarPartRecord.Hash] = partTag;
                    //    Writer.Write((ushort)aaronCarPartRecord.Attributes.Count);

                    //    foreach (var aaronCarPartAttribute in aaronCarPartRecord.Attributes)
                    //    {
                    //        Writer.Write((ushort)attributeIndexDictionary[aaronCarPartAttribute.GetHashCode()]);
                    //    }

                    //    partTag += aaronCarPartRecord.Attributes.Count + 1;
                    //}
                    NextAlignment(4);
                    EndChunk();
                }

                {
                    // Generate attributes table
                    BeginChunk(0x34605);

                    foreach (var aaronCarPartAttribute in distinctAttributes)
                    {
                        var newAttrib = new CarPartAttribute();
                        newAttrib.NameHash = aaronCarPartAttribute.Hash;

                        switch (newAttrib.NameHash)
                        {
                            case 0xB1027477:
                            case 0x46B79643:
                            case 0xFD35FE70:
                            case 0x7D65A926:
                                if (aaronCarPartAttribute.Strings.Count > 1)
                                {
                                    throw new Exception("Invalid string reference attribute data!");
                                }

                                if (aaronCarPartAttribute.Strings.Count == 0 || aaronCarPartAttribute.Strings[0] == "")
                                {
                                    newAttrib.iParam = -1;
                                }
                                else
                                {
                                    newAttrib.iParam = (int)(stringOffsets[aaronCarPartAttribute.Strings[0].GetHashCode()] / 4);
                                }
                                //newAttrib.iParam = stringOffsetTable[]
                                //attribList[i].Strings.Add(AaronCarPartManager.Get().PartNames[attribList[i].Data.iParam * 4]);
                                break;
                            case 0xFE613B98:
                                ushort offs1 = 0xFFFF;
                                ushort offs2 = 0xFFFF;

                                if (aaronCarPartAttribute.Strings.Count > 0 && aaronCarPartAttribute.Strings[0] != "")
                                {
                                    int offs1i = stringOffsets[aaronCarPartAttribute.Strings[0].GetHashCode()] >> 2;

                                    if (offs1i > ushort.MaxValue)
                                    {
                                        throw new Exception("string 1 out of bounds");
                                    }

                                    offs1 = (ushort)offs1i;
                                }
                                if (aaronCarPartAttribute.Strings.Count > 1 && aaronCarPartAttribute.Strings[1] != "")
                                {
                                    int offs2i = stringOffsets[aaronCarPartAttribute.Strings[1].GetHashCode()] >> 2;

                                    if (offs2i > ushort.MaxValue)
                                    {
                                        throw new Exception("string 2 out of bounds");
                                    }

                                    offs2 = (ushort)offs2i;
                                }
                                newAttrib.iParam = (offs2 << 16) | offs1;

                                //attribList[i].Strings.Add(AaronCarPartManager.Get().PartNames[(attribList[i].Data.iParam & 0xFFFF) * 4]);
                                //attribList[i].Strings.Add(AaronCarPartManager.Get().PartNames[((attribList[i].Data.iParam >> 16) & 0xFFFF) * 4]);
                                break;
                            case 0x8C185134:
                                if (aaronCarPartAttribute.Value is string s)
                                {
                                    newAttrib.uParam = Hashing.FilteredBinHash(s);
                                }
                                else
                                {
                                    newAttrib.uParam = Convert.ToUInt32(aaronCarPartAttribute.Value);
                                }
                                break;
                            case 0x9239CF16:
                                newAttrib.uParam = Convert.ToUInt16(aaronCarPartAttribute.Value);
                                //CarPartID cpi = CarPartID.TryParse()
                                //if (Enum.TryParse((string) aaronCarPartAttribute.Value, out CarPartID cpi))
                                //{
                                //    ushort us = (ushort) cpi;

                                //    newAttrib.iParam = us << 8;
                                //}
                                //else
                                //{
                                //    throw new Exception();
                                //}
                                break;
                            default:
                                if (aaronCarPartAttribute.Value is bool b)
                                {
                                    newAttrib.iParam = b ? 1 : 0;
                                }
                                else if (aaronCarPartAttribute.Value is long u)
                                {
                                    newAttrib.uParam = unchecked((uint)u);
                                }
                                else if (aaronCarPartAttribute.Value is double f)
                                {
                                    newAttrib.fParam = Convert.ToSingle(f);
                                }
                                else
                                {
                                    throw new Exception();
                                }

                                break;
                        }
                        BinaryHelpers.WriteStruct(Writer, newAttrib);
                    }

                    EndChunk();
                }

                {
                    // Generate interior data
                    BeginChunk(0x3460A);

                    {
                        var lodLevels = new[] { 'A', 'B', 'C', 'D', 'E' };
                        foreach (var aaronCarRecord in racerList)
                        {
                            Writer.Write(0xFFFF0000);

                            foreach (var lodLevel in lodLevels)
                            {
                                var str = $"{aaronCarRecord.CarTypeName}_KIT00_INTERIORHI_{lodLevel}";

                                Writer.Write(Hashing.BinHash(str));

                                for (int i = 0; i <= 10; i++)
                                {
                                    Writer.Write(0xFFFFFFFF);
                                }
                            }
                        }
                    }

                    EndChunk();
                }

                {
                    // Generate collection hash list
                    BeginChunk(0x3460B);
                    foreach (var aaronCarPartCollection in sortedCollectionList)
                    {
                        Writer.Write(aaronCarPartCollection.Hash);
                    }
                    EndChunk();
                }

                {
                    // Generate car part array
                    BeginChunk(0x34604);

                    for (var index = 0; index < sortedCollectionList.Count; index++)
                    {
                        var aaronCarPartCollection = sortedCollectionList[index];
                        foreach (var aaronCarPartRecord in aaronCarPartCollection.Parts)
                        {
                            var dbcp = new DBCarPart();
                            dbcp.CarIndex = (short)index;
                            dbcp.Hash = aaronCarPartRecord.Hash;
                            dbcp.AttributeTableOffset = partTags[dbcp.Hash];
                            BinaryHelpers.WriteStruct(Writer, dbcp);
                        }
                    }

                    EndChunk();
                }
            }

            EndChunk();
        }
        private int AttributeListHashCode(IEnumerable<AaronCarPartAttribute> attributes)
        {
            var res = 0x2D2816FE;
            foreach (var item in attributes) res = res * 31 + item.GetHashCode();
            return res;
        }
    }
}
