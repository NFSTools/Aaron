using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Aaron.Data;
using Aaron.Data.Bounds;
using Aaron.Enums;
using Aaron.Messages;
using Aaron.Services;
using Aaron.Structures;
using Aaron.Utils;
using GalaSoft.MvvmLight;

namespace Aaron.ViewModel
{
    public class AddCarViewModel : ViewModelBase
    {
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IPresetSkinService _presetSkinService;
        private string _carName;
        private CarUsageType _carType;
        private string _manufacturer;

        public string CarName
        {
            get => _carName;
            set
            {
                _carName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        public CarUsageType CarType
        {
            get => _carType;
            set
            {
                _carType = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                RaisePropertyChanged();
            }
        }

        public bool CanSubmit => !string.IsNullOrWhiteSpace(CarName);

        public List<CarUsageType> AvailableTypes => new List<CarUsageType> { CarUsageType.Racing, CarUsageType.Traffic, CarUsageType.Cop };

        public List<string> AvailableManufacturers => AaronManufacturers.GetManufacturers();

        public AddCarViewModel(ICarService carService, ICarPartService carPartService, IPresetSkinService presetSkinService)
        {
            _carService = carService;
            _carPartService = carPartService;
            _presetSkinService = presetSkinService;

            CarType = CarUsageType.Racing;
            Manufacturer = AvailableManufacturers.First();
        }

        public void DoWork()
        {
            Debug.Assert(this.CanSubmit);

            if (this._carService.FindCarByName(this._carName) != null)
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Error, $"There is already a car called '{this._carName}'."));

                return;
            }

            if (this._carPartService.GetCarPartCollectionByName(this._carName) != null)
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Error, $"There is already a car part collection called '{this._carName}'."));

                return;
            }

            if (this._presetSkinService.FindPresetSkinByName(this._carName) != null)
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Error, $"There is already a preset skin called '{this._carName}'."));

                return;
            }

            var carType = _carType;

            // add car record
            var carRecord = new AaronCarRecord();
            carRecord.CarTypeName = this._carName.ToUpperInvariant();
            carRecord.BaseModelName = carRecord.CarTypeName;
            carRecord.Changed = true;
            carRecord.UsageType = carType;
            carRecord.Skinnable = true;
            carRecord.ManufacturerName = this.Manufacturer;
            carRecord.DefaultSkinNumber = 1;
            carRecord.Spoiler = new AaronSpoilerRecord();
            carRecord.Spoiler.SpoilerType = AaronSpoilerType.None;
            carRecord.BoundsPack =
                Serialization.Deserialize<AaronBoundsPack>(
                    File.ReadAllText(@"ApplicationData\Templates\BoundsTemplate.json"));

            switch (carType)
            {
                case CarUsageType.Racing:
                    carRecord.DefaultBasePaint = 3099377161;
                    break;
                case CarUsageType.Traffic:
                    carRecord.DefaultBasePaint = 1756372793;
                    break;
                case CarUsageType.Cop:
                    carRecord.DefaultBasePaint = 889465276;
                    carRecord.Skinnable = false;
                    break;
            }

            _carService.AddCar(carRecord);

            // Add parts
            string templateId = "CAR_";

            switch (carType)
            {
                case CarUsageType.Racing:
                    templateId += "RACER";
                    break;
                case CarUsageType.Cop:
                    templateId += "COP";
                    break;
                case CarUsageType.Traffic:
                    templateId += "TRAFFIC";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var carPartCollection = new AaronCarPartCollection();
            carPartCollection.Parts = new SynchronizedObservableCollection<AaronCarPartRecord>();
            carPartCollection.Name = _carName;

            foreach (var aaronCarPartRecord in AaronCarTemplates.Get(templateId))
            {
                // create new part
                var npr = new AaronCarPartRecord();
                npr.Name = aaronCarPartRecord.Name.Replace("%", carRecord.CarTypeName);
                npr.CollectionHash = carPartCollection.Hash;
                npr.Attributes = new SynchronizedObservableCollection<AaronCarPartAttribute>();

                // create new attributes
                foreach (var aaronCarPartAttribute in aaronCarPartRecord.Attributes)
                {
                    var npa = new AaronCarPartAttribute();
                    npa.Name = aaronCarPartAttribute.Name;
                    npa.Value = aaronCarPartAttribute.Value;

                    foreach (var s in aaronCarPartAttribute.Strings)
                    {
                        npa.Strings.Add(string.Copy(s));
                    }

                    npr.Attributes.Add(npa);
                }

                carPartCollection.Parts.Add(npr);
            }

            carPartCollection.Changed = true;
            //carPartCollection.Priority = (uint) _carPartService.GetCarPartCollections().Count + 1;
            _carPartService.AddCarPartCollection(carPartCollection);

            // add preset skin
            var ps = new AaronPresetSkinRecord
            {
                PaintGroup = 47885063,
                PaintHue = 496032566,
                Variance = 0xff,
                PresetName = carRecord.CarTypeName,
                Changed = true
            };

            _presetSkinService.AddPresetSkin(ps);
        }
    }
}
