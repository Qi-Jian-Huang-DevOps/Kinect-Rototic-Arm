#include <Servo.h> 

Servo baseServo, shoulderServo, elbowServo, wristServo, clamServo;  // create servo object to control a servo 
// twelve servo objects can be created on most boards
const int LED1 = 8, LED2 = 9, LED3 = 10;
const int DEG_DIFFERENCE = 5;
const int BASE = 1;
const int SHOULDER = 2;
const int ELBOW = 3;
const int WRIST = 4;
const int CLAM = 5;
int clamPos = 0;    // variable to store the servo position 
boolean reverse = false;

void setup() 
{ 
  Serial.begin(9600);

  // LEDS
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);
  //Servos
  baseServo.attach(2);
  shoulderServo.attach(3);
  elbowServo.attach(4); 
  wristServo.attach(5);
  clamServo.attach(6);
  
  clamPos = clamServo.read();
} 

void loop() 
{        
  //initiaize the servos' positions for every loop
  //servo1.write(pos1);
  //servo2.write(pos2);


  if (Serial.available() >= 5) {
//    int[] input = Serial.read();
    int input[5];
    for (int n = 1; n <= 5; n++) {
      input[n] = Serial.read();
    }
    
    // FOR BASE **********************************************
    if (input[BASE] > 0 && input[BASE] <= 180) {
       baseServo.write(input[BASE]); 
    } 
    else {
       // do nothing 
    }

   // FOR SHOULDER *****************************************************
    if (input[SHOULDER] > 30 && input[SHOULDER] <= 90) {
    shoulderServo.write(input[SHOULDER]);
    }
    else {
     // do nothing 
    }
    
    // FOR ELBOW
    if (input[ELBOW] >= 0 && input[ELBOW] <= 60) {
     elbowServo.write(60 - input[ELBOW]); 
    }
     
// FOR CLAM ************************************************************
    if (input[CLAM] >= 9) { // opening clam
      if (clamPos >= 180) { 
        // doNothing 
      } 
      else {
        clamPos += DEG_DIFFERENCE;
        clamServo.write(clamPos);
      }
    }
    else if (input[CLAM] >= 1 && input[CLAM] < 9){ // closing clam
      if (clamPos <= 0) { 
        // doNothing
      }
      else {
        clamPos -= DEG_DIFFERENCE;
        clamServo.write(clamPos);
      }
    } 
    else {
      // do nothing
    }
    
    // ******************************************************
//    
//    // FOR ELBOW
//    if (input >= 0 && input <= 60) {
//     servo1.write(input); 
//    }
    
//    // FOR BASE
//    if (input > 0 && input <= 180) {
//       servo1.write(input); 
//    } 
//    else {
//       // do nothing 
//    }
    
//    
//    // FOR SHOULDER
//    if (input > 0 && input <= 90) {
//    servo1.write(input);
//    }
//    else {
//     // do nothing 
//    }
    
// FOR CLAM
//    if (input >= 9) { // opening clam
//      if (pos1 >= 180) { 
//        // doNothing 
//      } 
//      else {
//        pos1 += DEG_DIFFERENCE;
//        servo1.write(pos1);
//      }
//    }
//    else if (input >= 1 && input < 9){ // closing clam
//      if (pos1 <= 0) { 
//        // doNothing
//      }
//      else {
//        pos1 -= DEG_DIFFERENCE;
//        servo1.write(pos1);
//      }
//    } 
//    else {
//      // do nothing
//    }
    //Serial.write(pos1); // send current servo positio
  }
} 

//int[] getIntArray() {
//  int[] dataArray = new int[2];
//  for (int n = 0; n < 2; n++) {
//    dataArray[n] = Serial.read();
//  }
//  return dataArray;
//}
























