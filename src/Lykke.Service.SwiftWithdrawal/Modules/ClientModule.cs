using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Log;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.SwiftWithdrawal.Core.Services;
using Lykke.Service.SwiftWithdrawal.Services;
using Lykke.Service.SwiftWithdrawal.Settings;
using Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings;
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
            builder.RegisterPersonalDataServiceClient(_settings.CurrentValue.PersonalDataServiceClient, _log);
        }
    }
}
