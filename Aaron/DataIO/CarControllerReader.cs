using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aaron.Data;
using Aaron.Data.Bounds;
using Aaron.Enums;
using Aaron.Game;
using Aaron.Services;
using Aaron.Structures;
using Aaron.Utils;
using GalaSoft.MvvmLight.Ioc;

namespace Aaron.DataIO
{
    /// <summary>
    /// Reads data from the GlobalC.lzc file.
    /// </summary>
    public class CarControllerReader : ChunkReader
    {
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IPresetCarService _presetCarService;
        private readonly IPresetSkinService _presetSkinService;
        private readonly IDataTableService _dataTableService;

        private readonly Dictionary<long, AttributeOffsetTable> _attributeOffsetTables = new Dictionary<long, AttributeOffsetTable>();
        private readonly List<CarPartAttribute> _attributes = new List<CarPartAttribute>();
        private readonly Dictionary<long, string> _stringOffsetDictionary = new Dictionary<long, string>();

        private CarControllerReader(Stream stream) : base(stream)
        {
            _carService = SimpleIoc.Default.GetInstance<ICarService>();
            _carPartService = SimpleIoc.Default.GetInstance<ICarPartService>();
            _presetCarService = SimpleIoc.Default.GetInstance<IPresetCarService>();
            _presetSkinService = SimpleIoc.Default.GetInstance<IPresetSkinService>();
            _dataTableService = SimpleIoc.Default.GetInstance<IDataTableService>();
        }

        public CarControllerReader(string file) : this(File.OpenRead(file))
        {
        }

        protected override void PrepareStream()
        {
            SetStream(new MemoryStream(BlockCompression.ReadBlockFile(Stream)));
        }

        protected override void ProcessChunk(Chunk chunk)
        {
            if (chunk.Offset == 0 && chunk.Type != 0x34600)
            {
                throw new InvalidDataException("Expected first chunk to be BCHUNK_CARINFO_ARRAY");
            }

            //Debug.WriteLine("@{0:X}: {1:X} ({2} bytes)", chunk.Offset, chunk.Type, chunk.Size);

            switch (chunk.Type)
            {
                case 0x34600:
                    this.ProcessCarList(chunk);
                    break;
                case 0x34607:
                    this.ProcessSlotOverrideData(chunk);
                    break;
                // car part stuff
                case 0x34606:
                    this.ProcessStringTable(chunk);
                    break;
                case 0x3460C:
                    this.ProcessAttributeOffsetTables(chunk);
                    break;
                case 0x34605:
                    this.ProcessAttributes(chunk);
                    break;
                case 0x3460A:
                    // interior data
                    break;
                case 0x3460B:
                    this.ProcessCarHashList(chunk);
                    break;
                case 0x34604:
                    this.ProcessCarPartTable(chunk);
                    break;
                // other stuff
                case 0x30220:
                    this.ProcessPresetCars(chunk);
                    break;
                case 0x30250:
                    this.ProcessPresetSkins(chunk);
                    break;
                case 0x3B901:
                    this.ProcessBoundsPack(chunk);
                    break;
                case 0x3CE14:
                    this.ProcessDataTable(chunk);
                    break;
                default:
                    Debug.WriteLine("Unhandled chunk: 0x{0:X} ({1} bytes) @ 0x{2:X}", chunk.Type, chunk.Size, chunk.Offset);
                    break;
            }

            if (Stream.Position > chunk.EndOffset)
            {
                throw new Exception("went past chunk");
            }
        }

        private void ProcessDataTable(Chunk chunk)
        {
            Progress?.Report("Processing data table");
            Stream.Align(0x10);

            var table = new AaronDataTable();
            table.Name = HashResolver.Resolve(Reader.ReadUInt32());
            table.Entries = new List<AaronDataTableEntry>();

            while (Stream.Position < chunk.EndOffset)
            {
                var entry = new AaronDataTableEntry();
                entry.Name = HashResolver.Resolve(Reader.ReadUInt32());
                entry.Unknown = Reader.ReadUInt32();
                entry.Unknown2 = Reader.ReadSingle();

                table.Entries.Add(entry);
            }

            _dataTableService.AddDataTable(table);
        }

