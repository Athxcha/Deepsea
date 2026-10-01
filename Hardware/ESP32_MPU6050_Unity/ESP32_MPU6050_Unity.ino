#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>

Adafruit_MPU6050 mpu;

const int sdaPin = 21;
const int sclPin = 22;
const int sampleDelayMs = 10;
const float gyroDeadZoneDps = 1.5f;
const float tiltDeadZoneDegrees = 3.0f;

void setup() {
  Serial.begin(115200);
  delay(1000);

  Wire.begin(sdaPin, sclPin);

  if (!mpu.begin()) {
    Serial.println("ERR:MPU6050_NOT_FOUND");
    while (true) {
      delay(1000);
    }
  }

  mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
  mpu.setGyroRange(MPU6050_RANGE_500_DEG);
  mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);
}

void loop() {
  sensors_event_t acceleration;
  sensors_event_t gyro;
  sensors_event_t temperature;

  mpu.getEvent(&acceleration, &gyro, &temperature);

  float pitchRateXDps = gyro.gyro.x * 180.0f / PI;
  float pitchRateYDps = gyro.gyro.y * 180.0f / PI;
  float yawRateDps = gyro.gyro.z * 180.0f / PI;
  float rollDegrees = atan2(acceleration.acceleration.y, acceleration.acceleration.z) * 180.0f / PI;
  float pitchDegrees = atan2(
    -acceleration.acceleration.x,
    sqrt(
      acceleration.acceleration.y * acceleration.acceleration.y +
      acceleration.acceleration.z * acceleration.acceleration.z
    )
  ) * 180.0f / PI;

  if (abs(yawRateDps) < gyroDeadZoneDps) {
    yawRateDps = 0.0f;
  }

  if (abs(pitchRateXDps) < gyroDeadZoneDps) {
    pitchRateXDps = 0.0f;
  }

  if (abs(pitchRateYDps) < gyroDeadZoneDps) {
    pitchRateYDps = 0.0f;
  }

  if (abs(rollDegrees) < tiltDeadZoneDegrees) {
    rollDegrees = 0.0f;
  }

  if (abs(pitchDegrees) < tiltDeadZoneDegrees) {
    pitchDegrees = 0.0f;
  }

  Serial.print("X:");
  Serial.print(pitchRateXDps, 3);
  Serial.print(",V:");
  Serial.print(pitchRateYDps, 3);
  Serial.print(",Y:");
  Serial.print(yawRateDps, 3);
  Serial.print(",R:");
  Serial.print(rollDegrees, 3);
  Serial.print(",P:");
  Serial.println(pitchDegrees, 3);

  delay(sampleDelayMs);
}
