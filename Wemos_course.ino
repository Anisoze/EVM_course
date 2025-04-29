// библиотека датчика mhz19
#include <MHZ19_uart.h>     // CO2
#define MHZ_RX D3           // Rx пин на датчике CO2
#define MHZ_TX D4           // Tx пин на датчике CO2
MHZ19_uart mhz19;           // объект CO2 датчика

// подключения для дисплея
#include <Adafruit_GFX.h>         // основная графическая библиотека
#include <Adafruit_ST7789.h>      // библиотека для чипа ST7789 на дисплее
#include <SPI.h>                  // SPI библиотека Arduino 
#define TFT_CS D8                 // пин для выбора в SPI, но не используется, так как на дисплее всегда подтянут к 1 (некоторео значение должно быть задано для работы функции)
#define TFT_DC D8                 // для пина DC дисплея
#define TFT_RST D6                // для пина RES дисплея
Adafruit_ST7789 tft = Adafruit_ST7789(TFT_CS, TFT_DC, TFT_RST); // инициализация объекта дисплея

// библиотеки для модуля Wemos
#include <ESP8266WiFi.h>                   // библиотеки
#include <ESP8266WiFiMulti.h>
const char* WIFI_SSID = "RT-WiFi-D9F8";   // имя узла WiFi
const char* WIFI_PASS = "F8ij6j9eda";     // пароль узла WiFi

// настройки сервера
const char* host = "192.168.0.10";        // ip сервера
const uint16_t port = 49160;              // порт сервера
ESP8266WiFiMulti WiFiMulti;               
IPAddress ip;
WiFiClient client;

// библиотека I2C
#include <Wire.h> 

// библиотека датчика температуры
#include <Adafruit_Sensor.h> 
#include <Adafruit_BME280.h>
#define SEALEVELPRESSURE_HPA (1013.25)  
Adafruit_BME280 bme; // инициализация объекта датчика




// переменные
uint8_t Temp;     // значение параметра (или среднего) температуры для всех Режимов
uint8_t Hum;      // влажности
uint16_t Pres;    // давления
uint16_t CO2;     // углекислого газа

bool TempOn = true;     // активность параметра температуры
bool HumOn = true;      // влажности
bool PresOn = true;     // давления
bool CO2On = true;      // углекислого газа

uint8_t mode = 5;       // текущий Режим
uint8_t Byte;           // вспомогательная переменная для работы с байтами
String binStr = "";    // бинарная строка для чтения управляющего пакета
uint8_t Buffer[4];      // буфер для чтения нескольких байт

unsigned long CurTimer;          // таймер для отслеживания времени
unsigned long Interval;          // временной интервал между замерами
bool WaitToGetTime = true;       // флаг ожидания данных о временном интервале
bool WaitToGetNextInfo = true;   // флаг ожидания данных о количестве измерений
bool GaveASignal = false;        // флаг подачи сигнала

uint8_t NumOfParams = 0;         // количество параметров
uint8_t TempSensitivity;         // чувствительность по температуре 
uint8_t HumSensitivity;          // влажности 
uint8_t PresSensitivity;         // давлению
uint8_t CO2Sensitivity;          // углекислому газу

bool GetInfo;                    // флаг о ожидании данных о чувствительности
bool NewTemp = false;            // новая динамика по параметру температуры
bool NewHum = false;             // влажности 
bool NewPres = false;            // давления
bool NewCO2 = false;             // углекислого газа

uint8_t NumOfMeasurements;    // количество измерений для Режима Поиска Динамики и Углубленного Мониторинга
uint8_t CurMeasurement = 0;   // номер текущего измерения
uint32_t Sum = 0;             // вспомогательная переменная для нахождения среднего значения

uint8_t* TempArray = nullptr;   // массив значений температуры    для режимов Поиска Динамики и Углубленного Мониторинга
uint8_t* HumArray = nullptr;    // влажности
uint16_t* PresArray = nullptr;  // давления
uint16_t* CO2Array = nullptr;   // углекислого газа

