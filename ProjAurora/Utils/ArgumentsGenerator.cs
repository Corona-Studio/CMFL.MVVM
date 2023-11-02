using ProjAurora.Models;
using System.Text;
using ProjAurora.Launcher;

namespace ProjAurora.Utils
{
    public class ArgumentsGenerator
    {
        public static string GetLaunchArguments(LaunchArguments launchArguments, LaunchCore launchCore)
        {
            var sb = new StringBuilder();

            #region JVM参数

            if (!string.IsNullOrEmpty(launchArguments.CoreArguments.JavaAgent))
            {
                sb.AppendWithWhitespace($"-javaagent:{launchArguments.CoreArguments.JavaAgent.Trim()}");
            }

            if (launchArguments.CoreArguments.GCEnabled)
            {
                switch (launchArguments.CoreArguments.GCType)
                {
                    default:
                        break;
                    case GCType.G1GC:
                        sb.AppendWithWhitespace("-XX:+UseG1GC");
                        break;
                    case GCType.SerialGC:
                        sb.AppendWithWhitespace("-XX:+UseSerialGC");
                        break;
                    case GCType.ParallelGC:
                        sb.AppendWithWhitespace("-XX:+UseParallelGC");
                        break;
                    case GCType.CMSGC:
                        sb.AppendWithWhitespace("-XX:+UseConcMarkSweepGC");
                        break;
                }

                if (launchArguments.CoreArguments.CGCEnabled)
                {
                    sb.AppendWithWhitespace("-Xincgc");
                }

                if (launchArguments.Memory.Min > 0)
                {
                    sb.AppendWithWhitespace($"-Xms {launchArguments.Memory.Min}M");
                }

                sb.AppendWithWhitespace(launchArguments.Memory.Max > 0
                    ? $"-Xmx {launchArguments.Memory.Max}M"
                    : "-Xmx 1024M");

                launchArguments.AdvanceArguments.ForEach(arg =>
                {
                    sb.AppendWithWhitespace(arg);
                });
            }
            #endregion

            #region 游戏参数

            

            #endregion

            return sb.ToString();
        }
    }
}