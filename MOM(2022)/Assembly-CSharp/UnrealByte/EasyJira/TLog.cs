using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace UnrealByte.EasyJira
{
    public class TLog
    {
        private static TLog instance;

        public string logFilePath = "";

        private bool logAlive;

        private int errorIndex;

        public static string firstCritical = "";

        public static string firstCriticalHeader = "";

        public bool showFirstLog;

        public bool shownFirstLog;

        public bool usedDevMenu;

        public static TLog Get()
        {
            if (TLog.instance == null)
            {
                TLog.instance = new TLog();
                TLog.instance.initializeLog();
            }
            else if (!TLog.instance.logAlive)
            {
                global::UnityEngine.Debug.LogError("TLog is in zombie state (Deinitialized by refferenced)");
            }
            return TLog.instance;
        }

        public static void ClearFirstError()
        {
            TLog.Get().showFirstLog = false;
            TLog.Get().shownFirstLog = false;
            TLog.firstCritical = "";
            TLog.firstCriticalHeader = "";
        }

        public void initializeLog()
        {
            this.logFilePath = "";
            this.logFilePath = Application.persistentDataPath + "/GameLog.txt";
            string text = "Ver Undefined";
            this.logAlive = true;
            StreamWriter streamWriter = File.CreateText(this.logFilePath);
            streamWriter.WriteLine("=================== Begin Log ====================");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "]");
            streamWriter.WriteLine("Unity Version     : " + Application.unityVersion);
            streamWriter.WriteLine("Game Version      : " + text);
            streamWriter.WriteLine("CPU Type          : " + SystemInfo.processorType);
            streamWriter.WriteLine("CPU Count         : " + SystemInfo.processorCount);
            streamWriter.WriteLine("CPU Speed         : " + SystemInfo.processorFrequency + " MHz");
            streamWriter.WriteLine("RAM               : " + SystemInfo.systemMemorySize + " MB");
            streamWriter.WriteLine("VRAM              : " + SystemInfo.graphicsMemorySize + " MB");
            streamWriter.WriteLine("GPU VENDOR        : " + SystemInfo.graphicsDeviceVendor);
            streamWriter.WriteLine("GPU               : " + SystemInfo.graphicsDeviceName);
            streamWriter.WriteLine("Renderer Type     : " + SystemInfo.graphicsDeviceType);
            streamWriter.WriteLine("OS                : " + SystemInfo.operatingSystem);
            streamWriter.WriteLine("==================================================");
            streamWriter.WriteLine("");
            streamWriter.Close();
            Application.logMessageReceivedThreaded += HandleLog;
        }

        public void DeInitialize()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
            this.logAlive = false;
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error && condition.Contains("<RI.Hid>"))
            {
                return;
            }
            bool flag = false;
            lock (this)
            {
                StreamWriter streamWriter = new StreamWriter(this.logFilePath, append: true);
                bool flag2 = false;
                if (type == LogType.Assert || type == LogType.Exception || type == LogType.Error)
                {
                    if (!flag)
                    {
                        flag = true;
                    }
                    this.errorIndex++;
                    flag2 = true;
                    type = LogType.Error;
                }
                streamWriter.WriteLine();
                streamWriter.WriteLine("[" + type.ToString() + "]" + DateTime.Now.ToString());
                streamWriter.WriteLine(condition);
                if (flag2)
                {
                    StackTrace stackTrace2 = new StackTrace();
                    streamWriter.WriteLine("CRITICAL POINT: " + this.errorIndex);
                    if (stackTrace == null || stackTrace.Length < 5)
                    {
                        stackTrace = stackTrace2.ToString();
                    }
                    streamWriter.WriteLine(stackTrace);
                    if (flag)
                    {
                        this.showFirstLog = true;
                        TLog.firstCriticalHeader = condition;
                        TLog.firstCritical = condition + Environment.NewLine + stackTrace + Environment.NewLine + stackTrace2.ToString();
                    }
                }
                streamWriter.Close();
            }
        }
    }
}
