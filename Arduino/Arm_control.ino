/* Sweep
 by BARRAGAN <http://barraganstudio.com> 
 This example code is in the public domain.
 
 modified 8 Nov 2013
 by Scott Fitzgerald
 http://arduino.cc/en/Tutorial/Sweep
 */

#include <Servo.h> 

Servo servo1, servo2, servo3, servo4, servo5, servo6;  // create servo object to control a servo 
// twelve servo objects can be created on most boards

int pos = 0;    // variable to store the servo position 

void setup() 
{ // LEDS
  pinMode(8, OUTPUT);
  pinMode(9, OUTPUT);
  //Servos
  servo1.attach(2);
  servo2.attach(3); 
} 

void loop() 
{ 
  //initiaize the servos' positions for every loop
  servo1.write(pos);
  servo2.write(180 - pos);

  for(pos = 0; pos <= 180; pos += 1) // goes from 0 degrees to 180 degrees 
  {                                  // in steps of 1 degree 
    servo1.write(pos);
    servo2.write(180 - pos);    // tell servo to go to position in variable 'pos' 
    digitalWrite(8, HIGH);
    digitalWrite(9, LOW);
    delay(15);                       // waits 15ms for the servo to reach the position 
  } 
  for(pos = 180; pos>=0; pos-=1)     // goes from 180 degrees to 0 degrees 
  {                                
    servo1.write(pos); 
    servo2.write(180 - pos);
    digitalWrite(8, LOW);
    digitalWrite(9, HIGH);    // tell servo to go to position in variable 'pos' 
    delay(15);                       // waits 15ms for the servo to reach the position 
  } 
} 






