using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orders.API.Application.IntegrationEvents;
using Orders.Infrastructure;
using Serilog.Context;

namespace Orders.API.Application.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly OrdersContext _dbContext;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public TransactionBehaviour(OrdersContext dbContext,
        IOrderingIntegrationEventService orderingIntegrationEventService,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _orderingIntegrationEventService = orderingIntegrationEventService ??
                                           throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                Guid transactionId;

                await using (var transaction =
                             await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, request);

                    response = await next();

                    _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);

                    transactionId = transaction.TransactionId;

                    await transaction.CommitAsync(cancellationToken);
                }

                await _orderingIntegrationEventService.PublishEventsAsync(transactionId);
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
