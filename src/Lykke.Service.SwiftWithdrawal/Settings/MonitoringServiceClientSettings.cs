using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive", false)]
        public string MonitoringServiceUrl { get; set; }
    }
}
