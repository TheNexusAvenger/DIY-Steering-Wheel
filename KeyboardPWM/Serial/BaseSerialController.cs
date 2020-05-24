/*
 * TheNexusAvenger
 *
 * Base class for handling serial inputs.
 */

using System;

namespace KeyboardPWM.Serial
{
    public abstract class BaseSerialController
    {
        /*
         * Processes an input.
         */
        public void HandleInput(string input)
        {
            var inputType = input.Substring(0,1);
            if (inputType == "A" || inputType == "D")
            {
                // Determine the input and the value.
                var dividerIndex = input.IndexOf(",", StringComparison.Ordinal);
                var index = Convert.ToByte(input.Substring(1,dividerIndex - 1));
                var value = Convert.ToByte(input.Substring(dividerIndex + 1));
                
                // Signal the input.
                if (inputType == "A")
                {
                    this.OnAnalogInput(index,value);
                }
                else
                {
                    this.OnDigitalInput(index,value != 0);
                }
            }
            else
            {
                Console.WriteLine("Ignoring input: " + input);
            }
        }
        
        /*
         * Starts the controller.
         */
        public abstract void Start();
        
        /*
         * Stops the controller.
         */
        public abstract void Stop();
        
        /*
         * Invoked when an analog input changes.
         */
        public abstract void OnAnalogInput(byte id,byte value);
        
        /*
         * Invoked when an digital input changes.
         */
        public abstract void OnDigitalInput(byte id,bool value);
    }
}