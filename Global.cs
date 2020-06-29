using System;
using System.IO;
using Booker.Properties;
using Newtonsoft.Json;

namespace Booker
{
    public class Global
    {
        const string JSON_EXTENSION = ".JSON";

        public Global()
        {

        }

        #region Public properties.
        int[] Courts                        { get; set; } = new int[] {1,2,3,4,5,6,7};

        DateTime DateOfGame                 { get; set; } = DateTime.Today;

        TimeSpan TimeOfGame                 { get; set; } = new TimeSpan(9, 0, 0);

        TimeSpan StartRequestingCourtAt     { get; set; } = new TimeSpan(8, 0, 0);

        DateTime StartRequestingCourtOn     { get; set; } = DateTime.Today.AddDays(14);



        int LengthOfGameInMinutes { get; set; }
        #endregion
        #region Serialize/Deserialize methods.
        public bool Serialize(string fileNamePath)
        {
            bool result = true;

            try
            {
                using(StreamWriter file = File.CreateText(fileNamePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, this);
                }
            }
            catch(Exception ex)
            {
                result = false;
            }
            return result;
        }

        public static Global Deserialize(string fileNamePath)
        {
            Global global = null;

            try
            {
                // Make sure it's a JSON file.
                string ext = Path.GetExtension(Settings.Default.RuntimeSettingFile).ToUpper();

                if(ext == JSON_EXTENSION)
                {
                    // Make sure that the file exists.
                    if(File.Exists(Settings.Default.RuntimeSettingFile))
                    {
                        using(StreamReader file = File.OpenText(fileNamePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            global = (Global) serializer.Deserialize(file, typeof(Global));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return global;
        }
        #endregion
    }
}