uint8_t MinTemp = 255;        // минимальная температура    для Режима Углубленного Мониторинга
uint8_t MinHum  = 255;        // влажность
uint16_t MinPres  = 65000;    // давление
uint16_t MinCO2 = 65000;      // углекислый газ

uint8_t MaxTemp = 0;       // максимальная температура      для Режима Углубленного Мониторинга
uint8_t MaxHum = 0;        // влажность
uint16_t MaxPres = 0;      // давление
uint16_t MaxCO2 = 0;       // углекислый газ



// считывание значения параметра температуры или влажности 
void readSensorsPart_8(uint8_t type, uint8_t* Param, uint8_t* Array, uint8_t* MinParam, uint8_t* MaxParam){
  switch(type){
    case 0:
      *Param = bme.readTemperature();                    // считывание температуры
      break;
    case 1:
      *Param = bme.readHumidity();                       // считывание влажности
      break;
  }
  if(mode==2){                                           // Режим Поиска Динамики
    for(int i = 0; i<CurMeasurement; i++){
      Array[i] = Array[i+1];                             // смещение старых данных к началу массива
    }
    Array[(CurMeasurement==NumOfMeasurements) ? CurMeasurement : CurMeasurement+1] = *Param;      // запись нового значения в конец массива
  }
  else if(mode==3){                                      // Режим Углубленного Мониторинга
    Array[CurMeasurement] = *Param;                      // запись в массив
    if(*Param<*MinParam) *MinParam = *Param;             // перезапись наименьшего
    if(*Param>*MaxParam) *MaxParam = *Param;             // наибольшего
  }
}


// считывание значения параметра давления или углекислого газа
void readSensorsPart_16(uint16_t type, uint16_t* Param, uint16_t* Array, uint16_t* MinParam, uint16_t* MaxParam){
  switch(type){
    case 2:
      *Param = (float)bme.readPressure() * 0.00750062;    // считывание и перевод давления
      break;
    case 3:
      *Param = mhz19.getPPM();                            // считывание углекислого газа
      break;
  }
  if(mode==2){                                            // Режим Поиска Динамики
    for(int i = 0; i<CurMeasurement; i++){
      Array[i] = Array[i+1];                              // смещение старых данных к началу массива
    }
    Array[(CurMeasurement==NumOfMeasurements) ? CurMeasurement : CurMeasurement+1] = *Param;      // запись нового значения в конец массива
  }
  else if(mode==3){                                       // Режим Углубленного Мониторинга
    Array[CurMeasurement] = *Param;                       // запись в массив
    if(*Param<*MinParam) *MinParam = *Param;              // перезапись наименьшего
    if(*Param>*MaxParam) *MaxParam = *Param;              // наибольшего
  }
}


// считывания информации с датчиков
void readSensors() {
  if(TempOn || HumOn || PresOn){                                                // активен хотя бы один из параметров датчика BME280
    bme.takeForcedMeasurement();                                                // взятие замеров с BME280
    if(TempOn) readSensorsPart_8(0, &Temp, TempArray, &MinTemp, &MaxTemp);      // запись параметра температуры
    if(HumOn) readSensorsPart_8(1, &Hum, HumArray, &MinHum, &MaxHum);           // влажности
    if(PresOn) readSensorsPart_16(2, &Pres, PresArray, &MinPres, &MaxPres);     // давления
  }
  if(CO2On) readSensorsPart_16(3, &CO2, CO2Array, &MinCO2, &MaxCO2);            // углекислого газа
}





// вывод информации с датчиков на дисплей устройства
void displayData(){
  tft.fillScreen(ST77XX_BLACK);     // заливает экран чёрным цветом
  if(TempOn){
    tft.setCursor(0, 0);            // устанавливает курсор в левый верхний угол
    tft.print("Temp: ");            // выводит на экран температуру
    tft.print(Temp);
    tft.println(" C");  
  }
  if(HumOn){
    tft.setCursor(0, 0 + (TempOn ? 40 : 0));          // смещает курсор 
    tft.print("Humidity: ");                          // влажность
    tft.print(Hum);
    tft.println("%");  
  }
  if(PresOn){
    tft.setCursor(0, 0 + (TempOn ? 40 : 0) + (HumOn ? 40 : 0));    // смещает курсор 
    tft.print("Press: ");                                          // давление
    tft.print(Pres);
    tft.println(" mm");
  }
  if(CO2On){
    tft.setCursor(0, 0 + (TempOn ? 40 : 0) + (HumOn ? 40 : 0) + (PresOn ? 40 : 0));     // смещает курсор 
    tft.print("CO2: ");                                                                 // углекислый газ
    tft.print(CO2);
    tft.println(" ppm");
  } 
}


