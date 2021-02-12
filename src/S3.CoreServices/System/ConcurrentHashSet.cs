using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace S3.CoreServices.System
{
	[DebuggerDisplay("Count = {Count}")]
	public class ConcurrentHashSet<T> : ISet<T>
	{
		private readonly HashSet<T> _hashSet = new HashSet<T>();
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		public ConcurrentHashSet()
		{ }

		public ConcurrentHashSet(IEqualityComparer<T> comparer)
		{
			_hashSet = new HashSet<T>(comparer);
		}

		public ConcurrentHashSet(IEnumerable<T> collection)
		{
			_hashSet = new HashSet<T>(collection);
		}

		public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			_hashSet = new HashSet<T>(collection, comparer);
		}


		public IEnumerator<T> GetEnumerator()
		{
			return _hashSet.GetEnumerator();
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		public void Add(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				_hashSet.Add(item);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool AddIfNotExists(T item)
		{
			bool added;
			_lock.EnterWriteLock();
			try
			{

				added = _hashSet.Add(item);

			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}

			return added;
		}

		public void UnionWith(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			_lock.EnterReadLock();
			try
			{
				_hashSet.UnionWith(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
				if (_lock.IsReadLockHeld) _lock.ExitReadLock();
			}
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			_lock.EnterReadLock();
			try
			{
				_hashSet.IntersectWith(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
				if (_lock.IsReadLockHeld) _lock.ExitReadLock();
			}
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			_lock.EnterReadLock();
			try
			{
				_hashSet.ExceptWith(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
				if (_lock.IsReadLockHeld) _lock.ExitReadLock();
			}
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				_hashSet.SymmetricExceptWith(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.IsSubsetOf(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.IsSupersetOf(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.IsProperSupersetOf(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.IsProperSubsetOf(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.Overlaps(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.SetEquals(other);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		bool ISet<T>.Add(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.Add(item);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public void Clear()
		{
			_lock.EnterWriteLock();
			try
			{
				_hashSet.Clear();
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool Contains(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.Contains(item);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_lock.EnterWriteLock();
			try
			{
				_hashSet.CopyTo(array, arrayIndex);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public bool Remove(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				return _hashSet.Remove(item);
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}
		}

		public ConcurrentHashSet<T> RemoveWhere(Func<T, bool> predicate)
		{
			_lock.EnterWriteLock();
			try
			{
				var items = _hashSet.Where(predicate).ToArray();
				foreach (var item in items)
				{
					_hashSet.Remove(item);
				}
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}

			return this;
		}

		public int Count
		{
			get
			{
				_lock.EnterWriteLock();
				try
				{
					return _hashSet.Count;
				}
				finally
				{
					if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
				}
			}
		}

		public bool IsReadOnly => false;

		public void AddRange(IEnumerable<T> range)
		{
			_lock.EnterWriteLock();
			try
			{
				foreach (var item in range)
				{
					_hashSet.Add(item);
				}
			}
			finally
			{
				if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
			}

		}
	}
}