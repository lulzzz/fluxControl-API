// HttpClient
#include "WiFi.h"
#include "esp_http_client.h"
#include "esp_camera.h"
#include "Arduino.h"

// Pin definition for CAMERA_MODEL_AI_THINKER
#define PWDN_GPIO_NUM     32
#define RESET_GPIO_NUM    -1
#define XCLK_GPIO_NUM      0
#define SIOD_GPIO_NUM     26
#define SIOC_GPIO_NUM     27
#define Y9_GPIO_NUM       35
#define Y8_GPIO_NUM       34
#define Y7_GPIO_NUM       39
#define Y6_GPIO_NUM       36
#define Y5_GPIO_NUM       21
#define Y4_GPIO_NUM       19
#define Y3_GPIO_NUM       18
#define Y2_GPIO_NUM        5
#define FLASH              4
#define VSYNC_GPIO_NUM    25
#define HREF_GPIO_NUM     23
#define PCLK_GPIO_NUM     22

// WIFI
const char* ssid = "Souza";
const char* password =  "10041974";

const char* user_registration = "0";
const char* user_password = "!system@emurb!";

String TOKEN = "";

void setup() 
{
    pinMode(FLASH, OUTPUT);
   
    //WRITE_PERI_REG(RTC_CNTL_BROWN_OUT_REG, 0); //disable brownout detector
  
    Serial.begin(921600);

  // ---------------------------- CAM --------------------------------- //
    camera_config_t config;
    config.ledc_channel = LEDC_CHANNEL_0;
    config.ledc_timer = LEDC_TIMER_0;
    config.pin_d0 = Y2_GPIO_NUM;
    config.pin_d1 = Y3_GPIO_NUM;
    config.pin_d2 = Y4_GPIO_NUM;
    config.pin_d3 = Y5_GPIO_NUM;
    config.pin_d4 = Y6_GPIO_NUM;
    config.pin_d5 = Y7_GPIO_NUM;
    config.pin_d6 = Y8_GPIO_NUM;
    config.pin_d7 = Y9_GPIO_NUM;
    config.pin_xclk = XCLK_GPIO_NUM;
    config.pin_pclk = PCLK_GPIO_NUM;
    config.pin_vsync = VSYNC_GPIO_NUM;
    config.pin_href = HREF_GPIO_NUM;
    config.pin_sscb_sda = SIOD_GPIO_NUM;
    config.pin_sscb_scl = SIOC_GPIO_NUM;
    config.pin_pwdn = PWDN_GPIO_NUM;
    config.pin_reset = RESET_GPIO_NUM;
    config.xclk_freq_hz = 20000000;
    config.pixel_format = PIXFORMAT_JPEG; 
  
    if(psramFound())
    {
      config.frame_size = FRAMESIZE_UXGA; // FRAMESIZE_ + QVGA|CIF|VGA|SVGA|XGA|SXGA|UXGA
      config.jpeg_quality = 10;
      config.fb_count = 2;
    } 
    else 
    {
      config.frame_size = FRAMESIZE_SVGA;
      config.jpeg_quality = 12;
      config.fb_count = 1;
    }
  
    // Init Camera
    esp_err_t err = esp_camera_init(&config);
  
    if (err != ESP_OK) 
    {
      Serial.printf("Camera init failed with error 0x%x", err);
      return;
    }
  
  // ---------------------------- WIFI --------------------------------- //
    delay(4000);   //Delay needed before calling the WiFi.begin
 
    WiFi.begin(ssid, password); 

    //Check for the connection
    while (WiFi.status() != WL_CONNECTED) 
    { 
      delay(1000);
      Serial.println("Connecting to WiFi..");
    } Serial.println("Connected to the WiFi network");
    
    //TOKEN = makeLogin();
 
}

static esp_err_t take_send_photo()
{
    camera_fb_t *fb = NULL;
    esp_err_t res = ESP_OK;

    digitalWrite(FLASH, HIGH);
    delay(200);
    fb = esp_camera_fb_get();
    digitalWrite(FLASH, LOW);
    
    if (!fb)
    {
      Serial.println("Camera capture failed");
      esp_camera_fb_return(fb);
      return ESP_FAIL;
    }
   
    size_t fb_len = 0;
    if (fb->format != PIXFORMAT_JPEG)
    {
      Serial.println("Non-JPEG data not implemented");
      return ESP_FAIL;
    }
   
    esp_http_client_config_t config = {
      .url = "http://192.168.0.14:8080/API/FlowRecord/ProcessImageBytes",
    };
   
    esp_http_client_handle_t client = esp_http_client_init(&config);
    esp_http_client_set_post_field(client, (const char *)fb->buf, fb->len);
    esp_http_client_set_method(client, HTTP_METHOD_POST);
    esp_http_client_set_header(client, "Content-type", "application/octet-stream");
    esp_err_t err = esp_http_client_perform(client);
    if (err == ESP_OK)
      Serial.println("Frame uploaded");
    else
      Serial.printf("Failed to upload frame, error %d\r\n", err);
   
    esp_http_client_cleanup(client);
   
    esp_camera_fb_return(fb);
}
 
void loop() 
{
  //Check WiFi connection status
  if(WiFi.status() == WL_CONNECTED)
  {
    take_send_photo();
  }
  
  delay(1000 * 10); // wait 10 seconds  
}
