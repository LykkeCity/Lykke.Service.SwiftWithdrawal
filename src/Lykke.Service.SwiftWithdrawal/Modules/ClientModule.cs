using Autofac;
using Common.Log;
using Lykke.Service.PersonalData;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.SwiftWithdrawal.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.SwiftWithdrawal.Modules
{
    public class ClientModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public ClientModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IPersonalDataService>(new PersonalDataService(_settings.CurrentValue.PersonalDataServiceClient, _log));
        }
    }
}
