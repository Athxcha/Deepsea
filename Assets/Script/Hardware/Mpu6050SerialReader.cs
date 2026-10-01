using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace DeepScan
{
    public class Mpu6050SerialReader : MonoBehaviour
    {
        [Header("Serial")]
        [SerializeField]
        private string portName = "COM3";

        [SerializeField]
        private int baudRate = 115200;

        [SerializeField]
        private bool openOnStart = true;

        [SerializeField]
        private int readTimeoutMs = 50;

        [Header("Yaw")]
        [SerializeField]
        private float deadZoneDegreesPerSecond = 1.5f;

        [SerializeField]
        private float signalTimeoutSeconds = 0.25f;

        [Header("Tilt Turning")]
        [SerializeField]
        private bool useRollTiltToTurn = true;

        [SerializeField]
        private float tiltDeadZoneDegrees = 5f;

        [SerializeField]
        private float maxTiltDegrees = 45f;

        [SerializeField]
        private float maxTiltTurnSpeedDegreesPerSecond = 160f;

        [Header("Tilt Look")]
        [SerializeField]
        private bool useGyroPitchToLook = true;

        [SerializeField]
        private bool useGyroYForPitchLook = true;

        [SerializeField]
        private float pitchRateDeadZoneDegreesPerSecond = 1.5f;

        [SerializeField]
        private bool usePitchTiltToLook = true;

        [SerializeField]
        private float pitchDeadZoneDegrees = 5f;

        [SerializeField]
        private float maxPitchDegrees = 45f;

        [SerializeField]
        private float maxPitchLookSpeedDegreesPerSecond = 120f;

        [Header("Debug")]
        [SerializeField]
        private bool showDebugOverlay = true;

        [SerializeField]
        private bool logStatus = true;

        [SerializeField]
        private float logIntervalSeconds = 1f;

        private readonly object sampleLock = new object();

        private object serialPort;
        private MethodInfo serialOpenMethod;
        private MethodInfo serialCloseMethod;
        private MethodInfo serialDisposeMethod;
        private MethodInfo serialDiscardInBufferMethod;
        private MethodInfo serialReadLineMethod;
        private PropertyInfo serialIsOpenProperty;
        private Thread readThread;
        private volatile bool keepReading;

        private float yawRateDegreesPerSecond;
        private float pitchRateXDegreesPerSecond;
        private float pitchRateYDegreesPerSecond;
        private float rollDegrees;
        private float pitchDegrees;
        private DateTime lastSampleUtc = DateTime.MinValue;
        private string lastLine = string.Empty;
        private string lastError = string.Empty;
        private float nextLogTime;

        public bool IsOpen => serialPort != null && serialIsOpenProperty != null && (bool)serialIsOpenProperty.GetValue(serialPort);
        public string LastLine => lastLine;
        public string LastError => lastError;
        public float CurrentYawRateDegreesPerSecond => yawRateDegreesPerSecond;
        public float CurrentPitchRateDegreesPerSecond => useGyroYForPitchLook ? pitchRateYDegreesPerSecond : pitchRateXDegreesPerSecond;
        public float CurrentPitchRateXDegreesPerSecond => pitchRateXDegreesPerSecond;
        public float CurrentPitchRateYDegreesPerSecond => pitchRateYDegreesPerSecond;
        public float CurrentRollDegrees => rollDegrees;
        public float CurrentPitchDegrees => pitchDegrees;
        public bool HasRecentSignal => SecondsSinceLastSample() <= signalTimeoutSeconds;

        private void Start()
        {
            if (openOnStart)
            {
                Open();
            }
        }

        private void OnDisable()
        {
            Close();
        }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            try
            {
                Type serialPortType = FindSerialPortType();

                if (serialPortType == null)
                {
                    throw new InvalidOperationException(
                        "System.IO.Ports.SerialPort was not found. Set Player > Other Settings > Api Compatibility Level to .NET Framework."
                    );
                }

                serialPort = Activator.CreateInstance(serialPortType, portName, baudRate);
                serialOpenMethod = serialPortType.GetMethod("Open", Type.EmptyTypes);
                serialCloseMethod = serialPortType.GetMethod("Close", Type.EmptyTypes);
                serialDisposeMethod = serialPortType.GetMethod("Dispose", Type.EmptyTypes);
                serialDiscardInBufferMethod = serialPortType.GetMethod("DiscardInBuffer", Type.EmptyTypes);
                serialReadLineMethod = serialPortType.GetMethod("ReadLine", Type.EmptyTypes);
                serialIsOpenProperty = serialPortType.GetProperty("IsOpen");

                serialPortType.GetProperty("NewLine")?.SetValue(serialPort, "\n");
                serialPortType.GetProperty("ReadTimeout")?.SetValue(serialPort, Mathf.Max(1, readTimeoutMs));
                serialPortType.GetProperty("DtrEnable")?.SetValue(serialPort, true);

                serialOpenMethod.Invoke(serialPort, null);
                serialDiscardInBufferMethod?.Invoke(serialPort, null);

                keepReading = true;
                readThread = new Thread(ReadLoop)
                {
                    IsBackground = true
                };
                readThread.Start();
                lastError = string.Empty;
                Debug.Log($"MPU6050 serial opened on {portName} at {baudRate} baud.");
            }
            catch (Exception exception)
            {
                lastError = exception.Message;
                Debug.LogWarning($"MPU6050 serial open failed on {portName}: {exception.Message}");
                Close();
            }
        }

        public void Close()
        {
            keepReading = false;

            if (readThread != null && readThread.IsAlive)
            {
                readThread.Join(100);
            }

            readThread = null;

            if (serialPort != null)
            {
                try
                {
                    if (IsOpen)
                    {
                        serialCloseMethod?.Invoke(serialPort, null);
                    }
                }
                catch (Exception exception)
                {
                    lastError = exception.Message;
                }

                serialDisposeMethod?.Invoke(serialPort, null);
                serialPort = null;
            }
        }

        private void Update()
        {
            if (!logStatus || Time.unscaledTime < nextLogTime)
            {
                return;
            }

            nextLogTime = Time.unscaledTime + Mathf.Max(0.1f, logIntervalSeconds);

            Debug.Log(
                $"MPU6050 status | open={IsOpen} | recent={HasRecentSignal} | " +
                $"yawRate={CurrentYawRateDegreesPerSecond:F2} | pitchX={CurrentPitchRateXDegreesPerSecond:F2} | " +
                $"pitchY={CurrentPitchRateYDegreesPerSecond:F2} | activePitch={CurrentPitchRateDegreesPerSecond:F2} | " +
                $"roll={CurrentRollDegrees:F2} | " +
                $"pitch={CurrentPitchDegrees:F2} | " +
                $"last='{LastLine}' | error='{LastError}'"
            );
        }

        private void OnGUI()
        {
            if (!showDebugOverlay)
            {
                return;
            }

            GUI.Label(
                new Rect(12f, 12f, 640f, 96f),
                $"MPU6050 {portName} open:{IsOpen} recent:{HasRecentSignal}\n" +
                $"yaw:{CurrentYawRateDegreesPerSecond:F2} dps pitchX:{CurrentPitchRateXDegreesPerSecond:F2} pitchY:{CurrentPitchRateYDegreesPerSecond:F2}\n" +
                $"activePitch:{CurrentPitchRateDegreesPerSecond:F2} dps\n" +
                $"roll:{CurrentRollDegrees:F2} deg pitch:{CurrentPitchDegrees:F2} deg\n" +
                $"last:{LastLine}\n" +
                $"error:{LastError}"
            );
        }

        public float GetYawDeltaDegrees(float deltaTime)
        {
            float yawRate;
            DateTime sampleTime;

            lock (sampleLock)
            {
                yawRate = yawRateDegreesPerSecond;
                sampleTime = lastSampleUtc;
            }

            if (sampleTime == DateTime.MinValue)
            {
                return 0f;
            }

            if ((DateTime.UtcNow - sampleTime).TotalSeconds > signalTimeoutSeconds)
            {
                return 0f;
            }

            if (Mathf.Abs(yawRate) < deadZoneDegreesPerSecond)
            {
                return 0f;
            }

            return yawRate * deltaTime;
        }

        public float GetTurnDeltaDegrees(float deltaTime)
        {
            if (!useRollTiltToTurn)
            {
                return GetYawDeltaDegrees(deltaTime);
            }

            float roll;
            DateTime sampleTime;

            lock (sampleLock)
            {
                roll = rollDegrees;
                sampleTime = lastSampleUtc;
            }

            if (sampleTime == DateTime.MinValue)
            {
                return 0f;
            }

            if ((DateTime.UtcNow - sampleTime).TotalSeconds > signalTimeoutSeconds)
            {
                return 0f;
            }

            float absRoll = Mathf.Abs(roll);

            if (absRoll < tiltDeadZoneDegrees)
            {
                return 0f;
            }

            float usableTiltRange = Mathf.Max(1f, maxTiltDegrees - tiltDeadZoneDegrees);
            float normalizedTilt = Mathf.Clamp01((absRoll - tiltDeadZoneDegrees) / usableTiltRange);
            float turnRate = normalizedTilt * maxTiltTurnSpeedDegreesPerSecond * Mathf.Sign(roll);

            return turnRate * deltaTime;
        }

        public float GetLookDeltaDegrees(float deltaTime)
        {
            if (useGyroPitchToLook)
            {
                float pitchRate;
                DateTime gyroSampleTime;

                lock (sampleLock)
                {
                    pitchRate = useGyroYForPitchLook ? pitchRateYDegreesPerSecond : pitchRateXDegreesPerSecond;
                    gyroSampleTime = lastSampleUtc;
                }

                if (gyroSampleTime == DateTime.MinValue)
                {
                    return 0f;
                }

                if ((DateTime.UtcNow - gyroSampleTime).TotalSeconds > signalTimeoutSeconds)
                {
                    return 0f;
                }

                if (Mathf.Abs(pitchRate) < pitchRateDeadZoneDegreesPerSecond)
                {
                    return 0f;
                }

                return pitchRate * deltaTime;
            }

            if (!usePitchTiltToLook)
            {
                return 0f;
            }

            float pitch;
            DateTime sampleTime;

            lock (sampleLock)
            {
                pitch = pitchDegrees;
                sampleTime = lastSampleUtc;
            }

            if (sampleTime == DateTime.MinValue)
            {
                return 0f;
            }

            if ((DateTime.UtcNow - sampleTime).TotalSeconds > signalTimeoutSeconds)
            {
                return 0f;
            }

            float absPitch = Mathf.Abs(pitch);

            if (absPitch < pitchDeadZoneDegrees)
            {
                return 0f;
            }

            float usablePitchRange = Mathf.Max(1f, maxPitchDegrees - pitchDeadZoneDegrees);
            float normalizedPitch = Mathf.Clamp01((absPitch - pitchDeadZoneDegrees) / usablePitchRange);
            float lookRate = normalizedPitch * maxPitchLookSpeedDegreesPerSecond * Mathf.Sign(pitch);

            return lookRate * deltaTime;
        }

        private double SecondsSinceLastSample()
        {
            DateTime sampleTime;

            lock (sampleLock)
            {
                sampleTime = lastSampleUtc;
            }

            if (sampleTime == DateTime.MinValue)
            {
                return double.PositiveInfinity;
            }

            return (DateTime.UtcNow - sampleTime).TotalSeconds;
        }

        private void ReadLoop()
        {
            while (keepReading)
            {
                try
                {
                    string line = ((string)serialReadLineMethod.Invoke(serialPort, null)).Trim();

                    if (TryParseSample(line, out float pitchRateX, out float pitchRateY, out float yawRate, out float roll, out float pitch))
                    {
                        lock (sampleLock)
                        {
                            yawRateDegreesPerSecond = yawRate;
                            pitchRateXDegreesPerSecond = pitchRateX;
                            pitchRateYDegreesPerSecond = pitchRateY;
                            rollDegrees = roll;
                            pitchDegrees = pitch;
                            lastSampleUtc = DateTime.UtcNow;
                            lastLine = line;
                        }
                    }
                }
                catch (TimeoutException)
                {
                    // Expected when the ESP32 pauses between packets.
                }
                catch (TargetInvocationException exception) when (exception.InnerException is TimeoutException)
                {
                    // Expected when the ESP32 pauses between packets.
                }
                catch (Exception exception)
                {
                    lastError = exception is TargetInvocationException && exception.InnerException != null
                        ? exception.InnerException.Message
                        : exception.Message;
                    Thread.Sleep(100);
                }
            }
        }

        private static Type FindSerialPortType()
        {
            Type serialPortType = Type.GetType("System.IO.Ports.SerialPort, System.IO.Ports");

            if (serialPortType != null)
            {
                return serialPortType;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                serialPortType = assembly.GetType("System.IO.Ports.SerialPort");

                if (serialPortType != null)
                {
                    return serialPortType;
                }
            }

            return null;
        }

        private static bool TryParseSample(string line, out float pitchRateX, out float pitchRateY, out float yawRate, out float roll, out float pitch)
        {
            pitchRateX = 0f;
            pitchRateY = 0f;
            yawRate = 0f;
            roll = 0f;
            pitch = 0f;

            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] parts = line.Split(',');

            foreach (string part in parts)
            {
                string token = part.Trim();

                if (token.StartsWith("Y:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("Yaw:", StringComparison.OrdinalIgnoreCase))
                {
                    int separator = token.IndexOf(':');
                    float.TryParse(
                        token.Substring(separator + 1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out yawRate
                    );
                }

                if (token.StartsWith("X:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("PitchRate:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("PitchRateX:", StringComparison.OrdinalIgnoreCase))
                {
                    int separator = token.IndexOf(':');
                    float.TryParse(
                        token.Substring(separator + 1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out pitchRateX
                    );
                }

                if (token.StartsWith("V:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("PitchRateY:", StringComparison.OrdinalIgnoreCase))
                {
                    int separator = token.IndexOf(':');
                    float.TryParse(
                        token.Substring(separator + 1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out pitchRateY
                    );
                }

                if (token.StartsWith("R:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("Roll:", StringComparison.OrdinalIgnoreCase))
                {
                    int separator = token.IndexOf(':');
                    float.TryParse(
                        token.Substring(separator + 1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out roll
                    );
                }

                if (token.StartsWith("P:", StringComparison.OrdinalIgnoreCase) ||
                    token.StartsWith("Pitch:", StringComparison.OrdinalIgnoreCase))
                {
                    int separator = token.IndexOf(':');
                    float.TryParse(
                        token.Substring(separator + 1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out pitch
                    );
                }
            }

            if (parts.Length == 1 && !parts[0].Contains(":"))
            {
                return float.TryParse(
                    parts[0].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out yawRate
                );
            }

            return true;
        }
    }
}
