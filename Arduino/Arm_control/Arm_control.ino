#include <Servo.h> 

Servo servo1, servo2, servo3, servo4, servo5, servo6;  // create servo object to control a servo 
// twelve servo objects can be created on most boards
const int LED1 = 8, LED2 = 9, LED3 = 10;
const int DEG_DIFFERENCE = 5;
int pos1 = 0, pos2 = 0;    // variable to store the servo position 
boolean reverse = false;

void setup() 
{ 
  Serial.begin(9600);

  // LEDS
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);
  //Servos
  servo1.attach(2);
  servo2.attach(4); 
  pos1 = servo1.read();
} 

void loop() 
{        
  //initiaize the servos' positions for every loop
  //servo1.write(pos1);
  //servo2.write(pos2);

  if (Serial.available() > 0) {
    int input = Serial.read();

    if (input > 60) { // opening clam
      if (pos1 >= 180) { 
        // doNothing 
      } 
      else {
        pos1 += DEG_DIFFERENCE;
        servo1.write(pos1);
      }
    }
    else { // closing clam
      if (pos1 <= 0) { 
        // doNothing
      }
      else {
        pos1 -= DEG_DIFFERENCE;
        servo1.write(pos1);
      }
    }
    Serial.write(pos1); // send current servo position
  }
} 























