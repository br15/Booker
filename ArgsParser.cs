using System;
using System.Collections.Generic;
using KeithBalaam.Extensions;


namespace Booker
{
    public class ArgsParser
    {
        public ArgsParser() { }

        public Global ParseAndMergeArgs(string[] args, Global g)
        {
            Global global;
            string arg, value;  
            bool error                  = false;
            bool saveRunTimeParaeters   = false;


            // Initialise our Global instance if it's not already set.
            if (g == null) 
            { 
                global = new Global(); 
            }
            else 
            { 
                global = g; 
            }

            // Validate our parameters.
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No parameters passed at run-time.");
                return global;
            }

            // If our args contains "filename" then we will ignore all other parameters. 
            bool fileNameSpecified = Environment.CommandLine.ToLower().Contains("filename");

             // It looks like we've got some parameters. Let's validate them.
            for(int i = 0; i < args.Length; ++i)
            {
                // If the last parameter was bad, stop now.
                if (error)
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //
                    // Return null to indicate that a bad parameter has been passed and that the program should terminate.
                    //
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    
                    return null;  // <<== LOOK!
                }


                // Get the name of the parameter then increment the index to point at the parameter value. 
                arg = args[i++].ToLower();
                if(i < args.Length) { value = args[i]; }
                else { value = null; }

                try
                {
                    switch(arg)
                    {
                        // If a run-time parameter file name is passed, all other parameters will be ignored.
                        case "filename":
                            error = ValidateFileName(value, global);
                            break;

                        case "courts":
                        case "c":
                            if(!fileNameSpecified) error = ValidateCourts(value, global);
                            break;

                        case "gamedate":
                        case "gd":
                            if(!fileNameSpecified) ValidateGameDate(value, global);
                            break;

                        case "gametime":
                        case "gt":
                            if(!fileNameSpecified) ValidateGameTime(value, global);
                            break;

                        case "gamelength":
                        case "gl":
                            if(!fileNameSpecified) ValidateGameLength(value, global);
                            break;
                        case "userid":
                        case "u":
                            if(!fileNameSpecified) ValidateUserId(value, global);
                            break;

                        case "password":
                        case "p":
                            if(!fileNameSpecified) ValidatePassword(value, global);
                            break;

                        case "saveruntimeparameters":
                        case "srtp":
                            if(value == "y" || value == "yes")
                            {
                                saveRunTimeParaeters = true;
                            }
                            break;
                        default:
                            // Ignore unknown arguments.
                            Console.WriteLine($"Ignoring unknown arg: '{arg}' with a value of: '{value}'");
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"We had a problem with the '{arg}' parameter. Error: {ex.Message}");
                }
            }

            if (saveRunTimeParaeters)
            {
                string fileNamePath = $@"{AppDomain.CurrentDomain.BaseDirectory}{Global.RUN_TIME_PARAMS_FILE_NAME} ";
                global.Serialize(fileNamePath);
            }

