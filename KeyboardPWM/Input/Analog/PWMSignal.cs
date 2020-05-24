/*
 * TheNexusAvenger
 *
 * Abstract class for a PWM signal.
 */

using System.Threading;

namespace KeyboardPWM.Input.Analog
{
    public abstract class PWMSignal
    {
        public readonly double Frequency;
        public double DutyCycle = 0.5;
        private bool Enabled;
        
        /*
         * Creates the PWM signal.
         */
        public PWMSignal(double frequency)
        {
            this.Frequency = frequency;
            this.Enabled = false;
        }
        
        /*
         * Starts the PWM signal.
         */
        public void Start()
        {
            if (!this.Enabled)
            {
                // Run the thread and start the loop.
                new Thread(() =>
                {
                    while (this.Enabled)
                    {
                        // Make the signal active.
                        if (this.DutyCycle != 0)
                        {
                            this.Active();
                            Thread.Sleep((int) ((1000 / this.Frequency) * this.DutyCycle));
                        }

                        // Make the signal inactive.
                        if (this.DutyCycle != 1)
                        {
                            this.Inactive();
                            Thread.Sleep((int) ((1000 / this.Frequency) * (1 - this.DutyCycle)));
                        }
                    }
                }).Start();
            }

            // Set it as enabled.
            this.Enabled = true;
        }
        
        /*
         * Stops the PWM signal.
         */
        public void Stop()
        {
            this.Enabled = false;
        }
        
        /*
         * Method called for signalling the start of an active signal.
         */
        public abstract void Active();
        
        /*
         * Method called for signalling the start of an inactive signal.
         */
        public abstract void Inactive();
    }
}