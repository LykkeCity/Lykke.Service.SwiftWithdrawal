using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string BalancesInfoConnString { get; set; }

        [AzureTableCheck]
        public string ClientPersonalInfoConnString { get; set; }
    }
}
