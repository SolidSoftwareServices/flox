using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EI.RP.CoreServices.System
{
	public class TypesFinder
	{
		public static TypesFinder Resolver { get; private set; } =new TypesFinder();


		public static void Reset()
		{
			Resolver=new TypesFinder();
		}

		private TypesFinder()
		{
		}

		private readonly ConcurrentDictionary<string, IEnumerable<Type>> _findTypesCache =
			new ConcurrentDictionary<string, IEnumerable<Type>>();
		public Type FindType(string typeName,bool includePublicTypesOnly=true)
		{
			var type = FindTypes(x => x.FullName == typeName, typeName,includePublicTypesOnly).SingleOrDefault();
			if (type == null)
			{
				try
				{
					type = Type.GetType(typeName);
				}
				catch (FileLoadException)
				{
					type = null;
				}
			
				_findTypesCache[typeName] = type?.ToOneItemArray()??new Type[0];

			}

			return type;
		}

		public IEnumerable<Type> FindTypes(Func<Type, bool> predicate, string cacheQueryId = null, bool includePublicOnly = true)
		{
			return cacheQueryId == null ? Fetch() : _findTypesCache.GetOrAdd(cacheQueryId, x => Fetch());

			IEnumerable<Type> Fetch()
			{
				if (predicate == null)
					throw new ArgumentNullException(nameof(predicate));
				var result = new HashSet<Type>();
				foreach (var assembly in AllAssemblies.Value)
				{
					if (!assembly.IsDynamic)
					{
						Type[] exportedTypes = null;
						try
						{
							
							exportedTypes = assembly.GetTypes()
								.Where(x=>
									(includePublicOnly && (x.IsPublic ||x.IsNestedPublic))
									||(!includePublicOnly))
								.ToArray();
						}
						catch (ReflectionTypeLoadException e)
						{
							exportedTypes = e.Types;
						}

						if (exportedTypes != null)
						{
							foreach (var type in exportedTypes)
							{
								if (predicate(type))
									result.Add(type);
							}
						}
					}
				}
			
				return result;
			}
		}


		private static readonly Lazy<IEnumerable<Assembly>> AllAssemblies = new Lazy<IEnumerable<Assembly>>(() =>
		{
			var avoid = new[] {"Microsoft.TestPlatform.Utilities", "System.Threading.AccessControl", "TechTalk.SpecRun.Common", "EI.RP.WebApp.Views" };
			var value = typeof(TypesFinder).Namespace.Split('.')[0] + ".";

			var result = new List<Assembly>();
			var list = new List<string>();
			var stack = new Stack<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

			stack.Push(Assembly.GetEntryAssembly());
			do
			{
				var asm = stack.Pop();
				if (!result.Any(x => x.Equals(asm)))
				{
					result.Add(asm);
				}

				foreach (var reference in asm.GetReferencedAssemblies())
				{
					if (reference.FullName.StartsWith("EI",StringComparison.InvariantCultureIgnoreCase)
						&& !list.Contains(reference.FullName) 
						&& reference.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase)
					 && !avoid.Any(reference.FullName.StartsWith))
					{
						try
						{
							stack.Push(Assembly.Load(reference));
							list.Add(reference.FullName);
						}
						catch (FileNotFoundException)
						{

						}
					}

				}
			} while (stack.Count > 0);

			return result;
		});

	
		private readonly string _allTypesQueryKey = Guid.NewGuid().ToString();

		public IEnumerable<Type> AllTypes()
		{
			return FindTypes(x => true, _allTypesQueryKey);
		}

		public IEnumerable<Type> FindTypesByNamespace(string ns)
		{
			return FindTypes(x => x.Namespace != null &&
			                      x.Namespace.StartsWith(ns,
				                      StringComparison.InvariantCultureIgnoreCase), $"ByNamespace_{ns}");
		}

		public IEnumerable<Type> FindConcreteTypesOf(Type type, bool includePublicOnly = true)
		{
			return FindTypes(x =>
				!x.IsAbstract && type.IsAssignableFrom(x),$"FindConcreteTypesOf_{type.FullName}", includePublicOnly);

		}
		public IEnumerable<Type> FindConcreteTypesOf<T>(bool includePublicOnly=true)
		{
			return FindConcreteTypesOf(typeof(T), includePublicOnly);
		}
		
		public IEnumerable<Type> FindConcreteTypesOf(Type t1, Type t2)
		{
			return FindTypes(x =>
				!x.IsAbstract && (t1.IsAssignableFrom(x)|| t2.IsAssignableFrom(x)), $"FindConcreteTypesOf_{t1.FullName}_or_{t2.FullName}");

		}

		public IEnumerable<Type> FindConcreteTypesOf<T1,T2>()
		{
			return FindConcreteTypesOf(typeof(T1), typeof(T2));
		}

		public IEnumerable<Type> FindConcreteTypesOfOpenGeneric(params Type[] openGenericTypes)
		{
			return FindTypes(x =>
				!x.IsAbstract && openGenericTypes.Any(x.ImplementsOpenGeneric), $"FindConcreteTypesOfOpenGeneric_{string.Join(",",openGenericTypes.Select(x=>x.FullName).OrderBy(x=>x))}");
		}
	}
}