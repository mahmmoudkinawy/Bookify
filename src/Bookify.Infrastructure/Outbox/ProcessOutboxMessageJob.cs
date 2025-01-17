﻿using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessageJob(
	ISqlConnectionFactory sqlConnectionFactory,
	IPublisher publisher,
	IDateTimeProvider dateTimeProvider,
	ILogger<ProcessOutboxMessageJob> logger,
	IOptions<OutboxOptions> outboxOptions
) : IJob
{
	private static readonly JsonSerializerSettings _jsonSerializerSettings = new() { TypeNameHandling = TypeNameHandling.All };

	private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("Begining to process outbox messages");

		using var connection = sqlConnectionFactory.CreateConnection();
		using var transaction = connection.BeginTransaction();

		var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

		foreach (var outboxMessage in outboxMessages)
		{
			Exception? exception = null;

			try
			{
				var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, _jsonSerializerSettings);

				await publisher.Publish(domainEvent, context.CancellationToken);
			}
			catch (Exception caughtException)
			{
				logger.LogError(caughtException, "Exception while processing outbox message {MessageId}", outboxMessage.Id);

				exception = caughtException;
			}

			await UpdateOutboxMessagesAsync(connection, transaction, outboxMessage, exception);
		}

		transaction.Commit();

		logger.LogInformation("Completed processing outbox messages");
	}

	private async Task UpdateOutboxMessagesAsync(
		IDbConnection connection,
		IDbTransaction transaction,
		OutboxMessageResponse outboxMessage,
		Exception? exception
	)
	{
		const string sql =
			@"
			UPDATE outbox_messages
			SET processed_on_utc = @ProcessedOnUtc, error = @Error
			WHERE id = @Id";

		await connection.ExecuteAsync(
			sql,
			new
			{
				outboxMessage.Id,
				ProcessedOnUtc = dateTimeProvider.UtcNow,
				Error = exception?.ToString()
			},
			transaction: transaction
		);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection, IDbTransaction transaction)
	{
		var sql =
			$@"
				SELECT id, content
			    FROM outbox_messages
			    WHERE processed_on_utc IS NULL
			    ORDER BY occurred_on_utc
			    LIMIT {_outboxOptions.BatchSize}
                FOR UPDATE
			";

		var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

		return outboxMessages.ToList();
	}

	internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
