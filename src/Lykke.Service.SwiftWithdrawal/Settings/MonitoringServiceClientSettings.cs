using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string MonitoringServiceUrl { get; set; }
    }
}
