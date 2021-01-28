using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Validation
{
	class ProxyModelValidator : IProxyModelValidator
	{
		public void Validate<TModel>(TModel model, ProxyModelOperation operation)
		{
			//TODO: THE API metadata is misleading
			ValidateOperation<TModel>(operation);
			ValidateModelValues(model,operation);
		}
		private static readonly ConcurrentDictionary<Type, (bool CanQuery,bool CanAdd,bool CanUpdate,bool CanDelete)> _modelOperations=new ConcurrentDictionary<Type, (bool CanQuery, bool CanAdd, bool CanUpdate, bool CanDelete)>();
		private void ValidateOperation<TModel>(ProxyModelOperation operation)
		{
			//Since the service metadata is  not accurate, we can only enforce this in development
			//IMPORTANT: if a crud operation was generated from a wrong metadata, notify the SAP team and regenerate the proxy once they fix it
			//if (!Debugger.IsAttached)
			//{
			//	return;
			//}

			var operationsAllowed =_modelOperations.GetOrAdd(typeof(TModel),(key)=>
			{
				var attribute = (CrudOperationsAttribute) key
					.GetCustomAttributes(typeof(CrudOperationsAttribute), false).SingleOrDefault();

				return (CanQuery:attribute?.CanQuery??true, CanAdd:attribute?.CanAdd??true,CanUpdate:attribute?.CanUpdate??true,CanDelete:attribute?.CanDelete??true);
			});
			var canPerform = ResolveCanPerform();

			if (!canPerform)
			{
				throw new DomainException(DomainError.GeneralValidation, new ValidationException($"{typeof(TModel).Name} is not allowed to {operation}"));
			}

			bool ResolveCanPerform()
			{
				bool result = true;
				switch (operation)
				{
					case ProxyModelOperation.Query:
						result = operationsAllowed.CanQuery;
						break;
					case ProxyModelOperation.Add:
						result = operationsAllowed.CanAdd;
						break;
					case ProxyModelOperation.Update:
						result = operationsAllowed.CanUpdate;
						break;
					case ProxyModelOperation.Delete:
						result = operationsAllowed.CanDelete;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
				}

				return result;
			}
		}

		private static void ValidateModelValues<TModel>(TModel model, ProxyModelOperation operation)
		{
			if (operation.IsOneOf(ProxyModelOperation.Add, ProxyModelOperation.Update))
			{
				var context = new ValidationContext(model, serviceProvider: null, items: null);
				var validationResults = new List<ValidationResult>();
				if (!Validator.TryValidateObject(model, context, validationResults, true))
				{
					var message = string.Join(Environment.NewLine, validationResults
						.Select(x => x.ErrorMessage));
					throw new DomainException(DomainError.GeneralValidation, new ValidationException(message));
				}
			}
		}
	}
}
