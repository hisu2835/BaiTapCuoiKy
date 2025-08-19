using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BaiTapCuoiKy
{
    // Minimal NetworkManager to satisfy Form1 dependencies.
    // Provides a local connection address and no-op Initialize/Cleanup.
    public class NetworkManager
    {
        public event Action<string> LogMessage;
        public event Action<string> ErrorOccurred;

        private string _connectionAddress = "127.0.0.1";

        public async Task Initialize()
        {
            try
            {
                var ip = GetLocalIPv4();
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    _connectionAddress = ip;
                }
                LogMessage?.Invoke($"Network initialized. Connection address: {_connectionAddress}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Network init error: {ex.Message}");
            }
        }

        public string GetConnectionAddress()
        {
            return _connectionAddress;
        }

        public async Task Cleanup()
        {
            try
            {
                // No-op for now
                LogMessage?.Invoke("Network cleanup complete.");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Network cleanup error: {ex.Message}");
            }
        }

        private static string GetLocalIPv4()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch
            {
                // ignore
            }
            return null;
        }
    }
}
