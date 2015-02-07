#include <Servo.h> 

Servo servo1, servo2, servo3, servo4, servo5, servo6;  // create servo object to control a servo 
// twelve servo objects can be created on most boards
const int LED1 = 8, LED2 = 9, LED3 = 10;
const int DEG_DIFFERENCE = 5;
int pos1 = 0, pos2 = 0;    // variable to store the servo position 
boolean reverse = false;

void setup() 
{ 
  Serial.begin(9200);

  // LEDS
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);
  //Servos
  servo1.attach(2);
  servo2.attach(4); 
} 

void loop() 
{ 
  //initiaize the servos' positions for every loop
  servo1.write(pos1);
  servo2.write(pos2);

  if (Serial.available() > 0) {
    int input = Serial.read();

    switch (input) {
      /*
       case 0: servo1 off 
       case 1: servo1 on
       case 2: servo2 off
       case 3: servo2 on
       */
    case 0: 
      if (pos1 >= 160) {
      }
      else {
        pos1 += DEG_DIFFERENCE;
        digitalWrite(LED1, LOW);
        servo1.write(pos1);
      }
      break;
    case 1: 
      if (pos1 <= 30) {
      }
      else {
        pos1-= DEG_DIFFERENCE;
        digitalWrite(LED1, HIGH);
        servo1.write(pos1);
      }
      break;
    case 2:
      if (pos2 <= 0) {
      }
      else {
        pos2 -= DEG_DIFFERENCE;
        digitalWrite(LED2, LOW);
        servo2.write(pos2);
      }
      break;
    case 3:
      if (pos2 >= 180) {
      }
      else {
        pos2 += DEG_DIFFERENCE;
        digitalWrite(LED2, HIGH);
        servo2.write(pos2);
      }
      break;
    } 
  }
}


















