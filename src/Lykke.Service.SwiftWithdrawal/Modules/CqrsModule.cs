using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Settings;
using Lykke.Service.SwiftWithdrawal.Workflow.Handlers;
using Lykke.Service.SwiftWithdrawal.Workflow.Projections;
using Lykke.SettingsReader;

namespace Lykke.Service.SwiftWithdrawal.Modules
{
    public class CqrsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public CqrsModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            Messaging.Serialization.MessagePackSerializerFactory.Defaults.FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;
            var rabbitMqSagasSettings = new RabbitMQ.Client.ConnectionFactory { Uri = _settings.CurrentValue.SwiftWithdrawalService.Cqrs.RabbitConnectionString };

            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>();

            var messagingEngine = new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    { "SagasRabbitMq", new TransportInfo(rabbitMqSagasSettings.Endpoint.ToString(), rabbitMqSagasSettings.UserName, rabbitMqSagasSettings.Password, "None", "RabbitMq") }
                }),
                new RabbitMqTransportFactory());

            var sagasEndpointResolver = new RabbitMqConventionEndpointResolver(
                "SagasRabbitMq",
                SerializationFormat.MessagePack,
                environment: "lykke",
                exclusiveQueuePostfix: "k8s");

            builder.RegisterType<SwiftCashoutRequestCommandHandler>().SingleInstance();
            builder.RegisterType<SwiftRequestLogProjection>().SingleInstance();



            builder.Register(ctx =>
            {
                string commandsRoute = "commands";
                string eventsRoute = "events";

                return new CqrsEngine(_log,
                    ctx.Resolve<IDependencyResolver>(),
                    messagingEngine,
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(sagasEndpointResolver),

                    Register.BoundedContext(SwiftWithdrawalBoundedContext.Name)
                        .ListeningCommands(typeof(SwiftCashoutCreateCommand))
                        .On(commandsRoute)
                        .WithCommandsHandler<SwiftCashoutRequestCommandHandler>()
                        .PublishingEvents(typeof(SwiftCashoutCreatedEvent))
                        .With(eventsRoute)

                        .ListeningEvents(typeof(SwiftCashoutCreatedEvent))
                        .From(SwiftWithdrawalBoundedContext.Name)
                        .On(eventsRoute)
                        .WithProjection(typeof(SwiftRequestLogProjection), SwiftWithdrawalBoundedContext.Name));

            })
            .As<ICqrsEngine>()
            .SingleInstance()
            .AutoActivate();
        }

        internal class AutofacDependencyResolver : IDependencyResolver
        {
            private readonly IComponentContext _context;

            public AutofacDependencyResolver([NotNull] IComponentContext kernel)
            {
                _context = kernel ?? throw new ArgumentNullException(nameof(kernel));
            }

            public object GetService(Type type)
            {
                return _context.Resolve(type);
            }

            public bool HasService(Type type)
            {
                return _context.IsRegistered(type);
            }
        }
    }
}
