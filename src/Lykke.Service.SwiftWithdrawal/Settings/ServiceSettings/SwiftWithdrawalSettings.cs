using JetBrains.Annotations;

namespace Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SwiftWithdrawalSettings
    {
        public DbSettings Db { get; set; }

        public double DefaultWithdrawalLimit { get; set; }
    }
}
