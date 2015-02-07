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
} 

void loop() 
{        
  //initiaize the servos' positions for every loop
  //servo1.write(pos1);
  //servo2.write(pos2);

  if (Serial.available() > 0) {
    int input = Serial.read();
    
    if (input >=0 && input <= 20) {
      servo1.write(15);
    } else if (input > 20 && input <=40) {
      servo1.write(30);
    } else if (input > 40 && input <=60) {
      servo1.write(45);
    } else if (input > 60 && input <=80) {
      servo1.write(60);
    } else if (input > 80 && input <=100) {
      servo1.write(75);
    } else if (input > 100 && input <=120) {
      servo1.write(90);
    } else if (input > 120 && input <=140) {
      servo1.write(105);
    } else if (input > 140 && input <=160) {
      servo1.write(120);
    }
//    case 0: 
//      if (pos1 >= 160) {
//      }
//      else {
//        pos1 += DEG_DIFFERENCE;
//        digitalWrite(LED1, LOW);
//        servo1.write(pos1);
//      }
//      break;
//    case 1: 
//      if (pos1 <= 30) {
//      }
//      else {
//        pos1-= DEG_DIFFERENCE;
//        digitalWrite(LED1, HIGH);
//        servo1.write(pos1);
//      }
//      break;  
   // } 
  }
}


















