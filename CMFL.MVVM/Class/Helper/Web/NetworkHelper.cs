using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CMFL.MVVM.Class.Helper.Web
{
    public static class NetworkHelper
    {
        /// <summary>
        ///     系统代理检测
        /// </summary>
        /// <returns>检测结果</returns>
        public static bool SystemProxyCheck()
        {
            var flag = false;
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                    if (NetworkInterface.GetAllNetworkInterfaces()
                        .Where(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up).Any(
                            networkInterface => networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ppp &&
                                                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        return true;

                if (WebProxy.GetDefaultProxy().Address != null)
                    flag = true;
            }
            catch (NetworkInformationException)
            {
                flag = false;
            }

            return flag;
        }

        public static Task<string> GetFastestAddress(IEnumerable<string> list)
        {
            return Task.Run(() =>
            {
                using var ping = new Ping();
                var result = new Dictionary<long, string>();
                foreach (var address in list)
                    try
                    {
                        var pingReply = ping.Send(new Uri(address).Host);

                        if (pingReply == null) continue;

                        if (result.ContainsKey(pingReply.RoundtripTime))
                            result.Add(pingReply.RoundtripTime + 1, address);
                        else
                            result.Add(pingReply.RoundtripTime, address);
                    }
                    catch (PingException)
                    {
                    }

                return result.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value).FirstOrDefault().Value;
            });
        }
    }
}