// отправляет данные о температуре либо влажности на сервер
void SendDataPart_8(uint8_t type, bool New, uint8_t Param, uint8_t MinParam, uint8_t MaxParam){
  if(mode!=2 || New){                                                                                 // все Режимы кроме Поиска Динамики либо найдена новая Динамика
    switch(type){
      case 0:
        Serial.print("Temp = ");
        break;
      case 1:
        Serial.print("Hum = ");
        break;
    }
    Serial.println(Param);
    client.write(Param);               // отправка данных на сервер

    if(mode==3){                       // Углубленный Мониторинг
      switch(type){
        case 0:
          Serial.print("MinTemp = ");
          break;
        case 1:
          Serial.print("MinHum = ");
          break;
      }
      Serial.println(MinParam);
      switch(type){
        case 0:
          Serial.print("MaxTemp = ");
          break;
        case 1:
          Serial.print("MaxHum = ");
          break;
      }
      Serial.println(MaxParam);
      client.write(MinParam);       // отправка минимума и максимума на сервер
      client.write(MaxParam);
    }
  }
}


// отправляет данные о давлении либо углекислом газе на сервер
void SendDataPart_16(uint8_t type, bool New, uint16_t Param, uint16_t MinParam, uint16_t MaxParam){
  if(mode!=2 || New){             // все Режимы кроме Поиска Динамики либо найдена новая Динамика
    switch(type){
      case 2:
        Serial.print("Pres = ");
        break;
      case 3:
        Serial.print("CO2 = ");
        break;
    }
    Serial.println(Param);
    client.write((Param >> 8) & 255);   // отправка данных на сервер по частям
    client.write(Param  & 255);         

    if(mode==3){                       // Углубленный Мониторинг
      switch(type){
        case 2:
          Serial.print("MinPres = ");
          break;
        case 3:
          Serial.print("MinCO2 = ");
          break;
      }
      Serial.println(MinParam);
      switch(type){
        case 2:
          Serial.print("MaxPres = ");
          break;
        case 3:
          Serial.print("MaxCO2 = ");
          break;
      }
      Serial.println(MaxParam);
      client.write((MinParam >> 8) & 255);
      client.write(MinParam  & 255);           // отправка минимума и максимума на сервер
      client.write((MaxParam >> 8) & 255);
      client.write(MaxParam  & 255);
    }
  }
}




// отправка данных на сервер и вывод в консоль
void SendData(){
  if(TempOn) SendDataPart_8(0, NewTemp, Temp, MinTemp, MaxTemp);
  if(HumOn) SendDataPart_8(1, NewHum, Hum, MinHum, MaxHum);
  if(PresOn) SendDataPart_16(2, NewPres, Pres, MinPres, MaxPres);
  if(CO2On) SendDataPart_16(3, NewCO2, CO2, MinCO2, MaxCO2);
  Serial.println();
}