            return global;
        }


        private bool ValidateFileName(string arg, Global global)
        {
            bool error = false;


            // Get our run-time parameters.
            global = Global.Deserialize(arg);

            // Tell the user that we failed to retrieved our run-time parameters file.
            if(global == null)
            {
                Console.WriteLine($"We filed to load '{arg}'.");
                error = true;
            }

            return error;
        }

        private bool ValidateGameLength(string value, Global global)
        {
            bool error = false;
            int length;

            int.TryParse(value, out length);

            if (length > 0)
            {
                global.GameLength = new TimeSpan(0, length, 0);
            }
            else
            { error = true; }

            return error;
        }


        private bool ValidatePassword(string arg, Global global)
        {
            bool error = false;
            
            if (arg.IsNullOrEmpty() || arg.Contains(" "))
            {
                Console.WriteLine($"Password is not in the correct format: '{arg}'");
                error = true;
            }
            else
            {
                global.Password = arg;
            }

            return error;
        }

        private bool ValidateUserId(string arg, Global global)
        {
            bool error = false;

            if (arg.IsNullOrEmpty() || arg.Contains(" "))
            {
                Console.WriteLine($"UserId is not in the correct format: '{arg}'");
                error = true;
            }
            else
            {
                global.UserId = arg;
            }

            return error;
        }

        /// <summary>
        /// Valid time format is: hh:mm
        ///     hh  -   Must be in the range of 0 to 23.
        ///     :   -   Colon.
        ///     mm  -   Must be in the range of 0 to 59.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        private bool ValidateGameTime(string arg, Global global)
        {
            bool error = false;
            int hh, mm;


            string[] tokens = arg.Split(':');
           
            if (tokens.Length == 2 && tokens[0].IsNumeric() && tokens[1].IsNumeric())
            {
                if(int.TryParse(tokens[0], out hh) && int.TryParse(tokens[1], out mm))
                {
                    if(hh >= 0 && hh <= 23 && mm >= 0 && mm <= 59)
                    {
                        global.GameTime = new TimeSpan(hh, mm, 0);
                    }
                    else
                    {
                        Console.WriteLine($"GameTime is not in the correct range: '{arg}'. The correct format is 'hh:mm'.");
                        error = true;
                    }
                }
                else
                {
                    Console.WriteLine($"GameTime is not numeric '{arg}'.");
                    error = true;
                }
            }
            else
            {
                Console.WriteLine($"GameTime is not in the correct format: '{arg}'. The correct format is 'hh:mm'.");
                error = true;
            }

            return error;
        }

        /// <summary>
        /// Valid format is: n,n,n,n,n
        /// 
        ///     n   -   Court number in the range of 1 to 7.
        /// 
        /// Note:   The program will attempt to book the first court in the list until either we book the court or the court is booked by another person.
        ///         At which point, we will attempt to book the next court. This process will continue until either we book a court or no courts are available.
        ///         The list can contain from 1 to 7 courts e.g. "courts 1,2,3,4,5,6,7" is the maximum. We attempt to book the courts in the order of the list.
        ///         Any courts over seven will be ignored.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        private bool ValidateCourts(string arg, Global global)
        {
            bool error = false;
            List<int> courts = new List<int>();


            string[] tokens = arg.Split(',');

            if (tokens.Length > 7)
            {
                Console.WriteLine($"You may only specify 7 courts. {tokens.Length} were specified. {tokens.Length - 7} will be ignored.");
            }

            for(int i = 0; i < 7; i++)
            {
                if(tokens[i].IsNumeric())
                {
                    courts.Add(int.Parse(tokens[i]));
                }
                else
                {
                    Console.WriteLine($"Invalid court number: {tokens[i]} in position {i + 1} will be ignored.");
                }
            }

            if (courts.Count > 0)
            {
                global.Courts = courts.ToArray();
            }
            else
            {
                error = true;
            }

            return error;
        }

        /// <summary>
        /// Valid date format is: dd/mm/yyyy
        ///     dd      Day number in the range 1 to 31.
        ///     mm      Month number in the range 1 to 12.
        ///     yyyy    Year number. Must be equal or greater than 2020 and less than 2040.
        ///     
        /// Note: The date must not be in the past.
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        private bool ValidateGameDate(string arg, Global global)
        {
            bool error = false;
            int dd, mm, yyyy;


            string[] tokens = arg.Split('/');

            if (tokens.Length == 3 & tokens[0].IsNumeric() && tokens[1].IsNumeric() && tokens[2].IsNumeric())
            {
                dd      = int.Parse(tokens[0]);
                mm      = int.Parse(tokens[1]);
                yyyy    = int.Parse(tokens[2]);

                if (dd >= 1 && dd <= 31 && mm >= 1 && mm <= 12 && yyyy >= 2020 && yyyy <= 2040)
                {
                    DateTime dt = new DateTime(yyyy, mm, dd);

                    if (dt >= DateTime.Today)
                    {
                        global.GameDate = dt;
                    }
                    else
                    {
                        error = true;
                        Console.WriteLine($"GameTime is in the past: '{arg}'. Today is {dt:d}.");
                    }
                }
                else
                {
                    error = true;
                    Console.WriteLine($"GameTime is not in the correct range: '{arg}'. The correct format is 'hh:mm'.");
                }
            }
            else
            {
                error = true;
                Console.WriteLine($"GameTime is not in the correct format: '{arg}'. The correct format is 'hh:mm'.");
            }

            return error;
        }
    }
}
