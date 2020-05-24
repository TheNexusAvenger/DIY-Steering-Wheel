/*
 * TheNexusAvenger
 *
 * Runs the program.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace SerialController
{
    internal class Program
    {
        public const string ANALOG_CONFIGURATION_FILE_LOCATION = "hardware.json";

        /*
         * Returns the location for a file.
         */
        public static string GetFileLocation(string fileName,string directory) {
            // If the file exists in the directory, return the directory.
            if (File.Exists(Path.Combine(directory,fileName))) {
                return Path.Combine(directory,fileName);
            }
            
            // Return null if the first index and last index of "/" are the same.
            if (Directory.GetParent(directory) == null) {
                return null;
            }
            
            // Return the result for the parent directory.
            return GetFileLocation(fileName,Directory.GetParent(directory).FullName);
        }
        
        /*
         * Runs the program.
         */
        public static void Main(string[] args)
        {
            // Find the hardware file and return if it wasn't found.
            var analogInputsFile = GetFileLocation(ANALOG_CONFIGURATION_FILE_LOCATION,Directory.GetCurrentDirectory());
            if (analogInputsFile == null)
            {
                Console.WriteLine("Unable to find " + ANALOG_CONFIGURATION_FILE_LOCATION + " in the current or parent directory.");
                return;
            }
            
            // Open the ports.
            var analogInputs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AnalogInputData>>(File.ReadAllText(analogInputsFile));
            for (var i = 0; i <= 32; i++)
            {
                new SerialHandler(i,analogInputs).StartConnecting();
            }
        }
    }
}