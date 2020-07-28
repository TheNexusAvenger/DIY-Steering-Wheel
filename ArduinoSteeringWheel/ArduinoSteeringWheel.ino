/*
 * TheNexusAvenger
 * 
 * Arduino controller for sending raw inputs from the analog and digital pins.
 * Only sends signals when the values change.
 */

// Pins for reading from the multiplexer.
int ANALOG_READ_PIN = A0;
int DIGITAL_READ_PIN = 3;

// Pins for setting the multiplexer.
int ANALOG_MULTIPLEXER_COUNTER_PINS[] = {4,5,6,7};
int DIGITAL_MULTIPLEXER_COUNTER_PINS[] = {8,9,10,11};

// Pins to read from the multiplexer.
int ANALOG_MULIPLEXED_CHANNEL_PINS[] = {2,3,4,5,6,8,9,10,11,12};
int DIGITAL_MULIPLEXED_CHANNEL_PINS[] = {2,3,4,5,6,8,9,10,11,12};

// Thresholds for filtering noise.
int NOISE_THREADHOLD[] = {
  1,1,1,1,1,1,1,1,1,1, // Digital inputs
  2,2,2,2,2,2,2,2,2,2, // Analog inputs
};



// Storage for the previous values to prevent re-sending unchanged values.
int lastValues[] = {
  0,0,0,0,0,0,0,0,0,0, // Digital inputs
  0,0,0,0,0,0,0,0,0,0, // Analog inputs
};



/*
 * Updates the value if it changes.
 */
void updateValue(String type,int input,int serialChannel,int value) {
  // Return if the value hasn't changed.
  if (abs(lastValues[input] - value) < NOISE_THREADHOLD[input]) {
    return;
  }

  // Set the last value and output a serial signal.
  lastValues[input] = value;
  Serial.println(type + serialChannel + "," + value);
}

/*
 * Sets the multiplexer state.
 */
void setMultiplexer(int value,int pins[]) {
  int remainder = value;
  for (int i = 0; i <= 3; i++) {
    digitalWrite(pins[i],remainder % 2);
    remainder = remainder / 2;
  }
}

/*
 * Updates an analog value if it changes.
 */
void updateAnalogValue(int input) {
  setMultiplexer(ANALOG_MULIPLEXED_CHANNEL_PINS[input],ANALOG_MULTIPLEXER_COUNTER_PINS);
  updateValue("A",input + 10,input,analogRead(ANALOG_READ_PIN));
}

/*
 * Updates a digital value if it changes.
 */
void updateDigitalValue(int input) {
  setMultiplexer(DIGITAL_MULIPLEXED_CHANNEL_PINS[input],DIGITAL_MULTIPLEXER_COUNTER_PINS);
  updateValue("D",input,input,digitalRead(DIGITAL_READ_PIN));
}

/*
 * Updates all the analog values.
 */
void updateAnalogValues() {
  for (int i = 0; i < 10; i++) {
    updateAnalogValue(i);
  }
}

/*
 * Updates all the digital values.
 */
void updateDigitalValues() {
  for (int i = 0; i < 10; i++) {
    updateDigitalValue(i);
  }
}

/*
 * Sets up the program.
 */
void setup() {
  // Wait for the serial to be set up.
  Serial.begin(9600);
  while (!Serial) {
    
  }

  // Set the pin modes.
  for (int i = 0; i <= 13; i++) {
    pinMode(i,OUTPUT);
  }
  pinMode(DIGITAL_READ_PIN,INPUT);
}

/*
 * Runs the loop for the program.
 */
void loop() {
  updateAnalogValues();
  updateDigitalValues();
}
