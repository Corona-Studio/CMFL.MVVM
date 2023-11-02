using System.Collections.Generic;

namespace ProjAurora.Models
{
    public class LaunchOption
    {
        public string JavaAgent { get; set; }
        public bool CGCEnabled { get; set; }
        public bool GCEnabled { get; set; }
        public GCType GCType { get; set; }
        public Server Server { get; set; }
        public Memory Memory { get; set; }
        public WindowSize WindowSize { get; set; }
        public List<string> AdvanceArguments { get; set; }
    }
}