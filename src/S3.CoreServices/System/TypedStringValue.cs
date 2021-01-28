using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NLog;

namespace S3.CoreServices.System
{
	[DebuggerDisplay("{DebuggerDisplayValue}")]
	public abstract class TypedStringValue
	{
		private string _valueId;

		[JsonProperty]
		protected virtual string ValueId
		{
			get => _valueId;
			set
			{
				if (!ValidValues.GetOrAdd(GetType(), (t) =>
				{
					return t.GetFields(BindingFlags.Public | BindingFlags.Static)
						.Where(x => typeof(TypedStringValue).IsAssignableFrom(x.DeclaringType))
						.Select(x => x.GetValue(null)).Cast<string>();
				}).Any(x => x == null && _valueId == null || x != null && x.Equals(_valueId)))
				{
					throw new InvalidOperationException("Value is not valid");
				}

				_valueId = value;

			}
		}

		private static readonly ConcurrentDictionary<Type, IEnumerable<string>> ValidValues =
			new ConcurrentDictionary<Type, IEnumerable<string>>();



		/// <summary>
		/// values are case sensitive or not
		/// </summary>
		[JsonIgnore]
		public virtual bool CaseSensitive { get; } = false;



		protected TypedStringValue()
		{

		}

		protected bool IsValueDefinition { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="valueId"></param>
		/// <param name="debuggerFriendlyDisplayValue"></param>
		/// <param name="disableVerifyValueExists">values are not defined in a single class</param>
		protected TypedStringValue(string valueId, string debuggerFriendlyDisplayValue, bool disableVerifyValueExists) : this()
		{
			IsValueDefinition = true;
			VerifyValueExists = !disableVerifyValueExists;
			ValueId = valueId;

			_debuggerFriendlyDisplayValue = string.IsNullOrWhiteSpace(debuggerFriendlyDisplayValue) ? valueId : debuggerFriendlyDisplayValue;
		}

		protected bool VerifyValueExists { get; }


		public bool Equals(TypedStringValue other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(ValueId, other.ValueId,
				CaseSensitive ? StringComparison.InvariantCulture : StringComparison.CurrentCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TypedStringValue)obj);
		}

		public override int GetHashCode()
		{
			return (ValueId != null ? (CaseSensitive ? ValueId : ValueId.ToLower()).GetHashCode() : 0);
		}


		public static bool operator ==(TypedStringValue left, TypedStringValue right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TypedStringValue left, TypedStringValue right)
		{
			return !Equals(left, right);
		}


		public override string ToString()
		{
			return ValueId;
		}

		/// <summary>
		/// this is not an ui display value
		/// </summary>
		private readonly string _debuggerFriendlyDisplayValue;

		public string DebuggerDisplayValue =>
			string.IsNullOrWhiteSpace(_debuggerFriendlyDisplayValue) ? ValueId : $"{_debuggerFriendlyDisplayValue}({ValueId})";




	}

	public abstract class TypedStringValue<T> : TypedStringValue
		where T : TypedStringValue
	{

		public static bool CanParse(string value)
		{
			return AllValues.Any(x => x.ValueId == value);
		}

		private static readonly Lazy<HashSet<TypedStringValue<T>>> _RegisteredValues =
			new Lazy<HashSet<TypedStringValue<T>>>(() =>

				typeof(T).GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static)
					.Where(x => x.FieldType == typeof(T))
					.Select(x => x.GetValue(null)).Cast<TypedStringValue<T>>().ToHashSet()
			);

		public static IEnumerable<TypedStringValue<T>> AllValues => _RegisteredValues.Value.ToArray();
		private string _valueId;

		protected TypedStringValue() : base()
		{
		}

		protected TypedStringValue(string value, string debuggerFriendlyDisplayValue = null,
			bool disableVerifyValueExists = false) : base(value, debuggerFriendlyDisplayValue, disableVerifyValueExists)
		{
		}

		[JsonProperty]
		protected override string ValueId
		{
			get { return _valueId; }
			set
			{

				if (VerifyValueExists
					&& !IsValueDefinition
					&& !_RegisteredValues.Value.Any(x =>
						string.Compare(x.ToString(), value, StringComparison.InvariantCultureIgnoreCase) == 0))
					throw new InvalidCastException("Value not valid");
				_valueId = value;
			}
		}

		public static T From(string src)
		{
			var result = ((TypedStringValue<T>)src) as T;
			if (result == null) throw new InvalidCastException();
			return result;
		}

		private static readonly object SyncLock=new object();
		public static implicit operator TypedStringValue<T>(string src)
		{

			var result = AllValues.SingleOrDefault(x =>
				string.Compare(x._valueId, src, StringComparison.InvariantCultureIgnoreCase) == 0);
			if (result == null && src != null)
			{
				if (AllValues.First().VerifyValueExists)
				{
					throw new InvalidCastException();
				}

				result = AllValues.SingleOrDefault(x => x.ValueId == src);
				if (result == null)
				{
					lock (SyncLock)
					{
						result = AllValues.SingleOrDefault(x => x.ValueId == src);
						if (result == null)
						{
							result = ObjectBuilder.Default.Build<T>() as TypedStringValue<T>;
							result.ValueId = src;

							_RegisteredValues.Value.Add(result);
							Logger.Warn(() =>
								$"Not implemented: mapping type {nameof(T)} has not declared a known mapping for {src}.");
						}
					}
				}
			}
			return result;
		}

		protected static readonly ILogger Logger = LogManager.GetLogger(typeof(T).FullName);
		public static implicit operator string(TypedStringValue<T> src)
		{
			return src?.ToString();
		}

		public bool IsOneOf(T other, params T[] others)
		{
			return this.Equals(other) || others.Any(Equals);
		}

	
	}


}