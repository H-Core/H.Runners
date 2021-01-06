using System.Linq;
using System.Net.NetworkInformation;
using H.Core;
using H.Core.Runners;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InternetRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public InternetRunner()
        {
            Add(new SyncAction("check-internet", _ => new Value($"{IsAvailableNetworkActive()}")));
            AddVariable("$internet$", () => IsAvailableNetworkActive());
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailableNetworkActive()
        {
            return NetworkInterface.GetIsNetworkAvailable() &&
                   NetworkInterface.GetAllNetworkInterfaces()
                       .Where(@interface => @interface.OperationalStatus == OperationalStatus.Up &&
                                            @interface.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                                            @interface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                       .Select(@interface => @interface.GetIPv4Statistics())
                       .Any(statistics => statistics.BytesReceived > 0 && statistics.BytesSent > 0);
        }

        #endregion
    }
}
