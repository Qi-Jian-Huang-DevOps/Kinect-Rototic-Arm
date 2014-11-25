#include <Servo.h> 

Servo servo1, servo2, servo3, servo4, servo5, servo6;  // create servo object to control a servo 
// twelve servo objects can be created on most boards
const int LED1 = 8, LED2 = 9, LED3 = 10;
int pos = 0;    // variable to store the servo position 
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
  servo2.attach(3); 
} 

void loop() 
{ 
  //initiaize the servos' positions for every loop
  servo1.write(pos);
  servo2.write(120 - pos);

  if (Serial.available()>0) {
    int input = Serial.read();

    if(input == 1){
      digitalWrite(LED1, LOW);
      digitalWrite(LED2, HIGH);
      if (pos < 120 && reverse == false) {
        pos+=10;
        if (pos == 120) {
          reverse = true;
        }
      } 
      else {
        pos-=10;
        if (pos == 0) {
          reverse = false;
        }
      }
      servo1.write(pos);
      servo2.write(120 - pos);
    } 
    else { // stop
      //pos = 0; // reset pos
      digitalWrite(LED1, HIGH);
      digitalWrite(LED2, LOW);
    } 
//    else {
//      digitalWrite(LED3, HIGH);
//      delay(100);
//      digitalWrite(LED3, LOW);
//      delay(100);
//    } 
  }
}













