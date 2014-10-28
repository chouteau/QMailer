using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Timers;

namespace QMailer.Web
{
	public class SimpleCacheService : ICacheService
	{
		private SynchronizedCollection<CacheEntry> m_CachedList;
		Timer m_Timer;
		private static object m_Lock = new object();

		public SimpleCacheService()
		{
			m_CachedList = new SynchronizedCollection<CacheEntry>();
			m_Timer = new Timer(1000 * 60); // Toutes les 60 secondes
			m_Timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
			m_Timer.Start();

		}

		void OnTimerElapsed(object sender, ElapsedEventArgs arg)
		{
			while (true)
			{
				var item = m_CachedList.FirstOrDefault(i => i.ExpirationDate < arg.SignalTime);
				if (item == null)
				{
					break;
				}
				m_CachedList.Remove(item);
			}

			GlobalConfiguration.Configuration.Logger.Debug("Cache item count : {0} items", ItemCount);
		}
		
		#region ICacheService Members

		public int ItemCount
		{
			get
			{
				return m_CachedList.Count;
			}
		}

		public object this[string key]
		{
			get 
			{
				object result = null;
				lock (m_CachedList.SyncRoot)
				{
					var item = m_CachedList.SingleOrDefault(i => i.Key == key);
					if (item != null)
					{
						result = item.Value;
					}
				}
				return result;
			}
		}

		public void Add(string key, object value, DateTime absoluteExpiration)
		{
			CacheEntry cacheEntry = null;
			lock (m_CachedList.SyncRoot)
			{
				cacheEntry = m_CachedList.SingleOrDefault(i => i.Key == key);
				if (cacheEntry == null)
				{
					cacheEntry = new CacheEntry()
					{
						Key = key,
						Value = value,
						ExpirationDate = absoluteExpiration
					};
					m_CachedList.Add(cacheEntry);
				}
				else
				{
					cacheEntry.Value = value;
				}
			}
		}

		public object Get(string key)
		{
			return this[key];
		}

		public void Remove(string key)
		{
			CacheEntry cacheEntry = null;
			lock (m_CachedList.SyncRoot)
			{
				cacheEntry = m_CachedList.SingleOrDefault(i => i.Key == key);
				if (cacheEntry != null)
				{
					m_CachedList.Remove(cacheEntry);
				}
			}
		}

		public void ClearAll()
		{
			m_CachedList.Clear();
		}

		public T FirstOrDefault<T>(Func<T, bool> predicate)
		{
			var list = m_CachedList.Where(i => i.Value.GetType() == typeof(T)).Select(i => i.Value).Cast<T>();
			return list.FirstOrDefault(predicate);
		}

		public IEnumerable<T> GetListOf<T>(Func<T, bool> predicate)
		{
			var list = m_CachedList.Where(i => i.Value.GetType() == typeof(T)).Select(i => i.Value).Cast<T>();
			return list.Where(predicate);
		}

		public IQueryable<T> GetListOf<T>()
		{
			var list = m_CachedList.Where(i => i.Value.GetType() == typeof(T)).Select(i => i.Value).Cast<T>();
			return list.AsQueryable();
		}

		#endregion
	}
}
