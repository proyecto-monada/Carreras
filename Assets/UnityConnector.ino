#include <SerialCommand.h>

SerialCommand sCmd;

void setup() {
  Serial.begin(9600);
  while (!Serial);

}
int a=0;
void loop() {
  
  // Your operations here

  
  a++;
  Serial.println(a);

  delay(1000);  
}


