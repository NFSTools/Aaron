using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.DataIO;

namespace Aaron.Services.Implementations
{
    public class DataService : IDataService
    {
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IPresetCarService _presetCarService;
        private readonly IPresetSkinService _presetSkinService;
        private readonly IDataTableService _dataTableService;

        public DataService(ICarService carService, ICarPartService carPartService, IPresetCarService presetCarService,
            IPresetSkinService presetSkinService, IDataTableService dataTableService)
        {
            _carService = carService;
            _carPartService = carPartService;
            _presetCarService = presetCarService;
            _presetSkinService = presetSkinService;
            _dataTableService = dataTableService;
        }

        public void LoadCarController(string path, IProgress<string> progress)
        {
            _dataTableService.Reset();
            _presetSkinService.Reset();
            _presetCarService.Reset();
            _carPartService.Reset();
            _carService.Reset();

            using (var ccr = new CarControllerReader(path))
            {
                ccr.Read(progress);
            }
        }
    }
}
