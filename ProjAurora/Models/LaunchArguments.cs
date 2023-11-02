using System.Collections.Generic;
using System.Text;

namespace ProjAurora.Models
{
    public class CoreArguments
    {
        public string MainClass { get; set; }
        public string NativesPath { get; set; }
        public bool CGCEnabled { get; set; }
        public bool GCEnabled { get; set; }
        public GCType GCType { get; set; }
        public List<string> Libraries { get; set; }
        public string JavaAgent { get; set; }
    }

    public class WindowSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FullScreen { get; set; }
    }

    public class Memory
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class Server
    {
        public string Ip { get; set; }
    }

    public class LaunchArguments
    {
        public CoreArguments CoreArguments { get; set; }
        public List<string> AdvanceArguments { get; set; }
        public Version Version { get; set; }
        public Server Server { get; set; }
        public Memory Memory { get; set; }
        public WindowSize WindowSize { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            return "";
        }
    }
}