using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Aaron.Game;
using Aaron.Utils;

namespace Aaron
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load hash lists
            AaronHashLists.Load("GenericHashes", @"ApplicationData\HashLists\GenericHashes.txt");
            AaronHashLists.Load("RacerHashes", @"ApplicationData\HashLists\RacerHashes.txt");
            AaronHashLists.Load("CopHashes", @"ApplicationData\HashLists\CopHashes.txt");
            AaronHashLists.Load("TrafficHashes", @"ApplicationData\HashLists\TrafficHashes.txt");

            // Register all generic hashes immediately
            foreach (var hashText in AaronHashLists.Get("GenericHashes"))
            {
                HashResolver.Add(Hashing.BinHash(hashText), hashText);
            }

            if (Directory.Exists(@"ApplicationData\CustomHashLists"))
            {
                foreach (var file in Directory.GetFiles(@"ApplicationData\CustomHashLists", "*.txt",
                    SearchOption.TopDirectoryOnly))
                {
                    AaronHashLists.Load(Path.GetFileNameWithoutExtension(file), file);

                    foreach (var s in AaronHashLists.Get(Path.GetFileNameWithoutExtension(file)))
                    {
                        HashResolver.Add(Hashing.BinHash(s), s);
                    }
                }
            }

            // Load templates
            AaronCarTemplates.LoadFromFile("CAR_COP", @"ApplicationData\Templates\CarTemplate_COP.json");
            AaronCarTemplates.LoadFromFile("CAR_RACER", @"ApplicationData\Templates\CarTemplate_RACER.json");
            AaronCarTemplates.LoadFromFile("CAR_TRAFFIC", @"ApplicationData\Templates\CarTemplate_TRAFFIC.json");

            // Load other data
            AaronManufacturers.LoadFromFile(@"ApplicationData\Manufacturers.txt");
        }
    }
}
