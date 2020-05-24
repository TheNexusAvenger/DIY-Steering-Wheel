/*
 * TheNexusAvenger
 *
 * Singleton for a keyboard.
 */

using InputSimulatorStandard;
using InputSimulatorStandard.Native;

namespace KeyboardPWM.Inputs
{
    public class Keyboard
    {
        private static Keyboard StaticInstance;
        private KeyboardSimulator SimKeyboard;
        
        /*
         * Creates a keyboard object.
         */
        private Keyboard()
        {
            this.SimKeyboard = new KeyboardSimulator();
        }
        
        /*
         * Returns a static instance of the keyboard.
         */
        public static Keyboard GetInstance()
        {
            // Create the keyboard if it doesn't exist.
            if (StaticInstance == null)
            {
                StaticInstance = new Keyboard();
            }
            
            // Return the static instance.
            return StaticInstance;
        }
        
        /*
         * Sends a key down signal.
         */
        public void SendKeyDown(VirtualKeyCode code)
        {
            this.SimKeyboard.KeyDown(code);
        }
        
        /*
         * Sends a key up signal.
         */
        public void SendKeyUp(VirtualKeyCode code)
        {
            this.SimKeyboard.KeyUp(code);
        }
        
        /*
         * Sends a key press signal.
         */
        public void SendKeyPress(VirtualKeyCode code)
        {
            this.SimKeyboard.KeyPress(code);
        }
    }
}