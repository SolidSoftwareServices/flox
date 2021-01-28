using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using NLog;

namespace EI.RP.CoreServices.InProc
{
    class InProcDomainCommandDispatcher : IDomainCommandDispatcher
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider _container;
        private readonly IEventApiEventPublisher _eventsApiEventPublisher;
        private readonly IProfiler _profiler;
        private readonly ICacheProvider _cacheProvider;
        private readonly IUserSessionProvider _userSessionProvider;
        private readonly ICacheAccountPreLoaderRequester _cacheRequester;

        public InProcDomainCommandDispatcher(IServiceProvider container,IEventApiEventPublisher eventsApiEventPublisher,IProfiler profiler, ICacheProvider cacheProvider, IUserSessionProvider userSessionProvider, ICacheAccountPreLoaderRequester cacheRequester)
        {
            _container = container;
            _eventsApiEventPublisher = eventsApiEventPublisher;
            _profiler = profiler;
            _cacheProvider = cacheProvider;
            _userSessionProvider = userSessionProvider;
            _cacheRequester = cacheRequester;
        }

        public async Task ExecuteAsync<TCommand>(TCommand command, bool byPassPipeline = false)
            where TCommand : IDomainCommand
        {
	        const string profileCategoryId = "DomainCommand";
	        using (_profiler.Profile(profileCategoryId, typeof(TCommand).Name))
			{
				var eventBuilder =
					(ICommandEventBuilder<TCommand>)_container.GetService(typeof(ICommandEventBuilder<TCommand>));

				Logger.Debug(() =>
					eventBuilder == null ? $"No event builder defined for {typeof(TCommand).FullName}" : string.Empty);
				try
				{
					await RunValidation();

					await RunPayload();

					await RunSuccessfulEvent(eventBuilder);

					await UpdateCache();
				}
				catch (Exception ex)
				{
					await UpdateCache();
					using (_profiler.Profile(profileCategoryId, $"Handle domain error of {typeof(TCommand).Name}"))
					{
						await HandleDomainError(command, ex, eventBuilder,byPassPipeline);
					}
				}
			}

	        async Task RunValidation()
	        {

		        using (_profiler.Profile(profileCategoryId, $"Validate {typeof(TCommand).Name}"))
		        {
			        await ValidateCommand(command);
		        }
	        }

	        async Task RunSuccessfulEvent(ICommandEventBuilder<TCommand> eventBuilder)
	        {
		        if (!byPassPipeline)
		        {
			        using (_profiler.Profile(profileCategoryId, $"Publish Event of {typeof(TCommand).Name}"))
			        {
				        await PublishSuccessfulOperationEvent(eventBuilder, command);
			        }
		        }
	        }

	        async Task RunPayload()
	        {
		        using (_profiler.Profile(profileCategoryId, $"Perform {typeof(TCommand).Name}"))
		        {
			        await PerformPayload(command);
		        }
	        }

	        async Task UpdateCache()
	        {
		        if (command.InvalidatesCache)
		        {
			        var cacheContext = !_userSessionProvider.IsAnonymous()
				        ? _userSessionProvider.ActingAsUserName ?? _userSessionProvider.UserName
				        : null;

			        if (command.InvalidateQueriesOnSuccess != null && command.InvalidateQueriesOnSuccess.Any())
			        {
				        await _cacheProvider.InvalidateAsync(cacheContext,default(CancellationToken),
					        command.InvalidateQueriesOnSuccess.ToArray());
			        }
			        else
			        {
				        await _cacheProvider.ClearAsync(cacheContext);
			        }

			        if (cacheContext != null)
			        {
				        await _cacheRequester.SubmitRequestAsync(cacheContext);
			        }
		        }
	        }
        }

        private async Task HandleDomainError<TCommand>(TCommand command, Exception ex,
	        ICommandEventBuilder<TCommand> eventBuilder, bool byPassPipeline)
            where TCommand : IDomainCommand
        {
//every domain exception must be aggregate exception
            var exceptionToRethrow =
                !(ex is AggregateException) ? new AggregateException(ex) : (AggregateException) ex;

            var innerExceptions = exceptionToRethrow.InnerExceptions.ToList();
            var changed = false;
            for (var i = 0; i < innerExceptions.Count; i++)
            {
                var e = innerExceptions[i];
                if (!(e is DomainException))
                {
                    innerExceptions[i] = new DomainException(DomainError.Undefined, e);
                    changed = true;
                }
            }

            if (changed)
            {
                exceptionToRethrow = new AggregateException(innerExceptions);
            }

            if (!byPassPipeline)
            {
	            try
	            {
		            await PublishOperationFailedEvent(eventBuilder, command, exceptionToRethrow);
	            }
	            catch (Exception e)
	            {
		            exceptionToRethrow = new AggregateException(exceptionToRethrow.InnerExceptions.Union(new[] {e}));
	            }
            }

            throw exceptionToRethrow;
        }

        private async Task PublishOperationFailedEvent<TCommand>(ICommandEventBuilder<TCommand> commandEventBuilder,
            TCommand command, AggregateException exceptions) where TCommand : IDomainCommand
        {
            //some commands might not publish events
            if (commandEventBuilder != null)
            {
                var errorEvents = await commandEventBuilder.BuildEventsOnError(command,exceptions);

                if (errorEvents != null)
                {
                    await PublishEvents(errorEvents.Select(x => x.Validate()).ToArray());
                    Logger.Debug(() => $"Published event {errorEvents}");
                }
                else
                {
                    Logger.Debug(() => $"Event builder of {typeof(TCommand).FullName} does not build events on error");
                }
            }
        }

        private async Task PublishSuccessfulOperationEvent<TCommand>(ICommandEventBuilder<TCommand> commandEventBuilder,
            TCommand command) where TCommand : IDomainCommand
        {
            //some commands might not publish events
            if (commandEventBuilder != null)
            {
                var successfulEvents = await commandEventBuilder.BuildEventsOnSuccess(command)??new IEventApiMessage[0];
                if (successfulEvents.Any())
                {
                    await PublishEvents(successfulEvents.Select(x => x.Validate()).ToArray());
                }
                else
                {
                    Logger.Debug(()=>$"Event builder of {typeof(TCommand).FullName} does not build events on success");
                }
            }
        }

        private async Task PublishEvents(IEventApiMessage[] eventsToPublish)
        {
            var tasks = new List<Task>(eventsToPublish.Length);
            foreach (var @event in eventsToPublish)
            {
                tasks.Add(_eventsApiEventPublisher.Publish(@event));
            }

            await Task.WhenAll(tasks);
        }

        private async Task PerformPayload<TCommand>(TCommand command) where TCommand : IDomainCommand
        {
            var commandHandler = (ICommandHandler<TCommand>) _container.GetService(typeof(ICommandHandler<TCommand>));
            await commandHandler.ExecuteAsync(command);
        }

        private async Task ValidateCommand<TCommand>(TCommand commandData) where TCommand : IDomainCommand
        {

            var commandValidator = (ICommandValidator<TCommand>)_container.GetService(typeof(ICommandValidator<TCommand>));
            if (commandValidator != null)
            {
                await commandValidator.ValidateAsync(commandData);
            }
        }
    }
}
