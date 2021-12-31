using STasks.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace STasks.Services
{
    /// <summary>
    /// making this observable might become required in the future,
    /// </summary>
    [Serializable]
    
    public class ConfigService
    {

        public ConfigService()
        {
                
        }
        [NonSerialized]
        private static ConfigService _Instance ;
        public static ConfigService Instance { get { if (_Instance == null) _Instance = ConfigService.Load(); return _Instance; } }

        public string CurrentSTDocPath { get; set; } = string.Empty;
        public string CurrentWorkspaceDirectory { get; set; } = @"C:\faculte\s5 2021";

        public bool UseMockSemester { get; set; }
        static XmlSerializer sr = new XmlSerializer(typeof(ConfigService));
        /// <summary>
        /// attemps to load the xml config file, if file is missing the factory config is automatically saved and returned
        /// throws unhandled exceptions if file deserialization fails
        /// </summary>
        /// <param name="ConfigFile">if not specified, the MI.APP_CONFIG_FILE_V2 is used  </param>
        /// <returns></returns>
        private static ConfigService Load(string ConfigFile = null)
        {
            Trace.WriteLine("ConfigService Load was called");
            if (ConfigFile == null) ConfigFile = MI.APP_CONFIG_FILE;
            if (!File.Exists(ConfigFile))
            {
                return ConfigService.FactoryConfig().Save();
            }
            using (var stream = File.OpenRead(ConfigFile))
            {
                return sr.Deserialize(stream) as ConfigService;
            }

        }





        public ConfigService Save(string saveAS = null)
        {
            if (saveAS == null) saveAS = MI.APP_CONFIG_FILE;
            using (var stream = File.Open(saveAS, FileMode.Create))
            {
                sr.Serialize(stream, this);
            }
            Trace.WriteLine("configservice saved");
            //MainWindow.ShowMessage("Settings Saved"); todo mplement equvalent
            return this;
        }



        private static ConfigService FactoryConfig()
        {
            var facto = new ConfigService();
            //todo make this usfull
            return facto;
        }
    }
}
