using System.Collections.Generic;
using ProjAurora.Launcher;
using ProjAurora.Models;

namespace ProjAurora.Interface
{
    public interface IVersionLocator
    {
        LaunchCore LaunchCore { get; set; }
        List<Version> GetAllVersion();
        Version GetVersion();
    }
}