/*
 * TheNexusAvenger
 * 
 * Arduino controller for sending raw inputs from the analog and digital pins.
 * Only sends signals when the values change.
 */


// Thresholds for marking a value as changed.
int noiseThreshold[] = {
  1,1,1,1,1,1,1,1,1,1,1,1,1,1, // Digital inputs (0-13)
  2,2,2,2,2,2,                 // Analog inputs (14-19)
};

// Storage for the initial values.
int lastValues[] = {
  0,0,0,0,0,0,0,0,0,0,0,0,0,0, // Digital inputs (0-13)
  0,0,0,0,0,0,                 // Analog inputs (14-19)
};

/*
 * Updates the value if it changes.
 */
void updateValue(String type,int input,int serialChannel,int value) {
  // Return if the value hasn't changed.
  if (abs(lastValues[input] - value) < noiseThreshold[input]) {
    return;
  }

  // Set the last value and output a serial signal.
  lastValues[input] = value;
  Serial.println(type + serialChannel + "," + value);
}

/*
 * Updates an analog value if it changes.
 */
void updateAnalogValue(int input) {
   updateValue("A",input,input - A0,analogRead(input));
}

/*
 * Updates a digital value if it changes.
 */
void updateDigitalValue(int input) {
  updateValue("D",input,input,digitalRead(input));
}

/*
 * Updates all the analog values.
 */
void updateAnalogValues() {
  for (int i = A0; i <= A5; i++) {
    updateAnalogValue(i);
  }
}

/*
 * Updates all the digital values.
 */
void updateDigitalValues() {
  for (int i = 2; i <= 13; i++) {
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

  // Set the pin modes as input.
  for (int i = 0; i <= 13; i++) {
    pinMode(i,INPUT);
  }
}

/*
 * Runs the loop for the program.
 */
void loop() {
  updateAnalogValues();
  updateDigitalValues();
}
