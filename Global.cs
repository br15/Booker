using System;
using System.IO;
using Newtonsoft.Json;

namespace Booker
{
    public class Global
    {
        public const string JSON_EXTENSION             = ".json";
        public const string RUN_TIME_PARAMS_FILE_NAME  = "runtimeparams.json";

        public Global()
        {

        }

        #region Public properties.
        public int[] Courts             { get; set; } = new int[] {1,2,3,4,5,6,7};

        public DateTime GameDate        { get; set; } = DateTime.Today.AddDays(14);

        public TimeSpan GameTime        { get; set; } = new TimeSpan(9, 0, 0);

        public TimeSpan GameLength      { get; set; } = new TimeSpan(1, 30, 0);

        public TimeSpan StartRequests   { get; set; } = new TimeSpan(8, 0, 0);

        public string UserId            { get; set; }
        
        public string Password          { get; set; }
        #endregion
        
        public bool IsValid()
        {
            bool result = true;

            return result;
        }
        
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
                Console.WriteLine($"We had a problem serialising Run-time parameters file: {ex}");
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
                string ext = Path.GetExtension(fileNamePath).ToLower();

                if(ext == JSON_EXTENSION)
                {
                    // Make sure that the file exists.
                    if(File.Exists(fileNamePath))
                    {
                        using(StreamReader file = File.OpenText(fileNamePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            global = (Global) serializer.Deserialize(file, typeof(Global));
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Run-time parameters file must be a JSON file not '{ext}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"We had a problem de-serialising Run-time parameters file: '{fileNamePath}'. Error: {ex}");
            }

            return global;
        }
        #endregion
    }
}
