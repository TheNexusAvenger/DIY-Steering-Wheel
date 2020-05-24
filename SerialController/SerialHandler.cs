/*
 * TheNexusAvenger
 *
 * Reads data from serial ports.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using KeyboardPWM.Input.Analog;
using KeyboardPWM.Serial;

namespace SerialController
{
    
    
    public class SerialHandler
    {
        private int Port;
        private List<BaseSerialController> Controllers;
        private List<string> ControllerNames;
        private List<AnalogInputData> AnalogInputs;
        private List<string> AnalogInputNames;
        private List<byte> LastAnalogInputs;
        
        /*
         * Creates a serial handler.
         */
        public SerialHandler(int port,List<AnalogInputData> analogInputs)
        {
            this.Port = port;
            this.AnalogInputs = analogInputs;
        }
        
        /*
         * Handles the serial handler.
         */
        public void HandleInputs()
        {
            try
            {
                // Create and open the port.
                var port = new SerialPort("COM" + this.Port, 9600, Parity.None, 8, StopBits.One);
                port.Open();
                Console.WriteLine("Opened on port COM" + this.Port);

                // Create the controllers.
                if (this.Controllers == null)
                {
                    this.Controllers = new List<BaseSerialController>()
                    {
                        null,
                        new RobloxUltimateDrivingSerialController(),
                    };
                    this.AnalogInputNames = new List<string>()
                    {
                        "Mode Selector",
                        "Steering Wheel",
                        "Right Pedal",
                        "Left Pedal",
                        "Left Shifter",
                        "Right Shifter",
                    };
                    this.ControllerNames = new List<string>()
                    {
                        null,
                        "Roblox Ultimate Driving",
                    };
                    this.LastAnalogInputs = new List<byte>() { 0,0,0,0,0,0 };
                }

                // Create the analog input mapper.
                var analogMappers = new List<AnalogMapper>();
                foreach (var data in this.AnalogInputs)
                {
                    analogMappers.Add(AnalogMapper.FromData(data));
                }

                // Create the mode selector.
                var currentController = 0;
                var modeSelectorStateController = new AnalogState(6,0,(newState,previousState) =>
                {
                    currentController = newState;
                    
                    // Start the new controller.
                    if (newState < this.Controllers.Count && this.Controllers[newState] != null)
                    {
                        Console.WriteLine("Using controls for " + this.ControllerNames[newState]);
                        this.Controllers[newState].Start();
                    }
                    else
                    {
                        Console.WriteLine("Disabled controls"); 
                    }
                    
                    // Stop the previous controller.
                    if (previousState < this.Controllers.Count && this.Controllers[previousState] != null)
                    {
                        this.Controllers[previousState].Stop();
                    }
                });

                // Start accepting inputs.
                while (true)
                {
                    // Read the line.
                    var line = port.ReadLine();
                    try
                    {
                        // Handle the inputs.
                        if (line.Length >= 4 && line.Contains(","))
                        {
                            var inputType = line[0];
                            if (inputType == 'A')
                            {
                                var channel = int.Parse(line.Substring(1).Split(',')[0]);
                                var input = int.Parse(line.Substring(1).Split(',')[1]);
                                if (channel < analogMappers.Count)
                                {
                                    var mapper = analogMappers[channel];
                                    var initiallyEnabled = mapper.MinimumRangeMet();
                                    var mappedValue = mapper.GetValue(input);
                                    if (mapper.MinimumRangeMet())
                                    {
                                        if (channel == 0)
                                        {
                                            modeSelectorStateController.SetValue(mappedValue);
                                        }
                                        else
                                        {
                                            // Send the input.
                                            this.LastAnalogInputs[channel] = mappedValue;
                                            if (currentController < this.Controllers.Count && this.Controllers[currentController] != null)
                                            {
                                                this.Controllers[currentController].HandleInput("A" + channel + "," + mappedValue);
                                                if (!initiallyEnabled && this.AnalogInputNames[channel] != null)
                                                {
                                                    Console.WriteLine(this.AnalogInputNames[channel] + " is now active.");
                                                }
                                            }
                                            
                                            // Reset the pedals if both are down.
                                            if ((channel == 2 || channel == 3) && (this.LastAnalogInputs[2] == 255 && this.LastAnalogInputs[3] == 255))
                                            {
                                                Console.WriteLine("Resetting pedals.");
                                                analogMappers[2].Reset();
                                                analogMappers[3].Reset();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (currentController < this.Controllers.Count && this.Controllers[currentController] != null)
                                {
                                    this.Controllers[currentController].HandleInput(line);
                                }
                            }
                        }
                    }
                    catch (FormatException)
                    {

                    }

                }
            }
            catch (IOException)
            {

            }
            catch (InvalidOperationException)
            {

            }
            finally
            {
                if (this.Controllers != null) {
                    foreach (var controller in this.Controllers)
                    {
                        if (controller != null)
                        {
                            controller.Stop();
                        }
                    }
                }
            }
        }
        
        /*
         * Starts connecting the port.
         */
        public void StartConnecting()
        {
            new Thread(() =>
            {
                while (true)
                {
                    this.HandleInputs();
                    Thread.Sleep(200);
                }
            }).Start();
        }
    }
}