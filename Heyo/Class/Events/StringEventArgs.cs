using System;

namespace Heyo.Class.Events
{
    public class StringEventArgs : EventArgs
    {
        public StringEventArgs(string source)
        {
            Source = source;
        }

        public string Source { get; set; }
    }
}