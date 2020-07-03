using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aaron.Data;
using Aaron.Data.XML;
using Aaron.DataIO;
using Aaron.Messages;
using Aaron.Utils;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace Aaron.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private const int AaronProjectVersion = 7;
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IPresetCarService _presetCarService;
        private readonly IPresetSkinService _presetSkinService;
        private readonly IDataTableService _dataTableService;

        private AaronProject _project;

        public ProjectService(ICarService carService, ICarPartService carPartService,
            IPresetCarService presetCarService, IPresetSkinService presetSkinService,
            IDataTableService dataTableService)
        {
            _carService = carService;
            _carPartService = carPartService;
            _presetCarService = presetCarService;
            _presetSkinService = presetSkinService;
            _dataTableService = dataTableService;
        }

        public string GenerateProject(string directory)
        {
            AaronProject aaronProject = new AaronProject();
            aaronProject.Version = AaronProjectVersion;
            aaronProject.CarsDirectory = Path.Combine(directory, "Cars");
            aaronProject.CarPartsDirectory = Path.Combine(directory, "CarParts");
            aaronProject.PresetCarsDirectory = Path.Combine(directory, "PresetCars");
            aaronProject.PresetSkinsDirectory = Path.Combine(directory, "PresetSkins");
            aaronProject.DataTablesDirectory = Path.Combine(directory, "DataTables");
            aaronProject.OutputFilePath = Path.Combine(directory, "bin", "GlobalC.lzc");
            aaronProject.SlotOverrideData = _carPartService.GetSlotOverrideData();

            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(aaronProject.CarsDirectory);
            Directory.CreateDirectory(aaronProject.CarPartsDirectory);
            Directory.CreateDirectory(aaronProject.PresetCarsDirectory);
            Directory.CreateDirectory(aaronProject.PresetSkinsDirectory);
            Directory.CreateDirectory(aaronProject.DataTablesDirectory);
            Directory.CreateDirectory(Path.Combine(directory, "bin"));

            var projectPath = Path.Combine(directory, "project.aproj");
            File.WriteAllText(projectPath, Serialization.Serialize(aaronProject));

            DebugTiming.BeginTiming("ExportCars");
            foreach (var aaronCarRecord in _carService.GetCars())
            {
                File.WriteAllText(Path.Combine(aaronProject.CarsDirectory, aaronCarRecord.CarTypeName + ".json"),
                    Serialization.Serialize(aaronCarRecord));
            }
            DebugTiming.EndTiming("ExportCars");

            DebugTiming.BeginTiming("ExportCarParts");

            {
                var carPartCollections = _carPartService.GetCarPartCollections();

                foreach (var carPartCollection in carPartCollections)
                {
                    var fileName = Path.Combine(aaronProject.CarPartsDirectory, $"{carPartCollection.Name}.json");
                    var serialized = Serialization.Serialize(carPartCollection);

                    File.WriteAllText(fileName, serialized);
                }
            }

            DebugTiming.EndTiming("ExportCarParts");

            DebugTiming.BeginTiming("ExportPresets");

            foreach (var aaronPresetCar in _presetCarService.GetPresetCars())
            {
                File.WriteAllText(
                    Path.Combine(aaronProject.PresetCarsDirectory, $"{aaronPresetCar.PresetName}.xml"),
                    PresetConverter.ConvertAaronPresetToServerXML(aaronPresetCar).DataContractSerializeObject());
            }

            DebugTiming.EndTiming("ExportPresets");

            DebugTiming.BeginTiming("ExportPresetSkins");

            foreach (var aaronPresetSkinRecord in _presetSkinService.GetPresetSkins())
            {
                File.WriteAllText(
                    Path.Combine(aaronProject.PresetSkinsDirectory, $"{aaronPresetSkinRecord.PresetName}.json"),
                    Serialization.Serialize(aaronPresetSkinRecord));
            }

            DebugTiming.EndTiming("ExportPresetSkins");

            DebugTiming.BeginTiming("ExportDataTables");

            foreach (var aaronDataTable in _dataTableService.GetDataTables())
            {
                File.WriteAllText(
                    Path.Combine(aaronProject.DataTablesDirectory, $"{aaronDataTable.Name}.json"),
                    Serialization.Serialize(aaronDataTable));
            }

            DebugTiming.EndTiming("ExportDataTables");

            File.WriteAllText(Path.Combine(directory, "strings.json"), Serialization.Serialize(_carPartService.GetStrings()));

            return projectPath;
        }

        public void SaveProject()
        {
            AaronProject aaronProject = GetCurrentProject();

            foreach (var item in this.GetChangedItems())
            {
                item.Changed = false;
                switch (item)
                {
                    case AaronCarRecord aaronCarRecord:
                        File.WriteAllText(Path.Combine(aaronProject.Directory, aaronProject.CarsDirectory, aaronCarRecord.CarTypeName + ".json"),
                            Serialization.Serialize(aaronCarRecord));
                        break;
                    case AaronCarPartCollection carPartCollection:
                        var fileName = Path.Combine(aaronProject.Directory, aaronProject.CarPartsDirectory, $"{carPartCollection.Name}.json");
                        var serialized = Serialization.Serialize(carPartCollection);

                        File.WriteAllText(fileName, serialized);
                        break;
                    case AaronPresetCar aaronPresetCar:
                        File.WriteAllText(
                            Path.Combine(aaronProject.Directory, aaronProject.PresetCarsDirectory, $"{aaronPresetCar.PresetName}.xml"),
                            PresetConverter.ConvertAaronPresetToServerXML(aaronPresetCar).DataContractSerializeObject());
                        break;
                    case AaronPresetSkinRecord aaronPresetSkinRecord:
                        File.WriteAllText(
                            Path.Combine(aaronProject.Directory, aaronProject.PresetSkinsDirectory, $"{aaronPresetSkinRecord.PresetName}.json"),
                            Serialization.Serialize(aaronPresetSkinRecord));
                        break;
                    case AaronDataTable aaronDataTable:
                        File.WriteAllText(
                            Path.Combine(aaronProject.Directory, aaronProject.DataTablesDirectory, $"{aaronDataTable.Name}.json"),
                            Serialization.Serialize(aaronDataTable));
                        break;
                }
            }
        }

        public void LoadProject(string file)
        {
            CloseProject();

            var dir = Path.GetDirectoryName(file) ?? "";
            var project = Serialization.Deserialize<AaronProject>(File.ReadAllText(file));
            project.Path = file;
            project.Directory = dir;
            project.OutputFilePath = Path.Combine(dir, project.OutputFilePath);

            if (project.Version != AaronProjectVersion)
            {
                throw new Exception("Incompatible project. Please make a backup and copy your changes to a new project.");
            }

            // set slot override data
            _carPartService.SetSlotOverrideData(project.SlotOverrideData);

            DebugTiming.BeginTiming("LoadCars");

            // load cars
            foreach (var carFilePath in Directory.GetFiles(Path.Combine(dir, project.CarsDirectory), "*.json"))
            {
                _carService.AddCar(Serialization.Deserialize<AaronCarRecord>(File.ReadAllText(carFilePath)));
            }
            DebugTiming.EndTiming("LoadCars");

            DebugTiming.BeginTiming("LoadCarParts");

            // load car part
            foreach (var carPartFilePath in Directory.GetFiles(Path.Combine(dir, project.CarPartsDirectory), "*.json"))
            {
                var aaronCarPartCollection = Serialization.Deserialize<AaronCarPartCollection>(File.ReadAllText(carPartFilePath));
                _carPartService.AddCarPartCollection(
                    aaronCarPartCollection);
            }

            DebugTiming.EndTiming("LoadCarParts");

            DebugTiming.BeginTiming("LoadPresets");

            // load presets
            foreach (var presetCarFilePath in Directory.GetFiles(Path.Combine(dir, project.PresetCarsDirectory), "*.xml"))
            {
                var convertServerXmlToAaronPreset = PresetConverter.ConvertServerXMLToAaronPreset(
                    File.ReadAllText(presetCarFilePath).DataContractDeserializeObject<OwnedCarTrans>());

                convertServerXmlToAaronPreset.PresetName = Path.GetFileNameWithoutExtension(presetCarFilePath);

                _presetCarService.AddPresetCar(convertServerXmlToAaronPreset);
            }

            DebugTiming.EndTiming("LoadPresets");

            DebugTiming.BeginTiming("LoadPresetSkins");

            foreach (var presetSkinFilePath in Directory.GetFiles(Path.Combine(dir, project.PresetSkinsDirectory), "*.json"))
            {
                _presetSkinService.AddPresetSkin(
                    Serialization.Deserialize<AaronPresetSkinRecord>(File.ReadAllText(presetSkinFilePath)));
            }

            DebugTiming.EndTiming("LoadPresetSkins");

            DebugTiming.BeginTiming("LoadDataTables");

            foreach (var dataTableFilePath in Directory.GetFiles(Path.Combine(dir, project.DataTablesDirectory), "*.json"))
            {
                _dataTableService.AddDataTable(
                    Serialization.Deserialize<AaronDataTable>(File.ReadAllText(dataTableFilePath)));
            }

            DebugTiming.EndTiming("LoadDataTables");

            using (var namesFileStream = File.OpenRead(Path.Combine(dir, "strings.json")))
            {
                foreach (var s in Serialization.Deserialize<List<string>>(
                    namesFileStream))
                {
                    _carPartService.AddString(s);
                }
            }

            _project = project;
            Messenger.Default.Send(new ProjectMessage(project));
        }

        public AaronProject GetCurrentProject()
        {
            return _project;
        }

        public bool HasUnsavedChanges()
        {
            return _carService.HasAnyChanges() || _carPartService.HasAnyChanges() ||
                   _dataTableService.HasAnyChanges() || _presetCarService.HasAnyChanges() ||
                   _presetSkinService.HasAnyChanges();
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _carService.GetChangedItems()
                .Concat(_carPartService.GetChangedItems())
                .Concat(_dataTableService.GetChangedItems())
                .Concat(_presetCarService.GetChangedItems())
                .Concat(_presetSkinService.GetChangedItems())
                .ToList();
        }

        public void CloseProject()
        {
            _dataTableService.Reset();
            _presetSkinService.Reset();
            _presetCarService.Reset();
            _carPartService.Reset();
            _carService.Reset();

            if (_project != null)
            {
                Messenger.Default.Send(new ProjectMessage(null));
            }

            _project = null;
        }
    }
}
