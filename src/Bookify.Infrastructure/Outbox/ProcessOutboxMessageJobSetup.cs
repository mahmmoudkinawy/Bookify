using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

internal class ProcessOutboxMessageJobSetup(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
	private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

	public void Configure(QuartzOptions options)
	{
		const string jobName = nameof(ProcessOutboxMessageJob);

		options
			.AddJob<ProcessOutboxMessageJob>(configure => configure.WithIdentity(jobName))
			.AddTrigger(configure =>
				configure
					.ForJob(jobName)
					.WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever())
			);
	}
}
