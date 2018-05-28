using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.SwiftWithdrawal.AzureRepositories;
using Lykke.Service.SwiftWithdrawal.Core.Domain;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Lykke.Service.SwiftWithdrawal.Core.Services;
using Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings;
using Lykke.Service.SwiftWithdrawal.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.SwiftWithdrawal.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<SwiftWithdrawalSettings> _settings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<SwiftWithdrawalSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterRepositories(builder);
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterInstance<IWithdrawLimitsRepository>(
                new WithdrawLimitsRepository(
                    AzureTableStorage<WithdrawLimitRecord>.Create(_settings.ConnectionString(x => x.Db.ClientPersonalInfoConnString), "WithdrawLimits", _log),
                    _settings.CurrentValue.DefaultWithdrawalLimit));

            builder.RegisterInstance<ICashoutAttemptRepository>(
                new CashoutAttemptRepository(
                    AzureTableStorage<CashoutAttemptEntity>.Create(_settings.ConnectionString(x => x.Db.BalancesInfoConnString), "CashOutAttempt", _log)));

            builder.RegisterInstance<ICashoutRequestLogRepository>(
                new CashoutRequestLogRepository(
                    AzureTableStorage<CashoutRequestLogRecord>.Create(_settings.ConnectionString(x => x.Db.BalancesInfoConnString), "CashOutAttemptLog", _log)));

            builder.RegisterInstance<ICashoutPaymentDateRepository>(
                new CashoutPaymentDateRepository(
                    AzureTableStorage<CashoutPaymentDateEntity>.Create(_settings.ConnectionString(x => x.Db.BalancesInfoConnString), "CashoutPaymentDates", _log)));
        }
    }
}