        private void ProcessSlotOverrideData(Chunk chunk)
        {
            if ((chunk.Size - 3444) % 0x24 != 0)
            {
                throw new InvalidDataException("BCHUNK_CARINFO_SLOTTYPES is invalid!");
            }

            Progress?.Report("Processing slot override data");

            _carPartService.SetSlotOverrideData(Reader.ReadBytes(3444));

            while (Stream.Position < chunk.EndOffset)
            {
                var ss = BinaryHelpers.ReadStruct<SpoilerStructure>(Reader);
                var car = _carService.FindCarByHash(ss.CarTypeNameHash);

                car.Spoiler = new AaronSpoilerRecord
                {
                    CarTypeNameHash = ss.CarTypeNameHash,
                    SpoilerType = (AaronSpoilerType)ss.SpoilerHash
                };
            }
        }

        private void ProcessPresetSkins(Chunk chunk)
        {
            Progress?.Report("Processing preset skins");
            while (Stream.Position < chunk.EndOffset)
            {
                var pss = BinaryHelpers.ReadStruct<PresetSkin>(Reader);
                var ps = new AaronPresetSkinRecord();
                ps.PresetName = pss.PresetName;
                ps.PaintGroup = pss.PaintGroup;
                ps.PaintHue = pss.PaintHue;
                ps.Saturation = pss.PaintSaturation;
                ps.Variance = pss.PaintVariance;

                _presetSkinService.AddPresetSkin(ps);
            }
        }

        private void ProcessBoundsPack(Chunk chunk)
        {
            Progress?.Report("Processing car bounds pack");
            Stream.Align(0x10);

            var boundsHeader = BinaryHelpers.ReadStruct<BoundsHeader>(Reader);
            var car = _carService.FindCarByCollisionHash(boundsHeader.NameHash);

            if (car.BoundsPack != null)
            {
                throw new InvalidDataException("Duplicate bounds pack for " + car.CarTypeName);
            }

            var bp = new AaronBoundsPack
            {
                Entries = new List<AaronBoundsEntry>(boundsHeader.NumBounds),
                NameHash = boundsHeader.NameHash,
                PointClouds = new List<AaronBoundsPointCloud>()
            };

            for (var i = 0; i < boundsHeader.NumBounds; i++)
            {
                var boundsStruct = BinaryHelpers.ReadStruct<Bounds>(Reader);

                Debug.Assert(boundsStruct.CollectionPtr == 0);

                var boundsEntry = new AaronBoundsEntry
                {
                    Position = boundsStruct.Position,
                    AttributeName = boundsStruct.AttributeName,
                    ChildIndex = boundsStruct.ChildIndex,
                    Flags = (AaronBoundsFlags)boundsStruct.Flags,
                    HalfDimensions = boundsStruct.HalfDimensions,
                    NameHash = boundsStruct.NameHash,
                    NumChildren = boundsStruct.NumChildren,
                    Orientation = boundsStruct.Orientation,
                    PCloudIndex = boundsStruct.PCloudIndex,
                    Pivot = boundsStruct.Pivot,
                    Surface = boundsStruct.Surface
                };

                bp.Entries.Add(boundsEntry);
            }

            var pointCloudHeader = BinaryHelpers.ReadStruct<PCloudHeader>(Reader);

            bp.PointClouds = new List<AaronBoundsPointCloud>(pointCloudHeader.NumPClouds);

            for (var i = 0; i < pointCloudHeader.NumPClouds; i++)
            {
                int numPoints = Reader.ReadInt32();
                Reader.ReadInt64(); // fPad
                Reader.ReadUInt32(); // fPList

                var pointCloud = new AaronBoundsPointCloud
                {
                    Vertices = new List<Vector4>(numPoints)
                };

                for (var j = 0; j < numPoints; j++)
                {
                    var vertex = BinaryHelpers.ReadStruct<Vector4>(Reader);

                    pointCloud.Vertices.Add(vertex);
                }

                bp.PointClouds.Add(pointCloud);
            }

            car.BoundsPack = bp;
        }

        private void ProcessPresetCars(Chunk chunk)
        {
            Progress?.Report("Processing preset cars");
            while (Stream.Position < chunk.EndOffset)
            {
                var fePresetCar = BinaryHelpers.ReadStruct<FEPresetCar>(Reader);

                _presetCarService.AddPresetCar(PresetConverter.ConvertFeToAaronPresetCar(fePresetCar));
            }
        }

