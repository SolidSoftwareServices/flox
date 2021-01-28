using System;
using System.Threading.Tasks;
using FluentValidation;

namespace EI.RP.CoreServices.Cqrs.Commands
{
	public abstract class CommandValidator<TCommand> : ICommandValidator<TCommand> 
		where TCommand : IDomainCommand
	{
		private class Validator : AbstractValidator<TCommand>
		{
            
		}

		private readonly Lazy<Validator> _innerValidator;

		protected CommandValidator()
		{
			_innerValidator=new Lazy<Validator>(()=>
			{
				var result = new Validator();
				RegisterValidations(result);
				return result;
			});
		}

		protected abstract void RegisterValidations(AbstractValidator<TCommand> validation);

		public async Task ValidateAsync(TCommand command)
		{
			await Task.WhenAll(_innerValidator.Value.ValidateAndThrowAsync(command), OnValidateComplexAsync(command));
		}
		/// <summary>
		/// validate complex operations that are not covered by the fluent validator
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual Task OnValidateComplexAsync(TCommand command)
		{
			return Task.CompletedTask;
		}
	}
}