// запуск устройства
void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);

  WiFi.mode(WIFI_STA);                            // подключение к WiFi
  Serial.print("Connecting to ");
  Serial.print(WIFI_SSID);
  WiFiMulti.addAP(WIFI_SSID, WIFI_PASS);
  while(WiFiMulti.run() != WL_CONNECTED){         // ожидание
    delay(100);
    Serial.print(".");
  }
  Serial.println();
  Serial.println("Connected! IP address: ");
  Serial.println(WiFi.localIP());                 // выводит в консоль IP адрес модуля Wemos

  tft.init(240, 240, SPI_MODE2);                  // активация дисплея
  tft.setRotation(3);                             //установка корректной ориентации (дисплей квадратный, может быть повернут в любую сторону)
  tft.setTextSize(3);                             // установка размера текста
  tft.setTextColor(ST77XX_WHITE);                 // установка цвета текста

  mhz19.begin(MHZ_TX, MHZ_RX);                    // активация датчика CO2
  bme.begin();                                    // активация датчика BME280

  bme.setSampling(Adafruit_BME280::MODE_FORCED,   // настройка датчика BME280
                  Adafruit_BME280::SAMPLING_X1, 
                  Adafruit_BME280::SAMPLING_X1, 
                  Adafruit_BME280::SAMPLING_X1, 
                  Adafruit_BME280::FILTER_OFF   );


  
  readSensors();                                  // первоначальное считывание параметров
  Serial.print("Temperature (deg C) is: ");
  Serial.println(Temp);
  Serial.print("Humidity (%) is: ");
  Serial.println(Hum);                            // вывод в консоль
  Serial.print("Pressure (mm) is: ");
  Serial.println(Pres);
  Serial.print("CO2 (ppm) is: ");
  Serial.println(CO2);
  Serial.println();
  displayData();                                  // вывод на дисплей          
}



// получение временного интервала между измерениями
void GetTimeInterval(bool send){
  if(!GaveASignal){                                   
    Serial.println("waiting to get time control...");    // сообщение в консоль об ожидании
    GaveASignal = true;
  }
  if(client.available()>=4){                          // ожидание 4 байт содержащих данные о временном интервале
    client.readBytes(Buffer, 4);
    Interval = (Buffer[0] << 0)  | (Buffer[1] << 8)  | (Buffer[2] << 16) | (Buffer[3] << 24);
    Serial.print("new Time Interval = ");
    Serial.println(Interval);
    WaitToGetTime = false;
    if(send) client.write(255); // Подтверждение принятия данных
    CurTimer = millis();
  }
}



// удаляет массив температуры либо влажности
void DeleteArray_8(uint8_t* Array){
  delete[] Array;
  Array = nullptr;
}


// удаляет массив давления либо углекислого газа
void DeleteArray_16(uint16_t* Array){
  delete[] Array;
  Array = nullptr;
}



// считывает данные из буфера
void ReadNewData(bool withArray){
  Serial.println("New Data");
  Byte = static_cast<uint8_t>(client.read());     // считывание и перевод в байт
  Serial.println(Byte);                           // вывод в консоль

  if(Byte==0){                                    
    mode = 5;                                     // сброс режима
    client.write(255);                            // отправка на сервер подтверждения о прочтении новых данных
    if(withArray){
      CurMeasurement = 0;
      if(TempOn) DeleteArray_8(TempArray);
      if(HumOn) DeleteArray_8(HumArray);
      if(PresOn) DeleteArray_16(PresArray);
      if(CO2On) DeleteArray_16(CO2Array);
    }
    Serial.println("mode reset");
    while(client.available()){                    // очистка буфера от остатков данных
      client.read();
    } 
  } 
  else if(Byte==255) Serial.println("Data from sensors was send successfully");       // подтверждение получения сервером данных
}


// создает массив для записей температуры либо влажности и заполняет нулями
void ReadNumOfMeasurementsPart_8(uint8_t*& Array){
  Array = new uint8_t[NumOfMeasurements + ((mode == 2) ? 1 : 0)];
  for(int i = 0; i<NumOfMeasurements + ((mode == 2) ? 1 : 0); i++){
  Array[i] = 0;
  }
}


// создает массив для записей давления либо углекислого газа и заполняет нулями
void ReadNumOfMeasurementsPart_16(uint16_t*& Array){
  Array = new uint16_t[NumOfMeasurements + ((mode == 2) ? 1 : 0)];
  for(int i = 0; i<NumOfMeasurements + ((mode == 2) ? 1 : 0); i++){
  Array[i] = 0;
  }
}


