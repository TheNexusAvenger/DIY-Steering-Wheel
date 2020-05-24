/*
 * TheNexusAvenger
 *
 * Class for running the keyboard controller.
 * Only intended to use for testing without actual serial hardware.
 * Uses F24,F23,F22,F21,F20.
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InputSimulatorStandard.Native;
using KeyboardPWM;
using KeyboardPWM.Input.Analog;
using KeyboardPWM.Serial;

namespace KeyboardController
 {
    public partial class App : Application
    {
        public readonly List<Key> KEYS_TO_LISTEN_FOR = new List<Key>()
        {
            Key.F20,
            Key.F21,
            Key.F22,
            Key.F23,
            Key.F24,
        };
        public readonly List<string> ANALOG_CHANNEL_NAMES = new List<string>()
        {
            "Steering (A000)",
            "Brake (A001)",
            "Throttle (A002)",
            "Signals (A003)",
            "Ignition (A004)",
        };
        public readonly List<double> ANALOG_CHANNEL_DUTY_CYCLE_ADDITIONS = new List<double>()
        {
            -0.1,
            0.1,
            0.1,
            0.5,
            1,
        };
        public readonly List<string> DIGITAL_CHANNEL_NAMES = new List<string>()
        {
            "Horn (D000)",
            "Shift (D001)",
            "Cruise Control (D002)",
            "Hazards (D003)",
            "Headlights (D004)",
            "Park (D005)",
            "Flip (D006)",
            "Spoiler Adjust (D007)",
            "Nitro (D008)",
        };
        
        private const byte DIGITAL_HORN_INPUT = 0;
        private const byte DIGITAL_SHIFT_INPUT = 1;
        private const byte DIGITAL_CRUISE_CONTROL_INPUT = 2;
        private const byte DIGITAL_HAZARDS_INPUT = 3;
        private const byte DIGITAL_HEADLIGHTS_INPUT = 4;
        private const byte DIGITAL_PARK_INPUT = 5;
        private const byte DIGITAL_FLIP_INPUT = 6;
        private const byte DIGITAL_SPOILER_ADJUST_INPUT = 7;
        private const byte DIGITAL_NITRO_INPUT = 8;

        private int CurrentAnalogChannel = 0;
        private int CurrentDigitalChannel = 0;
        private List<double> CurrentAnalogChannelDutyCycles = new List<double>()
        {
            0.5,
            0,
            0,
            0.5,
            0,
        };
        private RobloxUltimateDrivingSerialController Driver;
        
        /*
         * Creates the application.
         */
        public App()
        {
            this.Driver = new RobloxUltimateDrivingSerialController();
            
            // Output the initial channels.
            Console.WriteLine("TODO: This controller is obsolete. Using it requires updating.");
            Console.WriteLine("Analog channel set to " + this.ANALOG_CHANNEL_NAMES[this.CurrentAnalogChannel]);
            Console.WriteLine("Digital channel set to " + this.DIGITAL_CHANNEL_NAMES[this.CurrentDigitalChannel]);
            
            // Initialize the initial states of the keys.
            var keyStates = new Dictionary<Key,bool>();
            foreach (var key in KEYS_TO_LISTEN_FOR)
            {
                keyStates[key] = false;
            }
            
            // Start the loop.
            while (true)
            {
                foreach (var key in KEYS_TO_LISTEN_FOR)
                {
                    var keyDown = Keyboard.IsKeyDown(key);
                    
                    // Invoke the method if the key is changed.
                    if (keyDown != keyStates[key])
                    {
                        if (keyDown)
                        {
                            this.KeyPressed(key);
                        }
                        else
                        {
                            this.KeyReleased(key);
                        }
                    }
                    
                    // Set the key state.
                    keyStates[key] = keyDown;
                }
            }
        }
        
        /*
         * Event for a key being pressed.
         */
        public void KeyPressed(Key key)
        {
            if (key == Key.F24)
            {
                // Send the digital input.
                this.Driver.HandleInput("D" + this.CurrentDigitalChannel + ",1");
            } else if (key == Key.F23)
            {
                // Increase the duty cycle.
                var newDutyCycle = Math.Clamp(this.CurrentAnalogChannelDutyCycles[this.CurrentAnalogChannel] + this.ANALOG_CHANNEL_DUTY_CYCLE_ADDITIONS[this.CurrentAnalogChannel],0,1);
                Console.WriteLine("Setting duty cycle of " + this.ANALOG_CHANNEL_NAMES[this.CurrentAnalogChannel] + " to " + newDutyCycle);
                this.CurrentAnalogChannelDutyCycles[this.CurrentAnalogChannel] = newDutyCycle;
                this.Driver.HandleInput("A" + this.CurrentAnalogChannel + "," + (byte) (255 * newDutyCycle));
            } else if (key == Key.F22)
            {
                // Decrease the duty cycle.
                var newDutyCycle = Math.Clamp(this.CurrentAnalogChannelDutyCycles[this.CurrentAnalogChannel] - this.ANALOG_CHANNEL_DUTY_CYCLE_ADDITIONS[this.CurrentAnalogChannel],0,1);
                Console.WriteLine("Setting duty cycle of " + this.ANALOG_CHANNEL_NAMES[this.CurrentAnalogChannel] + " to " + newDutyCycle);
                this.CurrentAnalogChannelDutyCycles[this.CurrentAnalogChannel] = newDutyCycle;
                this.Driver.HandleInput("A" + this.CurrentAnalogChannel + "," + (byte) (255 * newDutyCycle));
            } else if (key == Key.F21)
            {
                // Toggle the analog channel.
                this.CurrentAnalogChannel += 1;
                if (this.CurrentAnalogChannel >= this.ANALOG_CHANNEL_NAMES.Count)
                {
                    this.CurrentAnalogChannel = 0;
                }
                
                // Output the new analog channel.
                Console.WriteLine("Set analog channel to " + this.ANALOG_CHANNEL_NAMES[this.CurrentAnalogChannel]);
            } else if (key == Key.F20)
            {
                // Toggle the digital channel.
                this.CurrentDigitalChannel += 1;
                if (this.CurrentDigitalChannel >= this.DIGITAL_CHANNEL_NAMES.Count)
                {
                    this.CurrentDigitalChannel = 0;
                }
                
                // Output the new digital channel.
                Console.WriteLine("Set digital channel to " + this.DIGITAL_CHANNEL_NAMES[this.CurrentDigitalChannel]);
            }
        }
        
        /*
         * Event for a key being released.
         */
        public void KeyReleased(Key key)
        {
            if (key == Key.F24)
            {
                // Send the digital input.
                this.Driver.HandleInput("D" + this.CurrentDigitalChannel + ",0");
            }
        }
    }
}