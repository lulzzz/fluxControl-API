#include <SoftwareSerial.h>
#include <Servo.h>

#define MAX_DISTANCE 15
#define MIN_DISTANCE 5

#define entradaServo 2
#define entradaTrigger 12
#define entradaEcho 13

#define saidaServo 3
#define saidaTrigger 7
#define saidaEcho 6

SoftwareSerial master(9, 10); // RX, TX
Servo entrada;
Servo saida;

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
  entrada.write(0); // 0 - FECHADO | 90 - ABERTO
  delay(50);
  
  // Sensor Ultrasonico - Entrada
  pinMode(entradaTrigger, OUTPUT);
  pinMode(entradaEcho, INPUT);

  // Servo Motor - Saida
  saida.attach(saidaServo);
  saida.write(90); // 90 - FECHADO | 180 - ABERTO
  delay(50);

  // Sensor Ultrasonico - Saida
  pinMode(entradaTrigger, OUTPUT);
  pinMode(entradaEcho, INPUT);

  pinMode(saidaTrigger, OUTPUT);
  pinMode(saidaEcho, INPUT);
 
  // Open serial communications and wait for port to open:
  Serial.begin(115200);
   
  // set the data rate for the SoftwareSerial port
  master.begin(115200);
}

Ultrasonic sensorEntrada = Ultrasonic(entradaTrigger, entradaEcho);
Ultrasonic sensorSaida = Ultrasonic(saidaTrigger, saidaEcho);

void loop() // run over and over
{
  int distanciaEntrada = sensorEntrada.GetDistance();
  int distanciaSaida = sensorSaida.GetDistance();

  Serial.print("Entrada: ");
  Serial.println(distanciaEntrada);

  Serial.print("Saida: ");
  Serial.println(distanciaSaida);
    
  if (distanciaEntrada >= MIN_DISTANCE && distanciaEntrada <= MAX_DISTANCE)
  {
    master.write("E"); 
    delay(3000);
  }

  if (distanciaSaida >= MIN_DISTANCE && distanciaSaida <= MAX_DISTANCE)
  {
    master.write("S"); 
    delay(3000);
  }
    
  if (master.available())
  {
    String response = master.readString();

    if (response.equals("E-OK"))
    {
      entrada.write(0);
      delay(5);
      entrada.write(90);
      delay(3000);
      entrada.write(0);
      delay(5);
      
    }
    else if (response.equals("S-OK"))
    {
      saida.write(180);
      delay(3000);
      saida.write(120);
      delay(5);
    }
  }
}
