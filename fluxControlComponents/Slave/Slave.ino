#include <SoftwareSerial.h>
#include <Servo.h>

#define entradaServo 2
#define entradaTrigger 5
#define entradaEcho 6

SoftwareSerial master(9, 10); // RX, TX
Servo entrada;

struct Ultrasonic
{
  int _trigger;
  int _echo;

  Ultrasonic(int trigger, int echo)
  {
    _trigger = trigger;
    _echo = echo;  
  }

  int GetDistance()
  {
    digitalWrite(_trigger, LOW);
    delayMicroseconds(5);
  
    digitalWrite(_trigger, HIGH);
    delayMicroseconds(10);
    digitalWrite(_trigger, LOW);
  
    int duration = pulseIn(_echo, HIGH);
    int distance = duration * 0.034 / 2;
  
    return distance;
  }
};

void setup()
{
  // Servo Motor - Entrada
  entrada.attach(entradaServo);
  entrada.write(0);
  // Sensor Ultrasonico - Entrada
  pinMode(entradaTrigger, OUTPUT);
  pinMode(entradaEcho, INPUT);
 
  // Open serial communications and wait for port to open:
  Serial.begin(115200);
   
  // set the data rate for the SoftwareSerial port
  master.begin(115200);
}

Ultrasonic sensorEntrada = Ultrasonic(entradaTrigger, entradaEcho);

void loop() // run over and over
{
  //int distanciaEntrada = sensorEntrada.GetDistance();
  
  //if (distanciaEntrada >= 20 && distanciaEntrada <= 30)
    //master.write("ENTRADA"); 
    
  if (master.available())
  {
    String response = master.readString();

    if (response.indexOf("ENTRADA") > 0)
    {
      entrada.write(90);
      delay(3000);
      entrada.write(0);
      
    }
    else if (response.indexOf("SAIDA") > 0)
    {
      
    }
  }
}
