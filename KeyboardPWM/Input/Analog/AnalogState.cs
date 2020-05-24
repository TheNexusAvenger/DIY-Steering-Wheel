/*
 * TheNexusAvenger
 *
 * Simplifies logic for analog inputs that have fixed states.
 */

using System;

namespace KeyboardPWM.Input.Analog
{
    public class AnalogState
    {
        private byte TotalStates;
        private byte DebounceStates;
        private byte CurrentState;
        private Action<byte,byte> OnStateChange;
        
        /*
         * Creates the analog state.
         */
        public AnalogState(byte states,byte initialState,Action<byte,byte> onStateChange)
        {
            this.TotalStates = states;
            this.DebounceStates = (byte) ((states * 2) - 1);
            this.OnStateChange = onStateChange;
            this.CurrentState = initialState;
        }
        
        /*
         * Sets the current value.
         */
        public void SetValue(byte value)
        {
            var newState = (byte) (((value / (256 / this.DebounceStates)) + 1) / 2);
            if (newState == this.TotalStates)
            {
                newState = (byte) (this.TotalStates - 1);
            }
            
            // Invoke the change if the state changed.
            if (newState != this.CurrentState)
            {
                this.OnStateChange.Invoke(newState,this.CurrentState);
                this.CurrentState = newState;
            }
        }
    }
}