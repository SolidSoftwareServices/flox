using System;

namespace EI.RP.CoreServices.Ports.OData
{
	

	public abstract class ODataDtoModel : EntityContainerItem
	{
		private  string _collectionName;
		private bool _useOdata=true;

		[Obsolete("will be removed with odata v4")]
		public void SetAddAsOdata(bool useOdata)
		{
			_useOdata = useOdata;
		}
		[Obsolete("will be removed withodata v4")]
		public  virtual bool AddsAsOData() => _useOdata;

		public static string ResolveCollectionNameOf<T>() where T : ODataDtoModel, new()
		{
			return new T().CollectionName();
			
		}
		private string ResolveCollectionNameOf(Type type)
		{
			var a = type.Name;
			if (a.EndsWith("Dto"))
			{
				a = a.Substring(0, a.Length - 3);
			}

			return a;
		}

		public virtual string CollectionName() => _collectionName??(_collectionName=ResolveCollectionNameOf(GetType()));

		/// <summary>
		/// indicates whether can update partial object or must be the full one(false)
		/// </summary>
		/// <returns></returns>
		public virtual ODataUpdateType UpdateMode() => ODataUpdateType.OnlyChangedValues;

		public virtual object[] UniqueId()
		{
			return new object[0];
		}

	}
}