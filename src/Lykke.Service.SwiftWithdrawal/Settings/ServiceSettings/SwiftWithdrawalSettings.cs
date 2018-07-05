using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SwiftWithdrawalSettings
    {
        public DbSettings Db { get; set; }

        public double DefaultWithdrawalLimit { get; set; }

        public SagasRabbitMq Cqrs { get; set; }
    }


    public class SagasRabbitMq
    {
        [AmqpCheck]
        public string RabbitConnectionString { get; set; }
    }
}
