using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Audit.HostedServices
{
    [UsedImplicitly]
    public  class AuditHostedService : IHostedService
    {
        private readonly ILogger<AuditHostedService> _logger;
        private readonly IEventBus _eventBus;

        public AuditHostedService(ILogger<AuditHostedService> logger, IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting hosted service.");
            _eventBus.Subscribe<OrderStatusChangedIntegrationEvent, OrderStatusChangedIntegrationEventHandler>();

            _eventBus.Consume();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping hosted service.");

            return Task.CompletedTask;
        }
    }
}
