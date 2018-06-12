﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PlexServiceCommon;

namespace PlexServiceWCF
{
    /// <summary>
    /// Static class for writing to the log file
    /// </summary>
    static class LogWriter
    {
        private static string _logFile = Path.Combine(TrayInteraction.APP_DATA_PATH, "plexServiceLog.txt");

        private static readonly object _syncObject = new object();

        internal static void WriteLine(string detail)
        {
            lock (_syncObject)
            {
                if (!Directory.Exists(Path.GetDirectoryName(_logFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_logFile));
                }

                //reduce its size if its getting big
                if (getLineCount() > 200)
                {
                    //halve the log file
                    removeFirstLines(100);
                }

                // Create a writer and open the file:
                using (StreamWriter log = new StreamWriter(_logFile, true))
                {
                    log.WriteLine(DateTime.Now.ToString() + ": " + detail);
                }
            }
        }

        private static void removeFirstLines(int lineCount = 1)
        {
            if (File.Exists(_logFile))
            {
                string[] lines = File.ReadAllLines(_logFile);
                using (StreamWriter log = new StreamWriter(_logFile))
                {
                    for (int count = lineCount; count < lines.Length; count++)
                    {
                        log.WriteLine(lines[count]);
                    }
                }
            }
        }

        internal static void DeleteLog()
        {
            if(File.Exists(_logFile))
            {
                File.Delete(_logFile);
            }
        }

        private static int getLineCount()
        {
            int count = -1;
            if (File.Exists(_logFile))
            {
                count = File.ReadLines(_logFile).Count();
            }
            return count;
        }

        internal static string Read()
        {
            string log = string.Empty;
            if (File.Exists(_logFile))
            {
                try
                {
                    lock (_syncObject)
                    {
                        log = File.ReadAllText(_logFile);
                    }
                }
                catch { }
            }
            return log;
        }
    }
}
