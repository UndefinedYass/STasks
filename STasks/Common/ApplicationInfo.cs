using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace STasks.Common
{


    /// <summary>
    /// Mi reusable class contains the info in the about view and other stuff
    /// </summary>
    public class ApplicationInfo
    {

        public ApplicationInfo()
        {
            // Environment.CurrentDirectory
        }

        public static bool IsDev { get; set; } = true;
        public static string APP_TITLE { get; set; } = "STasks 1.0";
        public static string APP_SUB_TITLE { get; set; } = "© Mi 2021 ";
        public static string APP_VERSION { get; set; } = "0.1.0 (unreleased)" + (IsDev ? " [dev]" : "");
        public static string APP_DEVELOPER_NAME { get; set; } = "Yass.Mi";
        public static string APP_GUI_DESIGNER_NAME { get; set; } = "Yass.Mi";
        public static string APP_GITHUB_URL { get; set; } = "https://github.com/UndefinedYass/clw";


        public static string APP_DEVELOPER_EMAIL { get; set; } = "DIR16CAT17@gmail.com";

        public static int Host_Rendering_Tier { get; set; } = RenderCapability.Tier >> 16;
    }

    /// <summary>
    /// helper class that does the same: consider mering it with AppInfo
    /// origin: from fbhd
    /// usage: comment/uncomeent the statric properies as needed
    /// update ther values as needed
    /// </summary>
    public class MI
    {
        public static string MAIN_PATH = Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);

        //public static string CURL_PATH = MAIN_PATH + "\\curl\\curl.exe";
        public static string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mi\\STasks";
        //public static string DEFAULT_GLOBAL_OUTPUT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CLW Output";
        //public static string CLW_PRESETS_DIR = MAIN_PATH + "\\CLW Presets";
        public static string APP_CONFIG_FILE = APP_DATA + "\\stasks.config.mi.xml";
        //internal static string TEMP_HTML_FILES = APP_DATA + "\\Temp HTML";
        //internal static string SCRIPTS_DIR = MAIN_PATH + "\\scripts";
        //internal static string regexTestPattern = "\\\\x3CPeriod duration=\\\\\\\"PT(.*?)H(.*?)M(.*?)S\\\">";
        //internal static object SFX_DIRECTORY = MAIN_PATH + "\\SFX";
        internal static string ERRORS_LOG_FILE = APP_DATA + @"\Errors.log";

        internal static void OnAppStartup()
        {
            if (Directory.Exists(MI.APP_DATA) == false)
            {
                Directory.CreateDirectory(MI.APP_DATA);
            }
        }
        //public static string APP_CONFIG_FILE_V2 = APP_DATA + "\\config.mi.v2.xml";
    }





}
