using ProjAurora.Class.Utils;

namespace ProjAurora.Launcher
{
    public class LaunchHandler
    {
        public event GameLogHandler GameLog;
        public event GameExitHandler GameExit;
        public event LaunchLogHandler LaunchLog;
        public delegate void GameLogHandler(object sender, string e);
        public delegate void GameExitHandler(object sender, GameExitArg arg);
        public delegate void LaunchLogHandler(object sender, Log log);

        public LaunchHandler()
        {

        }
    }
}