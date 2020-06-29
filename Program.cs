using System;
using System.IO;
using Booker.Properties;


namespace Booker
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            Global global = null;

            // Validate our parameters.
            if(args == null || args.Length == 0)
            {
                // No parameters passed. See if we have the location set for our default run-time settings file.
                if(Settings.Default.RuntimeSettingFile != null && Settings.Default.RuntimeSettingFile.Length > 0)
                {
                    // Get our run-time settings.
                    global = Global.Deserialize(Settings.Default.RuntimeSettingFile);

                    // Tell the user that we failed to retrieved our run-time parameters file.
                    if(global == null)
                    {
                        Console.WriteLine($"No parameters were passed and we couldn't load '{Settings.Default.RuntimeSettingFile}'.");
                    }
                }

                // See if there's a default runtimeparams.json file in the execution directory. 
                string fileNamePath = $@"{AppDomain.CurrentDomain.BaseDirectory}runtimeparams.json";


                // Try and get our run-time settings from the execution directory.
                global = Global.Deserialize(fileNamePath);

                // Tell the user that we failed to retrieved our run-time parameters file.
                if(global == null)
                {
                    Console.WriteLine($"No run-time parameters were found in the execution directory '{fileNamePath}'.{Environment.NewLine}Bye!");

                    return 1;
                }
            }

            // Override any settings in our run-time parameter
            new Args(args, global);

            return 0;
        }
    }
}
