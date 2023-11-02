using System.IO;
using System.Linq;
using ProjAurora.Interface;
using ProjAurora.Launcher.Version;
using ProjAurora.Models;
using ProjAurora.Utils;
using ProjAurorax.Utils;

namespace ProjAurora.Launcher
{
    public class LaunchCore
    {
        public string GameRootPath { get; set; }
        public string JavaPath { get; set; }

        private LaunchArguments _launchArguments;

        private IVersionLocator _versionLocator;
        private IVersionLocator VersionLocator
        {
            get => _versionLocator;
            set
            {
                _versionLocator = value;
                _versionLocator.LaunchCore = this;
            }
        }

        public LaunchCore(string gameRootPath = null, string javaPath = null, IVersionLocator versionLocator = null)
        {
            GameRootPath = new DirectoryInfo(gameRootPath ?? ".minecraft").FullName;
            JavaPath = javaPath ?? JVMLocator.Search().FirstOrDefault();
            VersionLocator = versionLocator ?? new VersionLocator();
        }

        public LaunchResult LaunchGame(LaunchOption launchOption)
        {
            _launchArguments = new LaunchArguments
            {
                CoreArguments = new CoreArguments
                {
                    CGCEnabled = launchOption.CGCEnabled,
                    GCEnabled = launchOption.GCEnabled,
                    GCType = launchOption.GCType,
                    JavaAgent = launchOption.JavaAgent
                },
                Memory = launchOption.Memory,
                WindowSize = launchOption.WindowSize,
                Server = launchOption.Server,
                AdvanceArguments = launchOption.AdvanceArguments
            };

            return LaunchGameInternal();
        }

        private LaunchResult LaunchGameInternal()
        {
            var launchArguments = ArgumentsGenerator.GetLaunchArguments(_launchArguments, this);
        }
    }
}