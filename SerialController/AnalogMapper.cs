/*
 * TheNexusAvenger
 *
 * Map analog inputs.
 */

using System;

namespace SerialController
{
    public class AnalogInputData
    {
        public bool Inverted = false;
        public int? InitialMin;
        public int? InitialMax;
        public int MinRange = 0;
        public int MaxRange = 1024;
        public double DeadZone = 0;
    }
    
    public class AnalogMapper
    {
        private bool Inverted;
        private double DeadZone;
        private int MinValue;
        private int MaxValue;
        private int MinRange;
        private int MaxRange;
        
        /*
         * Creates an analog mapper object.
         */
        public AnalogMapper(bool inverted,double deadZone = 0,int minRange = 0,int maxRange = 1024)
        {
            this.Inverted = inverted;
            this.DeadZone = deadZone;
            this.MinValue = int.MaxValue;
            this.MaxValue = 0;
            this.MinRange = minRange;
            this.MaxRange = maxRange;
        }
        
        /*
         * Creates an analog mapper.
         */
        public static AnalogMapper FromData(AnalogInputData data)
        {
            // Create the mapper.
            var mapper = new AnalogMapper(data.Inverted,data.DeadZone,data.MinRange,data.MaxRange);
            if (data.InitialMin != null && data.InitialMax != null)
            {
                mapper.SetBounds(data.InitialMin.Value,data.InitialMax.Value);
            }
            
            // Return the mapper.
            return mapper;
        }
        
        /*
         * Returns if the mapper has met the range requirements.
         */
        public bool MinimumRangeMet()
        {
            return this.MaxValue - this.MinValue >= this.MinRange;
        }
        
        /*
         * Resets the bounds.
         */
        public void Reset()
        {
            this.MinValue = int.MaxValue;
            this.MaxValue = 0;
        }
        
        /*
         * Sets the bounds of the mapper.
         */
        public void SetBounds(int min,int max)
        {
            this.MinValue = min;
            this.MaxValue = max;
        }
        
        /*
         * Returns the value for a given int value.
         */
        public byte GetValue(int value)
        {
            // Update the bounds.
            if (value > this.MaxValue && value - this.MinValue <= this.MaxRange)
            {
                this.MaxValue = value;
            }
            if (value < this.MinValue && this.MaxValue - value <= this.MaxRange)
            {
                this.MinValue = value;
            }
             
            // Return 0 if the bounds are the same (more information required).
            if (this.MinValue == this.MaxValue)
            {
                return 0;
            }
            
            // Determine the multipliers.
            var delta = this.MaxValue - this.MinValue;
            var deadZone = (int) (delta * this.DeadZone);
            var usableDelta = delta - (2 * deadZone);
            if (usableDelta == 0)
            {
                return 0;
            }
            var multiplier = (value - (deadZone + this.MinValue)) / (float) usableDelta;

            // Return the value.
            var mappedInput = Math.Max(Math.Min(multiplier * (float) byte.MaxValue,byte.MaxValue),0);
            if (this.Inverted)
            {
                mappedInput = byte.MaxValue - mappedInput;
            }
            return (byte) mappedInput;
        }
    }
}