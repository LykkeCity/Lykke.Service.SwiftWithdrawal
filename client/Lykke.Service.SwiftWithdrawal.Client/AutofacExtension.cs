using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.SwiftWithdrawal.Client
{
    public static class AutofacExtension
    {
        public static void RegisterSwiftWithdrawalClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<SwiftWithdrawalClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ISwiftWithdrawalClient>()
                .SingleInstance();
        }

        public static void RegisterSwiftWithdrawalClient(this ContainerBuilder builder, SwiftWithdrawalServiceClientSettings settings, ILog log)
        {
            builder.RegisterSwiftWithdrawalClient(settings?.ServiceUrl, log);
        }
    }
}
