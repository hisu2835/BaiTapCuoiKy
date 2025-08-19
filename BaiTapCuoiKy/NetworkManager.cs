using System;
using System.Threading.Tasks;
using System.Net;
using Open.Nat;
using System.Collections.Generic;
using System.Linq;

namespace BaiTapCuoiKy
{
    public class NetworkManager
    {
        private NatDevice _device;
        private Mapping _mapping;
        private const int DEFAULT_PORT = 7777;
        
        public event Action<string> LogMessage;
        public event Action<string> ErrorOccurred;

        public string PublicIP { get; private set; }
        public int MappedPort { get; private set; }
        private bool _isInitialized = false;

        public async Task Initialize()
        {
            try
            {
                if (_isInitialized) return;

                var discoverer = new NatDiscoverer();
                _device = await discoverer.DiscoverDeviceAsync();
                
                // Get public IP
                PublicIP = (await _device.GetExternalIPAsync()).ToString();
                LogMessage?.Invoke($"Public IP: {PublicIP}");

                // Create port mapping
                _mapping = new Mapping(Protocol.Tcp, DEFAULT_PORT, DEFAULT_PORT, "DrawMaster Game");
                await _device.CreatePortMapAsync(_mapping);
                MappedPort = DEFAULT_PORT;

                _isInitialized = true;
                LogMessage?.Invoke("UPnP initialized successfully");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"UPnP initialization failed: {ex.Message}");
                // Fall back to local IP
                PublicIP = GetLocalIPAddress();
                MappedPort = DEFAULT_PORT;
            }
        }

        public string GetConnectionAddress()
        {
            return PublicIP;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        public async Task Cleanup()
        {
            if (_device != null && _mapping != null)
            {
                try
                {
                    await _device.DeletePortMapAsync(_mapping);
                    LogMessage?.Invoke("Port mapping removed");
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke($"Error removing port mapping: {ex.Message}");
                }
            }
        }
    }
}