        private void ProcessStringTable(Chunk chunk)
        {
            if (chunk.Size % 4 != 0)
            {
                throw new InvalidDataException("malformed string table chunk");
            }

            Progress?.Report("Processing string table");

            while (Stream.Position < chunk.EndOffset)
            {
                var streamPosition = Stream.Position - (chunk.Offset + 8);
                var alignedString = Stream.ReadAlignedString();
                _stringOffsetDictionary[streamPosition] = alignedString;
                _carPartService.AddString(alignedString);
            }
        }

        private void ProcessCarHashList(Chunk chunk)
        {
            if (chunk.Size % 4 != 0)
            {
                throw new InvalidDataException("malformed car hash list chunk");
            }

            Progress?.Report("Processing car part collection hash list");

            while (Stream.Position < chunk.EndOffset)
            {
                AaronCarPartCollection carPartCollection = new AaronCarPartCollection();
                carPartCollection.Name = HashResolver.Resolve(Reader.ReadUInt32());
                carPartCollection.Parts = new SynchronizedObservableCollection<AaronCarPartRecord>();

                this._carPartService.AddCarPartCollection(carPartCollection);
            }
        }

        private static readonly HashSet<CarPartAttribute> trackedUnknownAttributes = new HashSet<CarPartAttribute>();

        private void ProcessAttributes(Chunk chunk)
        {
            trackedUnknownAttributes.Clear();
            Progress?.Report("Processing attributes table");
            while (Stream.Position < chunk.EndOffset)
            {
                var carPartAttribute = BinaryHelpers.ReadStruct<CarPartAttribute>(Reader);
                _attributes.Add(carPartAttribute);
            }
        }

        private void ProcessAttributeOffsetTables(Chunk chunk)
        {
            Progress?.Report("Processing attribute offset tables");
            while (Stream.Position < chunk.EndOffset)
            {
                var table = new AttributeOffsetTable();
                table.Offset = (Stream.Position - (chunk.Offset + 8)) / 2;
                var numOffsets = Reader.ReadUInt16();
                table.Offsets = new List<ushort>(numOffsets);

                for (int i = 0; i < numOffsets; i++)
                {
                    table.Offsets.Add(Reader.ReadUInt16());
                }

                _attributeOffsetTables[table.Offset] = table;
            }
        }

        private void ProcessCarPartTable(Chunk chunk)
        {
            if (chunk.Size % 0xC != 0)
            {
                throw new InvalidDataException("Malformed car part table chunk");
            }

            DebugTiming.BeginTiming("ProcessCarPartTable");

            Progress?.Report("Processing car parts");

            List<AaronCarPartAttribute> attributes = new List<AaronCarPartAttribute>();

            var allCollections = _carPartService.GetCarPartCollections();
            Dictionary<int, AaronCarPartCollection> collectionsDict =
                allCollections.ToDictionary(c => allCollections.IndexOf(c), c => c);

            while (Stream.Position < chunk.EndOffset)
            {
                DBCarPart dbCarPart = BinaryHelpers.ReadStruct<DBCarPart>(Reader);

                var part = new AaronCarPartRecord();
                part.Name = HashResolver.Resolve(dbCarPart.Hash);
                part.Attributes = this.PrepareAttributes(dbCarPart);

                attributes.AddRange(part.Attributes);
                //_carPartService.GetCarPartCollections()
                collectionsDict[dbCarPart.CarIndex].Parts.Add(part);
                //_carPartService.GetCarPartCollectionByIndex(dbCarPart.CarIndex).Parts.Add(part);
            }


            var uniqueAttribs = attributes.DistinctBy(a => a.GetHashCode()).Count();

            DebugTiming.EndTiming("ProcessCarPartTable");

            // we can discard internal attribute data now
            _attributeOffsetTables.Clear();
            _attributes.Clear();
        }

