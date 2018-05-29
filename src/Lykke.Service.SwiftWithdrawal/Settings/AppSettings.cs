using JetBrains.Annotations;
using Lykke.Service.PersonalData.Settings;
using Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings;
using Lykke.Service.SwiftWithdrawal.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public SwiftWithdrawalSettings SwiftWithdrawalService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }

        public SagasRabbitMq SagasRabbitMq { get; set; }

        public PersonalDataServiceClientSettings PersonalDataServiceClient { get; set; }
    }
    
    public class SagasRabbitMq
    {
        [AmqpCheck]
        public string RabbitConnectionString { get; set; }
    }
}
