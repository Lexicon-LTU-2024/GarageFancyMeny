using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Exercise5
{
    public class EventLog
    {
        private Queue<string> log;
        private int maxSize;

        public EventLog(int maxSize)
        {
            log = new Queue<string>();
            this.maxSize = maxSize;
        }

        /// <summary>
        /// Clears the log.
        /// </summary>
        public void Clear()
        {
            log.Clear();
        }

        /// <summary>
        /// Adds a message to the log. Deletes the oldest message if log already has max capacity.
        /// </summary>
        /// <param name="message">A message worth logging</param>
        public void Add(string message)
        {
            if(log.Count == maxSize)
            {
                log.Dequeue();
            }
            log.Enqueue($"{DateTime.Now.ToString("HH:mm:ss")} {message}");
        }

        /// <summary>
        /// Gets log entries.
        /// </summary>
        /// <returns>Returns a string array with one string for each log entry</returns>
        public string[] GetLogEntries()
        {
            var x = log.ToArray();
            List<string> result = new List<string>();
            foreach(var s in log)
            {
                result.Add(s);
            }
            return result.ToArray();
        }
    }
}