        private SynchronizedObservableCollection<AaronCarPartAttribute> PrepareAttributes(DBCarPart dbCarPart)
        {
            SynchronizedObservableCollection<AaronCarPartAttribute> attributes = new SynchronizedObservableCollection<AaronCarPartAttribute>();
            AttributeOffsetTable attributeOffsetTable = _attributeOffsetTables[dbCarPart.AttributeTableOffset];

            foreach (var offset in attributeOffsetTable.Offsets)
            {
                var rawAttribute = _attributes[offset];
                AaronCarPartAttribute newAttribute = this.ConvertAttribute(rawAttribute);
                //AaronCarPartAttribute newAttribute = new AaronCarPartAttribute();
                //newAttribute.Name = HashResolver.Resolve(rawAttribute.NameHash);
                //newAttribute.Value = rawAttribute.iParam;


                //switch (newAttribute.Hash)
                //{
                //    case 0xB1027477:
                //    case 0x46B79643:
                //    case 0xFD35FE70:
                //    case 0x7D65A926:
                //        this.LoadSingleAttribString(newAttribute, rawAttribute);
                //        break;
                //    case 0xFE613B98:
                //        this.LoadDoubleAttribString(newAttribute, rawAttribute);
                //        break;
                //}

                attributes.Add(newAttribute);
            }

            return attributes;
        }

        private static HashSet<uint> trackedUpgradeGroups = new HashSet<uint>();

        private AaronCarPartAttribute ConvertAttribute(CarPartAttribute rawAttribute)
        {
            AaronCarPartAttribute attribute = new AaronCarPartAttribute();
            attribute.Name = HashResolver.Resolve(rawAttribute.NameHash);

            switch (attribute.Name)
            {
                case "TEXTURE":
                case "LOD_CHARACTERS_OFFSET":
                case "NAME_OFFSET":
                    this.LoadSingleAttribString(attribute, rawAttribute);
                    break;
                case "LOD_BASE_NAME":
                    this.LoadDoubleAttribString(attribute, rawAttribute);
                    break;
                default:
                    //attribute.Value = rawAttribute.iParam;
                    this.SetAttributeValue(attribute, rawAttribute);
                    break;
            }
            return attribute;
        }

        private void SetAttributeValue(AaronCarPartAttribute attribute, CarPartAttribute rawAttribute)
        {
            switch (attribute.Name)
            {
                case "LOD_NAME_PREFIX_SELECTOR":
                case "MAX_LOD":
                case "CV":
                case "LANGUAGEHASH":
                case "KITNUMBER":
                case "MODEL_TABLE_OFFSET":
                case "MORPHTARGET_NUM":
                case "0xE80A3B62":
                case "0xCE7D8DB5":
                case "0xEB0101E2":
                case "0x7D29CF3E":
                case "0xEBB03E66":
                case "RED":
                case "GREEN":
                case "BLUE":
                case "GLOSS":
                case "MATTE":
                case "0x6BA02C05":
                case "SWATCH":
                case "0xD68A7BAB":
                case "PAINTGROUP":
                case "MAT0":
                case "MAT1":
                case "MAT2":
                case "MAT3":
                case "MAT4":
                case "MAT5":
                case "MAT6":
                case "MAT7":
                case "TEXTUREHASH":
                case "0xC9818DFC":
                case "MATNAMEA":
                case "MATNAMEB":
                case "DAMAGELEVEL":
                case "0x04B39858":
                case "GROUPLANGUAGEHASH":
                case "0x5412A1D9":
                    attribute.Value = rawAttribute.uParam;
                    break;
                case "PARTID_UPGRADE_GROUP":
                    if (trackedUpgradeGroups.Add(rawAttribute.uParam))
                    {
                        ushort sp = (ushort) rawAttribute.uParam;
                        Debug.WriteLine("PARTID_UPGRADE_GROUP: {0} ({1})", Convert.ToString(sp & 0xff, 2).PadLeft(8, '0'), rawAttribute.uParam);
                        Debug.WriteLine((CarPartID) (sp >> 8));
                    }
                    goto default;
                //    attribute.Value = (CarPartID)(rawAttribute.iParam >> 8);
                //    break;
                case "STOCK":
                    attribute.Value = rawAttribute.iParam == 1;
                    break;
                case "BLEND":
                    attribute.Value = rawAttribute.fParam;
                    break;
                default:
                    //if (trackedUnknownAttributes.Add(rawAttribute))
                    //{
                    //    Debug.WriteLine("Unknown attribute: {0} U {1} I {2} F {3}", attribute.Name, rawAttribute.uParam, rawAttribute.iParam, rawAttribute.fParam);
                    //}
                    attribute.Value = rawAttribute.uParam;
                    break;
            }
        }

