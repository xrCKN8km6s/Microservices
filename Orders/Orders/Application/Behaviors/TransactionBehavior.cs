using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Application.IntegrationEvents;
using Orders.Infrastructure;
using Serilog.Context;

namespace Orders.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly MicroserviceContext _dbContext;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public TransactionBehaviour(MicroserviceContext dbContext,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(MicroserviceContext));
            _orderingIntegrationEventService = orderingIntegrationEventService ??
                                               throw new ArgumentException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.Database.CurrentTransaction != null)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction =
                        await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
                    )
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, typeName);

                        transaction.Commit();
                    }

                    await _orderingIntegrationEventService.PublishEventsAsync();
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling transaction for {CommandName} ({@Command})", typeName,
                    request);

                throw;
            }
        }
    }
}