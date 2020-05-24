/*
 * TheNexusAvenger
 *
 * Serial input for Ultimate Driving on Roblox.
 */

using System.Collections.Generic;
using InputSimulatorStandard.Native;
using KeyboardPWM.Input.Analog;
using KeyboardPWM.Inputs;

namespace KeyboardPWM.Serial
{
    public class RobloxUltimateDrivingSerialController : BaseSerialController
    {
        public const float THROTTLE_KEYBOARD_FREQUENCY = 10;
        
        public const byte ANALOG_STEERING_WHEEL_INPUT = 1;
        public const byte ANALOG_THROTTLE_PEDAL_INPUT = 2;
        public const byte ANALOG_BRAKE_PEDAL_INPUT = 3;
        public const byte ANALOG_TURN_SIGNAL_INPUT = 4;
        public const byte ANALOG_SHIFTER_INPUT = 5;
        
        public readonly Dictionary<byte,VirtualKeyCode> DIGITAL_IDS_TO_KEYS = new Dictionary<byte,VirtualKeyCode>()
        {
            {2,VirtualKeyCode.VK_Y}, // Cruise control
            {3,VirtualKeyCode.VK_R}, // Boost
            {4,VirtualKeyCode.VK_G}, // Horn
            {5,VirtualKeyCode.VK_H}, // Headlights
            {6,VirtualKeyCode.VK_J}, // High beams
            {7,VirtualKeyCode.VK_U}, // Emergency light toggle
            {8,VirtualKeyCode.VK_V}, // Move spoiler
            {9,VirtualKeyCode.VK_L}, // Lock
            {10,VirtualKeyCode.VK_F}, // Flip
            {11,VirtualKeyCode.VK_X}, // Hazards
        };

        private Keyboard Keyboard;
        private Gamepad Gamepad;
        
        private KeyboardPWMSignal BrakeSignal;
        private AnalogState TurnSignalStateController;
        private AnalogState ShifterStateController;

        private bool Enabled = false;
        private bool Reversing = false;
        private byte LastBrakeValue = 0;
        private byte LastThrottleValue = 0;
        private byte LastSteeringValue = 127;
        
        /*
         * Creates the serial controller.
         */
        public RobloxUltimateDrivingSerialController()
        {
            // Get the input devices.
            this.Keyboard = Keyboard.GetInstance();
            this.Gamepad = Gamepad.GetInstance();
            
            // Create the signals.
            this.BrakeSignal = new KeyboardPWMSignal(VirtualKeyCode.VK_B,THROTTLE_KEYBOARD_FREQUENCY);
            this.BrakeSignal.DutyCycle = 0;
            
            // Create the state controllers.
            this.TurnSignalStateController = new AnalogState(3,1,(newState,previousState) =>
            {
                if ((newState == 1 && previousState == 0) || newState == 0)
                {
                    this.Keyboard.SendKeyPress(VirtualKeyCode.VK_Z);
                } else if ((newState == 1 && previousState == 2) || newState == 2)
                {
                    this.Keyboard.SendKeyPress(VirtualKeyCode.VK_C);
                }
            });
            this.ShifterStateController = new AnalogState(3,1,(newState,previousState) =>
            {
                // Change the reversing state.
                if (newState == 0)
                {
                    this.Reversing = true;
                }
                else
                {
                    this.Reversing = false;
                }
                    
                // Press the parking key.
                if ((newState == 1 && previousState == 2) || newState == 2)
                {
                    this.Keyboard.SendKeyPress(VirtualKeyCode.VK_P);
                }
            });
        }
        
        /*
         * Updates the state of the inputs.
         */
        public void UpdateInputs()
        {
            if (this.Enabled)
            {
                // Update the steering.
                this.Gamepad.SetAnalogInput(AnalogInput.LeftThumbstickX,(short) ((this.LastSteeringValue * 256) - (short.MaxValue)));
                
                // Update the throttle.
                if (this.Reversing)
                {
                    this.Gamepad.SetAnalogInput(AnalogInput.RightTrigger,0);
                    this.Gamepad.SetAnalogInput(AnalogInput.LeftTrigger,(short) (((short) this.LastThrottleValue) * 128));
                }
                else
                {
                    this.Gamepad.SetAnalogInput(AnalogInput.LeftTrigger,0);
                    this.Gamepad.SetAnalogInput(AnalogInput.RightTrigger,(short) (((short) this.LastThrottleValue) * 128));
                }
                
                // Update the brake.
                this.BrakeSignal.DutyCycle = this.LastBrakeValue / 255.0;
                this.BrakeSignal.Start();
            }
            else
            {
                // Disable all analog inputs.
                this.Gamepad.SetAnalogInput(AnalogInput.LeftThumbstickX,0);
                this.Gamepad.SetAnalogInput(AnalogInput.LeftTrigger,0);
                this.Gamepad.SetAnalogInput(AnalogInput.RightTrigger,0);
                this.BrakeSignal.DutyCycle = 0;
                this.BrakeSignal.Stop();
                
                // Unplug the controller.
                this.Gamepad.Stop();
            }
        }
        
        /*
         * Starts the controller.
         */
        public override void Start()
        {
            if (!this.Enabled)
            {
                this.Enabled = true;
                this.UpdateInputs();
            }
        }
        
        /*
         * Stops the controller.
         */
        public override void Stop()
        {
            if (this.Enabled)
            {
                this.Enabled = false;
                this.UpdateInputs();
            }
        }
        
        /*
         * Invoked when an analog input changes.
         */
        public override void OnAnalogInput(byte id,byte value)
        {
            if (id == ANALOG_STEERING_WHEEL_INPUT)
            {
                this.LastSteeringValue = value;
                this.UpdateInputs();
            } else if (id == ANALOG_BRAKE_PEDAL_INPUT)
            {
                this.LastBrakeValue = value;
                this.UpdateInputs();
            } else if (id == ANALOG_THROTTLE_PEDAL_INPUT) {
                this.LastThrottleValue = value;
                this.UpdateInputs();
            } else if (id == ANALOG_TURN_SIGNAL_INPUT) {
                this.TurnSignalStateController.SetValue(value);
            } else if (id == ANALOG_SHIFTER_INPUT)
            {
                this.ShifterStateController.SetValue(value);
            }
        }
        
        /*
         * Invoked when an digital input changes.
         */
        public override void OnDigitalInput(byte id,bool value)
        {
            if (value)
            {
                if (this.DIGITAL_IDS_TO_KEYS.ContainsKey(id))
                {
                    this.Keyboard.SendKeyDown(this.DIGITAL_IDS_TO_KEYS[id]);
                }
            }
            else
            {
                if (this.DIGITAL_IDS_TO_KEYS.ContainsKey(id))
                {
                    this.Keyboard.SendKeyUp(this.DIGITAL_IDS_TO_KEYS[id]);
                }
            }
        }
    }
}