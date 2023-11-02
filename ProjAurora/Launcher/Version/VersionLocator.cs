using System.Collections.Generic;
using ProjAurora.Interface;
using ProjAurora.Models;

namespace ProjAurora.Launcher.Version
{
    public class VersionLocator : IVersionLocator
    {
        public LaunchCore LaunchCore { get; set; }

        public List<Models.Version> GetAllVersion()
        {
            throw new System.NotImplementedException();
        }

        public Models.Version GetVersion()
        {
            throw new System.NotImplementedException();
        }
    }
}