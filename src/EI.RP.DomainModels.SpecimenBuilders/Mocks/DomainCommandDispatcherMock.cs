using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using Moq;

namespace EI.RP.DomainModels.SpecimenBuilders.Mocks
{
	public class DomainCommandDispatcherMock : IDomainCommandDispatcher
	{
		private readonly DomainQueryResolverMock _queryResolverMock;

		public DomainCommandDispatcherMock(DomainQueryResolverMock queryResolverMock)
		{
			_queryResolverMock = queryResolverMock;
			Reset();
		}

		public Mock<IDomainCommandDispatcher> Reset()
		{
			Current = new Mock<IDomainCommandDispatcher>();
			return Current;
		}

		public Mock<IDomainCommandDispatcher> Current { get; private set; } = new Mock<IDomainCommandDispatcher>();

		public Task ExecuteAsync<TCommand>(TCommand command,bool byPassPipeline=false) where TCommand : IDomainCommand
		{
			SyncQueries();
		
			return Current.Object.ExecuteAsync(command, byPassPipeline);

			void SyncQueries()
			{
				if (command.GetType() == typeof(RecordMovingOutProgress))
				{
					var cmd = command as RecordMovingOutProgress;
					_queryResolverMock.ExpectQuery(new MoveHouseProgressQuery
					{
						MoveType = cmd.MoveType,
						InitiatedFromAccount = cmd.InitiatedFromAccount,
						OtherAccount = cmd.OtherAccount
					}, new[]
					{
						new MovingHouseInProgressMovingOutInfo
						{
							ElectricityMeterReadingNightOrNshValue = cmd.ElectricityMeterReadingNightOrNshValue,
							ElectricityMeterReadingDayOr24HrsValue = cmd.ElectricityMeterReading24HrsOrDayValue,
							GasMeterReadingValue = cmd.GasMeterReadingValue,
							IncomingOccupant = cmd.IncomingOccupant,
							InformationCollectionAuthorized = cmd.InformationCollectionAuthorized,
							InitiatedFromAccountNumber = cmd.InitiatedFromAccount.AccountNumber,
							LettingPhoneNumber = cmd.LettingPhoneNumber,
							LettingAgentName = cmd.LettingAgentName,
							MovingOutDate = cmd.MovingOutDate,
							OccupierDetailsAccepted = cmd.OccupierDetailsAccepted,
							OtherAccountNumber = cmd.OtherAccount?.AccountNumber,
							TermsAndConditionsAccepted = cmd.TermsAndConditionsAccepted
						}
					});
					_queryResolverMock.ExpectQuery(new MoveHouseProgressQuery
					{
						
						MoveType = cmd.MoveType,
						InitiatedFromAccount = cmd.InitiatedFromAccount,
						OtherAccount = cmd.OtherAccount
					}, new[]
					{
						new MovingHouseInProgressNewPRNsInfo()
					});
					_queryResolverMock.ExpectQuery(new MoveHouseProgressQuery
					{MoveType = cmd.MoveType,
						InitiatedFromAccount = cmd.InitiatedFromAccount,
						OtherAccount = cmd.OtherAccount
					}, new[]
					{
						new MovingHouseInProgressMovingInInfo()
					});
				}
				else if (command.GetType() == typeof(RecordMovingHomePrns))
				{
					var cmd = command as RecordMovingHomePrns;
					_queryResolverMock.ExpectQuery(new MoveHouseProgressQuery
					{
						MoveType = cmd.MoveType,
						InitiatedFromAccount = cmd.InitiatedFromAccount,
						OtherAccount = cmd.OtherAccount
					}, new[]
					{
						new MovingHouseInProgressNewPRNsInfo
						{
							NewGprn = cmd.NewGprn,
							NewMprn = cmd.NewMprn
						}
					});
				}
				else if (command.GetType() == typeof(RecordMovingInProgress))
				{
					var cmd = command as RecordMovingInProgress;
					_queryResolverMock.ExpectQuery(new MoveHouseProgressQuery
					{
						MoveType = cmd.MoveType,
						InitiatedFromAccount = cmd.InitiatedFromAccount,
						OtherAccount = cmd.OtherAccount
					}, new[]
					{
						new MovingHouseInProgressMovingInInfo
						{
							ElectricityMeterReadingNightOrNshValue = cmd.ElectricityMeterReadingNightValueOrNshValue,
							ElectricityMeterReadingDayOr24HrsValue = cmd.ElectricityMeterReading24HrsOrDayValue,
							GasMeterReadingValue = cmd.GasMeterReadingValue,
							ContactNumber = cmd.ContactNumber,
							MovingInDate = cmd.MovingInDate
						}
					});
				}
			}
		}




		public DomainCommandDispatcherMock ExpectCommandAndSuccess<TCommand>(TCommand expectedCommand,Action<TCommand> mockedCommandBehaviour=null,Func<TCommand, TCommand  ,bool> commandComparer=null)
			where TCommand : IDomainCommand
		{
			void ActionWithByPass(TCommand cmd, bool b) => mockedCommandBehaviour(cmd);

			var a = Current.Setup(x =>
				x.ExecuteAsync(
					It.Is<TCommand>(c => commandComparer == null ? c.Equals(expectedCommand) : commandComparer(expectedCommand,c)),
					false));
			if (mockedCommandBehaviour != null)
			{
				a.Callback<TCommand,bool>(ActionWithByPass).Returns(Task.CompletedTask);
			}


			return this;
		}

		public DomainCommandDispatcherMock ExpectCommandAndThrow<TCommand>(TCommand expectedCommand, DomainException domainException)
			where TCommand : IDomainCommand
		{

			Current.Setup(x => x.ExecuteAsync(It.Is<TCommand>(c => c.Equals(expectedCommand)), false)).Throws(new AggregateException(domainException));
			return this;
		}

		public void AssertCommandWasExecuted<TCommand>(TCommand command) 
			where TCommand:IDomainCommand
		{
			Current.Verify(x=>x.ExecuteAsync(command,false),Times.Once);
		}
		public void AssertCommandWasExecuted<TCommand>()
			where TCommand : IDomainCommand
		{
			Current.Verify(x => x.ExecuteAsync(It.IsAny<TCommand>(), false), Times.Once);
		}
		public void AssertCommandWasNotExecuted<TCommand>(TCommand command)
            where TCommand : IDomainCommand
        {
            Current.Verify(x => x.ExecuteAsync(command, false), Times.Never);
        }
        public void AssertCommandWasNotExecuted<TCommand>()
	        where TCommand : IDomainCommand
        {
	        Current.Verify(x => x.ExecuteAsync(It.IsAny<TCommand>(), false), Times.Never);
        }
	}
}
