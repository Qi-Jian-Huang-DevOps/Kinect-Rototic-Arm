#include <Servo.h> 

Servo baseServo, shoulderServo, elbowServo, wristServo, clamServo;  // create servo object to control a servo 
const int LED_7 = 7, LED_8 = 8, LED_9 = 9, LED_10 = 10, LED_11 = 11, LED_12 = 12, LED_13 = 13;
const int DEG_DIFFERENCE = 5;
const int BASE = 1;
const int SHOULDER = 2;
const int ELBOW = 3;
const int WRIST = 4;
const int CLAM = 5;
int clamPos = 0;    // variable to store the clam servo position 
boolean reverse = false;

void setup() 
{ 
  Serial.begin(9600);

  // LEDS
  pinMode(LED_7, OUTPUT);
  pinMode(LED_8, OUTPUT);
  pinMode(LED_9, OUTPUT);
  pinMode(LED_10, OUTPUT);
  pinMode(LED_11, OUTPUT);
  pinMode(LED_12, OUTPUT);
  pinMode(LED_13, OUTPUT);
  
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

  if (Serial.available() >= 5) {
    //    int[] input = Serial.read();
    int input[5];
    for (int n = 1; n <= 5; n++) {
      input[n] = Serial.read();
    }

    digitalWrite(LED_7, LOW);
    digitalWrite(LED_8, LOW);
    digitalWrite(LED_9, LOW);
    digitalWrite(LED_10, LOW);
    digitalWrite(LED_11, LOW);
    digitalWrite(LED_12, LOW);
    digitalWrite(LED_13, LOW);

    // FOR BASE **********************************************
    if (input[BASE] > 0 && input[BASE] <= 180) {
      baseServo.write(input[BASE]); 
    } 

    // FOR SHOULDER *****************************************************
    if (input[SHOULDER] > 0 && input[SHOULDER] <= 50) {
      shoulderServo.write(input[SHOULDER] * 2);

      // display LEDs depend on the shoulder placement
      if (input[SHOULDER] >= 0 && input[SHOULDER] <= 7){
        digitalWrite(LED_7, HIGH);
      }
      else if (input[SHOULDER] >= 7 && input[SHOULDER] <= 14) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
      }
      else if (input[SHOULDER] >= 21 && input[SHOULDER] <= 28) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
        digitalWrite(LED_9, HIGH);
      }
      else if (input[SHOULDER] >= 28 && input[SHOULDER] <= 35) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
        digitalWrite(LED_9, HIGH);
        digitalWrite(LED_10, HIGH);
      }
      else if (input[SHOULDER] >= 35 && input[SHOULDER] <= 42) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
        digitalWrite(LED_9, HIGH);
        digitalWrite(LED_10, HIGH);
        digitalWrite(LED_11, HIGH);
      }
      else if (input[SHOULDER] >= 42 && input[SHOULDER] <= 49) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
        digitalWrite(LED_9, HIGH);
        digitalWrite(LED_10, HIGH);
        digitalWrite(LED_11, HIGH);
        digitalWrite(LED_12, HIGH);
      }
      else if (input[SHOULDER] >= 49) {
        digitalWrite(LED_7, HIGH);
        digitalWrite(LED_8, HIGH);
        digitalWrite(LED_9, HIGH);
        digitalWrite(LED_10, HIGH);
        digitalWrite(LED_11, HIGH);
        digitalWrite(LED_12, HIGH);
        digitalWrite(LED_13, HIGH);
      }
    }

    // FOR ELBOW
    if (input[ELBOW] >= 0 && input[ELBOW] <= 60) {
      // 180 - 60
      elbowServo.write(180 - input[ELBOW] * 2); 
    }

    // FOR WRIST
    if (input[WRIST] > 0 && input[WRIST] <= 30) {
      // 0 - 150
      wristServo.write(180 - (180 - (input[WRIST] * 5))); 
    } 

    // FOR CLAM ************************************************************
    if (input[CLAM] >= 8) { // opening clam
      if (clamPos <= 50) { 
        // doNothing 
      } 
      else {
        clamPos -= DEG_DIFFERENCE;
        clamServo.write(clamPos);
      }
    }
    else if (input[CLAM] >= 1 && input[CLAM] < 8){ // closing clam
      if (clamPos >= 180) { 
        // doNothing
      }
      else {
        clamPos += DEG_DIFFERENCE;
        clamServo.write(clamPos);
      }
    } 
  }
} 



