        private void LoadDoubleAttribString(AaronCarPartAttribute attribute, CarPartAttribute rawAttribute)
        {
            ushort component1 = (ushort)(rawAttribute.uParam & 0xFFFF);
            ushort component2 = (ushort)((rawAttribute.uParam >> 16) & 0xFFFF);

            if (component1 != 0xFFFFu)
            {
                attribute.Strings.Add(_stringOffsetDictionary[component1 * 4]);
            }
            else
            {
                attribute.Strings.Add("");
            }

            if (component2 != 0xFFFFu)
            {
                attribute.Strings.Add(_stringOffsetDictionary[component2 * 4]);
            }
            else
            {
                attribute.Strings.Add("");
            }
        }

        private void LoadSingleAttribString(AaronCarPartAttribute attribute, CarPartAttribute rawAttribute)
        {
            if (rawAttribute.uParam != 0xFFFFFFFF)
            {
                var str = _stringOffsetDictionary[rawAttribute.uParam * 4];

                attribute.Strings.Add(str);
            }
            else
            {
                attribute.Strings.Add("");
            }
        }

        private void ProcessCarList(Chunk chunk)
        {
            var newSize = chunk.Size - Reader.AlignToBoundary(0x10);

            if (newSize % 0xD0 != 0)
            {
                throw new InvalidDataException("Malformed car list chunk");
            }

            Progress?.Report("Processing car list");

            while (Stream.Position < chunk.EndOffset)
            {
                var cti = BinaryHelpers.ReadStruct<CarTypeInfo>(Reader);

                //Debug.WriteLine("{0} {1} ({2})", cti.ManufacturerName, cti.BaseModelName, cti.UsageType);

                var acr = new AaronCarRecord();
                acr.BaseModelName = cti.BaseModelName;
                acr.CarTypeName = cti.CarTypeName;
                acr.ManufacturerName = cti.ManufacturerName;
                acr.DefaultBasePaint = cti.DefaultBasePaint;
                acr.DefaultSkinNumber = cti.DefaultSkinNumber;
                acr.Skinnable = cti.Skinnable;
                acr.UsageType = cti.UsageType;

                _carService.AddCar(acr);
                HashResolver.Add(cti.CarTypeNameHash, cti.CarTypeName);
            }

            this.GenerateHashes();
        }

        private void GenerateHashes()
        {
            Progress?.Report("Building hash table");
            DebugTiming.BeginTiming("GenerateHashes");

            var racerHashes = AaronHashLists.Get("RacerHashes");
            var copHashes = AaronHashLists.Get("CopHashes");
            var trafficHashes = AaronHashLists.Get("TrafficHashes");

            foreach (var aaronCarRecord in _carService.GetCarsByType(CarUsageType.Racing))
            {
                //Debug.WriteLine($"Add racer hashes for {aaronCarRecord.CarTypeName}");
                foreach (var hashText in racerHashes.Select(s => s.Replace("%", aaronCarRecord.CarTypeName)))
                {
                    HashResolver.Add(Hashing.BinHash(hashText), hashText);
                }
            }

            foreach (var aaronCarRecord in _carService.GetCarsByType(CarUsageType.Cop))
            {
                //Debug.WriteLine($"Add cop hashes for {aaronCarRecord.CarTypeName}");
                foreach (var hashText in copHashes.Select(s => s.Replace("%", aaronCarRecord.CarTypeName)))
                {
                    //Debug.WriteLine(hashText);
                    HashResolver.Add(Hashing.BinHash(hashText), hashText);
                }
            }

            foreach (var aaronCarRecord in _carService.GetCarsByType(CarUsageType.Traffic))
            {
                //Debug.WriteLine($"Add traffic hashes for {aaronCarRecord.CarTypeName}");
                foreach (var hashText in trafficHashes.Select(s => s.Replace("%", aaronCarRecord.CarTypeName)))
                {
                    //Debug.WriteLine(hashText);
                    HashResolver.Add(Hashing.BinHash(hashText), hashText);
                }
            }

            DebugTiming.EndTiming("GenerateHashes");
        }
    }
}