// чтение количества измерений и создание массивов для их хранения
void ReadNumOfMeasurements(){
  if(client.available()){
    NumOfMeasurements = static_cast<uint8_t>(client.read());
    WaitToGetNextInfo = false;
    Serial.print("new Number of Measurements = ");
    Serial.println(NumOfMeasurements);
    if(TempOn) ReadNumOfMeasurementsPart_8(TempArray);   
    if(HumOn) ReadNumOfMeasurementsPart_8(HumArray);           // создание массивов
    if(PresOn) ReadNumOfMeasurementsPart_16(PresArray);
    if(CO2On) ReadNumOfMeasurementsPart_16(CO2Array);
    client.write(255);                                         // подтверждение принятия данных
    CurTimer = millis();                                       // обновление таймера
  }
}



// вывод массива температуры либо влажности в консоль
void PrintArray_8(bool type, uint8_t* Array){
  if(type) Serial.print("TempArray = ");
  else Serial.print("HumArray = ");
  for(int i = 0; i<NumOfMeasurements + ((mode==2) ? 1 : 0); i++){
    Serial.print(Array[i]);
    Serial.print(" ");
    if(i == (NumOfMeasurements - ((mode == 3) ? 1 : 0))) Serial.println(" ");
  }
}


// вывод массива давления либо углекислого газа в консоль
void PrintArray_16(bool type, uint16_t* Array){
  if(type) Serial.print("PresArray = ");
  else Serial.print("CO2Array = ");
  for(int i = 0; i<NumOfMeasurements + ((mode==2) ? 1 : 0); i++){
    Serial.print(Array[i]);
    Serial.print(" ");
    if(i == (NumOfMeasurements - ((mode == 3) ? 1 : 0))) Serial.println(" ");
  }
}


// выводит все массивы в консоль
void PrintOutArrays(){
  if(TempOn) PrintArray_8(true, TempArray);
  if(HumOn) PrintArray_8(false, HumArray);
  if(PresOn) PrintArray_16(true, PresArray);
  if(CO2On) PrintArray_16(false, CO2Array);
}



// нахождение модуля разницы между двумя значениями температуры или влажности
uint8_t Difference_8(uint8_t a, uint8_t b){
  return (a>b) ? a-b : b-a;
}


// нахождение модуля разницы между двумя значениями давления или углекислого газа
uint16_t Difference_16(uint16_t a, uint16_t b){
  return (a>b) ? a-b : b-a;
}


// нахождение среднего для температуры либо влажности
uint8_t GetAverage_8(uint8_t* Array, uint8_t Num){
  Sum = 0;                                                 //сброс суммы элементов массива
  for(int i = 0; i<Num; i++){
    Sum+=Array[i];                                         // нахождение суммы элементов массива
  }
  return Sum / Num;                                        // нахождение среднего по массиву
}
  

// нахождение среднего для давления либо углекислого газа
uint16_t GetAverage_16(uint16_t* Array, uint8_t Num){
  Sum = 0;                                                  //сброс суммы элементов массива
  for(int i = 0; i<Num; i++){
    Sum+=Array[i];                                          // нахождение суммы элементов массива
  }
  return Sum / Num;                                         // нахождение среднего по массиву
}




// поиск новой Динамики температуры либо влажности
void FindDynamicsPart_8(bool type, uint8_t* Array, bool* New, uint8_t Sensitivity){
  Sum = GetAverage_8(Array, CurMeasurement);
  *New = (Difference_8(Sum, Array[CurMeasurement]) >= Sensitivity) ? true : false;
  if(type){
    Serial.print("Average Temp = ");
    Byte += *New ? 8 : 0;
  }  
  else{
    Serial.print("Average Hum = ");
    Byte += *New ? 4 : 0;
  }
  Serial.println(Sum);
}


// поиск новой Динамики давления либо углекислого газа
void FindDynamicsPart_16(bool type, uint16_t* Array, bool* New, uint8_t Sensitivity){
  Sum = GetAverage_16(Array, CurMeasurement);
  *New = (Difference_16(Sum, Array[CurMeasurement]) >= Sensitivity) ? true : false;
  if(type){
    Serial.print("Average Pres = ");
    Byte += *New ? 2 : 0;
  }  
  else{
    Serial.print("Average CO2 = ");
    Byte += *New ? 1 : 0;
  }
  Serial.println(Sum);
}


