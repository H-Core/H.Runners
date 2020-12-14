using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace H.Runners
{
    /// <summary>
    /// Keeps track of the X most recent number of events recorded by the listener. X is specified in the constructor
    /// </summary>
    public class Top10Listener : TraceListener
    {
        private int capacity;
        private LinkedList<string> traces;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public Top10Listener(int capacity)
        {
            this.capacity = capacity;
            this.traces = new LinkedList<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            lock (traces)
                traces.Last.Value += message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            lock (traces)
            {
                if (traces.Count >= capacity)
                    traces.RemoveFirst();

                traces.AddLast(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void ExportTo(Action<string> action)
        {
            lock (traces)
                foreach (string s in this.traces)
                    action.Invoke(s);
        }
    }
}
