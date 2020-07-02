using System;
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
            Global global       = null;
            string fileNamePath = null;
            bool okToRun = false;           


            // Announce ourselves to the user.
            Console.WriteLine("Booker version 0.1 Beta now running...");

            // Validate our parameters. See if we have the location set for our default run-time parameters file.
            if(Settings.Default.RuntimeSettingFile != null && Settings.Default.RuntimeSettingFile.Length > 0)
            {
                // Get our run-time parameters.
                global = Global.Deserialize(Settings.Default.RuntimeSettingFile);

                // Tell the user that we failed to retrieved our run-time parameters file.
                if(global == null)
                {
                    Console.WriteLine($"We couldn't load '{Settings.Default.RuntimeSettingFile}'.");
                }
            }

            // If globals hasn't been set, look in the current directory.
            if(global == null)
            {
                // See if there's a default runtimeparams.json file in the execution directory. 
                fileNamePath = $@"{AppDomain.CurrentDomain.BaseDirectory}runtimeparams.json";

                // Try and get our run-time settings from the execution directory.
                global = Global.Deserialize(fileNamePath);
            }

            // Tell the user that we failed to retrieved our run-time parameters file.
            if(global == null)
            {
                Console.WriteLine($"No run-time parameter files are being used.");
            }

            // Override any settings in our run-time parameter with the parameters the user passed.
            ArgsParser ap = new ArgsParser();
            global = ap.ParseAndMergeArgs(args, global);

            // If the Global instance isn't in a valid state, ask the user to fill in the blanks.
            if (global.IsValid()) { okToRun = true; }
            else
            {
                if (FillInTheBlanks(global)) 
                { 
                    if (global.IsValid()) { okToRun = true; }
                }
            }

            // Show the user our run parameters for this run.
            global.ToString();

            // Ask them if they want to continue.
            Console.WriteLine("Do you wish to continue? enter 'y' to continue or any other character to cancel the run.");
            string goNoGo = Console.ReadLine().ToLower();

            if (goNoGo == "y")
            {
                new BookCourt().Run(global);
            }


            // Announce our demise to the user.
            Console.WriteLine("Booker version 0.1 Beta now terminated.");


            return 0;
        }

        private static bool FillInTheBlanks(Global global)
        {
            return true;
        }
    }
}