// поиск всех новых Динамик
void FindDynamics(){
  if(TempOn) FindDynamicsPart_8(true, TempArray, &NewTemp, TempSensitivity);
  if(HumOn) FindDynamicsPart_8(false, HumArray, &NewHum, HumSensitivity);
  if(PresOn) FindDynamicsPart_16(true, PresArray, &NewPres, PresSensitivity);
  if(CO2On) FindDynamicsPart_16(false, CO2Array, &NewCO2, CO2Sensitivity);
}






// основной цикл
void loop() {
  while(!client.connected()){         // проверка подключение
    mode = 5;                         // сброс Режима
    client.connect(host, port);       // повторное подключение
  }

  if(mode == 5){                                      // Режим Ожидания
    if (client.available()) {                         // проверка наличия данных
      Serial.println("New Data");                     // сообщение о новых данных
      Byte = static_cast<uint8_t>(client.read());     // преобразование в байт
      if(Byte != 0){                                  // проверка на наличие управляющего пакета   
        binStr="";                                    // обнуление бинарной строки
        for (int i = 7; i >= 0; i--) {
          binStr += ((Byte >> i) & 1) ? '1' : '0';   // составление бинарной строки
        }
        Serial.println(binStr);                       // вывод бинарной строки в консоль  

        TempOn = (binStr[0] == '1');                                                    
        Serial.println(TempOn ? "Temperature sensor on" : "Temperature sensor off");    
        HumOn = (binStr[1] == '1');
        Serial.println(HumOn ? "Humidity sensor on" : "Humidity sensor off");
        PresOn = (binStr[2] == '1');                                                    //Выбор включенных датчиков
        Serial.println(PresOn ? "Pressure sensor on" : "Pressure sensor off");
        CO2On = (binStr[3] == '1');
        Serial.println(CO2On ? "CO2 sensor on" : "CO2 sensor off");

        if(binStr[4] == '0'){                         //Выбор Режима
          mode = (binStr[5] == '0') ? 0 : 3;          // Стандартный Мониторинг либо Углубленный Мониторинг
        }
        else{
          mode = (binStr[5] == '0') ? 1: 2;           // По Команде либо Поиск Динамики
        }
        NumOfParams = (TempOn ? 1 : 0) + (HumOn ? 1 : 0) + (PresOn ? 1 : 0) + (CO2On ? 1 : 0);      // подсчет количества параметров
        Serial.print("Change mode: ");
        Serial.println(mode);               // вывод текущего Режима в консоль
        if(mode==1) client.write(255);      // Подтверждение принятия данных для Режима По Команде
        WaitToGetTime = true;               
        WaitToGetNextInfo = true;           // обновление флагов
        GaveASignal = false;      
        GetInfo = true;
      }
    }
  }



  //Стандартный мониторинг
  else if(mode == 0){            
    if(WaitToGetTime){
      GetTimeInterval(true);            // получение временного интервала
    }
    else{
      if (client.available()) {         // Проверка наличия данных в буфере
        ReadNewData(false);             // считывание данных из буфера
      }
      if(millis() - CurTimer >= Interval){    // по истечению временного интервала
        readSensors();                        // считывание данных с сенсоров
        displayData();                        // вывод данных на дисплей   
        SendData();                           // отправление данных на сервер
        CurTimer = millis();                  // обновление таймера
      }
    }
  }



  //Измерение по требованию
  else if (mode == 1){      
    if (client.available()) { // проверка наличия данных в буфере
      ReadNewData(false);     // считывание данных из буфера
    }
    if(!GaveASignal){         // если ещё не отправил данные
      readSensors();          // считывание данных с сенсоров
      displayData();          // вывод данных на дисплей       
      SendData();             // отправление данных на сервер
      GaveASignal = true;     // изменение флага
    }
  }





  //Поиск Динамики
  else if (mode == 2){     
    if(WaitToGetTime){
      GetTimeInterval(false);                      // получение временного интервала
    }
    else{
      if(GetInfo){                                 // получение чувствительности
        if(client.available()>=NumOfParams){
          client.readBytes(Buffer, NumOfParams);
          GetInfo = false;
          Serial.println("new Sensitivity controls");    
          if(TempOn){
            TempSensitivity = static_cast<uint8_t>(Buffer[0]);
            Serial.print("Temp Sensitivity = ");
            Serial.println(TempSensitivity);
          }
          if(HumOn){
            HumSensitivity = static_cast<uint8_t>(Buffer[(TempOn ? 1 : 0)]);
            Serial.print("Hum Sensitivity = ");
            Serial.println(HumSensitivity);
          }
          if(PresOn){
            PresSensitivity = static_cast<uint8_t>(Buffer[(TempOn ? 1 : 0) + (HumOn ? 1 : 0)]);
            Serial.print("Pres Sensitivity = ");
            Serial.println(PresSensitivity);
          } 
          if(CO2On){
            CO2Sensitivity = static_cast<uint8_t>(Buffer[(TempOn ? 1 : 0) + (HumOn ? 1 : 0) + (PresOn ? 1 : 0)]);
            Serial.print("CO2 Sensitivity = ");
            Serial.println(CO2Sensitivity);
          } 
        }
      }
      else{
        if(WaitToGetNextInfo){
          ReadNumOfMeasurements();                // получение количества измерений
        }
        else{
          if (client.available()) {               // проверка наличия данных в буфере
            ReadNewData(true);                    // считывание данных из буфера
          }

          if(millis() - CurTimer >= Interval){                                  // по истечению временного интервала
            readSensors();                                                      // считывание данных с сенсоров
            PrintOutArrays();                                                   // вывод массивов в консоль
            displayData();                                                      // вывод на дисплей     
            if(CurMeasurement < NumOfMeasurements) CurMeasurement+=1;
            Byte = 0;                                                           // сброс байта обозначающего новые динамики
            FindDynamics();                                                     // нахождение новых Динамик
            Serial.println();
            client.write(Byte);                                                 // на сервер отправляется байт, содержащий информацию о наличии новых динамик
            if(Byte!=0){
              SendData();                                                       // при наличии динамик замеры отправляются на сервер
            }
            CurTimer = millis();                                                // обновление таймера
          }
        }
      }
    }
  }



  //Углубленный мониторинг
  else if (mode == 3){     
    if(WaitToGetTime){
      GetTimeInterval(false);                           // получение временного интервала
    }
    else{
      if(WaitToGetNextInfo){
        ReadNumOfMeasurements();                        // получение количества измерений
      }
      else{
        if (client.available()) {                       // проверка наличия данных в буфере
          ReadNewData(true);                            // считывание данных из буфера
        }

        if(millis() - CurTimer >= Interval){            // по истечению временного интервала
          readSensors();                                // считывание данных с сенсоров
          Serial.print("Measurement #");
          Serial.println(CurMeasurement);
          PrintOutArrays();                             // вывод массивов в консоль
          displayData();                                // вывод на дисплей   
          CurMeasurement+=1;
          if(CurMeasurement == NumOfMeasurements){                          // накапливание достаточного количества замеров
            if(TempOn) Temp = GetAverage_8(TempArray, NumOfMeasurements);
            if(HumOn) Hum = GetAverage_8(HumArray, NumOfMeasurements);
            if(PresOn) Pres = GetAverage_16(PresArray, NumOfMeasurements);  // нахождение среднего
            if(CO2On) CO2 = GetAverage_16(CO2Array, NumOfMeasurements);
            SendData();                                                     // отправление данных на сервер
            if(TempOn) {
              MinTemp = 255;
              MaxTemp = 0;
            }
            if(HumOn) {
              MinHum = 255;
              MaxHum = 0;         // сброс минимума и максимума
            }
            if(PresOn) {
              MinPres = 65000;
              MaxPres = 0;
            }
            if(CO2On) {
              MinCO2 = 65000;
              MaxCO2 = 0;
            }
            CurMeasurement = 0;
          }
          CurTimer = millis();      // обновление таймера
        }
      }
    }
  }
}
