/*
 * TheNexusAvenger
 *
 * PWM signal for keyboard presses.
 */

using InputSimulatorStandard.Native;
using KeyboardPWM.Inputs;

namespace KeyboardPWM.Input.Analog
{
    public class KeyboardPWMSignal : PWMSignal
    {
        private VirtualKeyCode Key;
        private bool KeyDown = false;
        
        /*
         * Creates the Keyboard PWM Signal.
         */
        public KeyboardPWMSignal(VirtualKeyCode key,float frequency) : base(frequency)
        {
            this.Key = key;
            this.KeyDown = false;
        }
        
        /*
         * Method called for signalling the start of an active signal.
         */
        public override void Active()
        {
            if (!this.KeyDown)
            {
                this.KeyDown = true;
                Keyboard.GetInstance().SendKeyDown(this.Key);
            }
        }
        
        /*
         * Method called for signalling the start of an inactive signal.
         */
        public override void Inactive()
        {
            if (this.KeyDown)
            {
                this.KeyDown = false;
                Keyboard.GetInstance().SendKeyUp(this.Key);
            }
        }
    }
}