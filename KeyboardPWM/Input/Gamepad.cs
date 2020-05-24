/*
 * TheNexusAvenger
 *
 * Singleton for a gamepad.
 */

using System;
using System.Collections.Generic;
using SimWinInput;

namespace KeyboardPWM.Inputs
{
    public enum AnalogInput
    {
        LeftTrigger,
        RightTrigger,
        LeftThumbstickX,
        LeftThumbstickY,
        RightThumbstickX,
        RightThumbstickY,
    }
    
    public class Gamepad
    {
        private bool Active;
        private static List<Gamepad> GamepadsInUse = new List<Gamepad>();
        private static int ActiveGamepads = 0;
        
        /*
         * Creates a gamepad object.
         */
        private Gamepad()
        {
            SimGamePad.Instance.Initialize();
        }
        
        /*
         * Returns an instance of the gamepad.
         */
        public static Gamepad GetInstance()
        {
            var newGamepad = new Gamepad();
            GamepadsInUse.Add(newGamepad);
            return newGamepad;
        }
        
        /*
         * Starts the controller.
         */
        public void Start()
        {
            if (!this.Active)
            {
                this.Active = true;
                if (ActiveGamepads == 0)
                {
                    SimGamePad.Instance.PlugIn();
                }
                ActiveGamepads += 1;
            }
        }
        
        /*
         * Stops the controller.
         */
        public void Stop()
        {
            if (this.Active)
            {
                this.Active = false;
                ActiveGamepads += -1;
                if (ActiveGamepads == 0)
                {
                    SimGamePad.Instance.State[0].Reset();
                    SimGamePad.Instance.Unplug();
                }
            }
        }
        
        /*
         * Sets the value of an analog input.
         */
        public void SetAnalogInput(AnalogInput input,short value)
        {
            this.Start();
            if (this.Active)
            {
                // Get the state.
                var state = SimGamePad.Instance.State[0];

                // Set the input.
                if (input == AnalogInput.LeftTrigger)
                {
                    state.LeftTrigger = Convert.ToByte(((float) value / (float) short.MaxValue) * byte.MaxValue);
                }
                else if (input == AnalogInput.RightTrigger)
                {
                    state.RightTrigger = Convert.ToByte(((float) value / (float) short.MaxValue) * byte.MaxValue);
                }
                else if (input == AnalogInput.LeftThumbstickX)
                {
                    state.LeftStickX = value;
                }
                else if (input == AnalogInput.LeftThumbstickY)
                {
                    state.LeftStickY = value;
                }
                else if (input == AnalogInput.RightThumbstickX)
                {
                    state.RightStickX = value;
                }
                else if (input == AnalogInput.RightThumbstickY)
                {
                    state.RightStickY = value;
                }

                // Update the state.
                SimGamePad.Instance.Update();
            }
        }
    }
}